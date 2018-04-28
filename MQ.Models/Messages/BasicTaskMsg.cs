using System;
using System.Collections.Generic;
using System.Text;

namespace MQ.Models.Messages
{
    public abstract class BasicTaskMsg
    {
        public BasicTaskMsg(string taskId)
        {
            TaskId = taskId;
        }

        public string TaskId { get; set; }

        public abstract TaskMsgType MsgType { get; }
    }
}
