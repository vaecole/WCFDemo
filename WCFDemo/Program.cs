using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using System.Net.Http;
using RestSharp;
using System.Threading.Tasks;

namespace WCFDemo
{

    public class ThreadWork
    {
        public static void DoWork()
        {
            try
            {
                //int count = 0;
                //for (int i = 0; i < 100000; i++)
                //{
                //    count++;
                //    //Console.WriteLine("Thread - working.");
                //    Thread.Sleep(100);
                //}

                while (true)
                {
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("Thread - caught ThreadAbortException - resetting.");
                Console.WriteLine("Exception message: {0}", e.Message);
                Thread.ResetAbort();
            }
            Console.WriteLine("Thread - still alive and working.");
            Thread.Sleep(1000);
            Console.WriteLine("Thread - finished working.");
        }
    }

    class Program
    {
        static void Main(params string[] paras)
        {
            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            ThreadStart myThreadDelegate = new ThreadStart(ThreadWork.DoWork);
            Thread myThread = new Thread(myThreadDelegate);
            myThread.Start();
            Thread.Sleep(1000);
            Console.WriteLine("Main - aborting my thread.");
            myThread.Abort();

            //myThread.Join();
            Console.Read();
            return;
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether 
        // or not they wish to abort execution. 
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 过滤线程中止的异常
            if (e == null || e.ExceptionObject is ThreadAbortException)
            {
                return;
            }

            Exception outException = e.ExceptionObject as Exception;
            var exception = outException;
            if (outException?.InnerException != null)
            {
                exception = outException?.InnerException;
            }
            if (exception != null)
            {
                Console.WriteLine(exception);
            }
        }


    }

}
