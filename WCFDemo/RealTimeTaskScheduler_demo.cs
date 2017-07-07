using Octopus.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WCFDemo
{
    public class MyTaskScheduler<TTaskKey>
    {
        /// <summary>
        /// 每个轮询时刻任务到时后的动作
        /// </summary>
        /// <param name="taskKey">任务的唯一标识</param>
        /// <returns>是否继续保留在定时列表</returns>
        public delegate bool TimeUpHandler(TTaskKey taskKey);
        private readonly int MAX_POLL_WAIT_MS = 30 * 1000; // 最大轮询间隔时间
        private readonly int MIN_POLL_WAIT_MS = 1000; // 最小轮询间隔时间
        private TimeUpHandler _timeUpHandle;
        private readonly object _scheduledTasksLock = new object(); // 定时列表访问互斥锁
        Dictionary<TTaskKey, TaskContainer> _scheduledTasks = new Dictionary<TTaskKey, TaskContainer>(); // 定时列表
        public MyTaskScheduler(TimeUpHandler timeUpHandler, int minIntervalMS = 1000, int maxIntervalMS = 30 * 1000)
        {
            if (timeUpHandler == null) throw new ArgumentNullException(nameof(timeUpHandler));
            if (minIntervalMS <= 0) throw new ArgumentOutOfRangeException(nameof(minIntervalMS));
            if (maxIntervalMS <= 0) throw new ArgumentOutOfRangeException(nameof(maxIntervalMS));
            if (maxIntervalMS < minIntervalMS) throw new ArgumentOutOfRangeException(nameof(maxIntervalMS) + " should larger than or equals with " + nameof(minIntervalMS));

            _timeUpHandle = timeUpHandler;
            MIN_POLL_WAIT_MS = minIntervalMS;
            MAX_POLL_WAIT_MS = maxIntervalMS;
        }

        /// <summary>
        /// 是否在轮询的标识
        /// </summary>
        private bool _isPolling = false;
        // 开始轮询
        public void StartPolling()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Stopwatch processWatch = new Stopwatch();
            Task pollingTask = new Task(() =>
            {
                _isPolling = true;
                List<TTaskKey> removingTaskIds = new List<TTaskKey>();
                int waitingMS = MIN_POLL_WAIT_MS;
                long lastIntervalElapsedTotal = 0;
                while (_isPolling)
                {
                    try
                    {
                        LogHelper.LogDebug("=======================Polling==========================");
                        // pre-process: 决定本次启动列表
                        var runningTaskIds = new List<TTaskKey>();
                        if (_scheduledTasks.Count > 0)
                        {
                            long currentIntervalElapsedTotal = stopwatch.ElapsedMilliseconds;
                            long intervalElapsed = currentIntervalElapsedTotal - lastIntervalElapsedTotal;// P1 记录上次 pre-process 和 runing消耗的时间
                            lastIntervalElapsedTotal = currentIntervalElapsedTotal;
                            lock (_scheduledTasksLock) // 占用锁，阻塞其他地方对_scheduledTasks的修改（主要是和启/停互斥）
                            {
                                // 删除上次需要删除的任务：启动失败
                                for (int i = 0; i < removingTaskIds.Count; i++)
                                {
                                    if (removingTaskIds[i] != null)
                                        _scheduledTasks.Remove(removingTaskIds[i]);
                                }
                                removingTaskIds.Clear();
                                runningTaskIds.Clear();
                                foreach (var item in _scheduledTasks)
                                {
                                    item.Value.CurrentLeftMillionSeconds -= intervalElapsed; //elapsed
                                    if (item.Value.CurrentLeftMillionSeconds <= 0) // timeup
                                    {
                                        runningTaskIds.Add(item.Key);
                                        _scheduledTasks[item.Key].CurrentLeftMillionSeconds = _scheduledTasks[item.Key].IntervalMillionSeconds + _scheduledTasks[item.Key].CurrentLeftMillionSeconds;
                                    }
                                }
                            }
                        }// if schedule list count > 0
                        // runing: 启动runingList
                        if (runningTaskIds.Count > 0)
                        {
                            processWatch.Restart();
                            waitingMS = MIN_POLL_WAIT_MS; // 一旦有任务需要执行即重置轮询间隔
                            LogHelper.LogDebug("Polling list: " + _scheduledTasks.Count);
                            LogHelper.LogDebug("Running list: " + runningTaskIds.Count);
                            object removingLock = new object(); // 由于使用的是并行执行，访问removingList时需要加锁
                            var parallelLoopResult = Parallel.ForEach(runningTaskIds, (task) =>
                            {
                                if (_isPolling)
                                {
                                    // 运行结果为false时将会被移除定时列表
                                    if (!_timeUpHandle(task))
                                    {
                                        lock (removingLock)
                                            removingTaskIds.Add(task);
                                    }
                                }
                            });
                            while (!parallelLoopResult.IsCompleted) // 等待并行流水线结束
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                            LogHelper.LogDebug($"{runningTaskIds.Count} tasks complete. {processWatch.ElapsedMilliseconds / 1000F:0.00} second(s) used.");
                        } // if runing list count > 0 
                        else // 若运行列表为空则 Sleep 1-MAX_POLL_WAIT_MS s
                        {
                            waitingMS += MIN_POLL_WAIT_MS;
                            if (waitingMS > MAX_POLL_WAIT_MS)
                            {
                                waitingMS = MAX_POLL_WAIT_MS;
                            }
                            System.Threading.Thread.Sleep(waitingMS);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError("Error in Scheduler polling: " + ex.Message + ex.StackTrace);
                    }
                }
            });
            pollingTask.Start();
        }
        public void StopPolling()
        {
            _isPolling = false;
        }

        //  新增定时任务
        public void StartSchedule(TTaskKey taskKey, int intervalSeconds)
        {
            if (taskKey == null || intervalSeconds <= 0)
                return;
            LogHelper.LogDebug(taskKey + " entering.");
            if (_scheduledTasks.ContainsKey(taskKey))
            {
                if (intervalSeconds != _scheduledTasks[taskKey].IntervalMillionSeconds / 1000)
                {
                    lock (_scheduledTasksLock)
                    {
                        _scheduledTasks[taskKey] = new TaskContainer(intervalSeconds * 1000);
                    }
                }
            }
            else
            {
                TaskContainer scheduledTask = new TaskContainer(intervalSeconds * 1000);
                lock (_scheduledTasksLock)
                {
                    _scheduledTasks.Add(taskKey, scheduledTask);
                }
            }
        }

        public void StopSchedule(TTaskKey taskKey)
        {
            if (taskKey == null)
                return;
            LogHelper.LogDebug(taskKey + " stopping.");
            if (_scheduledTasks.ContainsKey(taskKey))
            {
                lock (_scheduledTasksLock)
                {
                    _scheduledTasks.Remove(taskKey);
                }
            }
        }
    }

    internal class TaskContainer
    {
        public long IntervalMillionSeconds { get; private set; }
        public long CurrentLeftMillionSeconds { get; set; }
        public TaskContainer(long intervalMillionSeconds)
        {
            IntervalMillionSeconds = CurrentLeftMillionSeconds = intervalMillionSeconds;
        }
    }

    public enum TaskScheduleStatus
    {
        Default,
        Pending,
        Started_Pending,
        StartFailed_Pending,
        StartFailed_AutoRemoved,
        Stopped
    }
}
