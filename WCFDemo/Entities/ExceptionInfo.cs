using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFDemo.Entities
{
    public class ExceptionInfo
    {
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public string IP { get; set; }
        public string Module { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

}
