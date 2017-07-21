using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFDemo.NetworkUtility
{

    public class HttpResult<TData>
    {
        public TData Data { get; set; }

    }
    public class HttpResult
    {
        public string Message { get; set; }
        public string StatusCode { get; set; }

    }
}
