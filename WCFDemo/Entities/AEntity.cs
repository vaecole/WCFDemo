using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WCFDemo.Entities
{
    public abstract class AIPCEntity
    {
        public string Message { get; set; }
        public string Code { get; set; }
    }
}
