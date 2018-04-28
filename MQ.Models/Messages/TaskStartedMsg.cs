using System;
using System.Collections.Generic;
using System.Text;

namespace MQ.Models.Messages
{
    public class TaskStartedMsg : BasicTaskMsg
    {
        public TaskStartedMsg(string taskId) : base(taskId)
        {
        }

        public override TaskMsgType MsgType { get { return TaskMsgType.TaskStarted; } }
    }
}
