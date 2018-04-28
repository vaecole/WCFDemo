using System;
using System.Collections.Generic;
using System.Text;

namespace MQ.Models.Messages
{
    public enum TaskMsgType
    {
        TaskStarted,
        TaskStopped,
        SplitStarted,
        SplitCompleted,
        TaskCompleted,
    }
}
