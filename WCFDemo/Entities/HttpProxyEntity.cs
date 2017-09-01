using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octopus.API.SDK.Entites
{
    public class HttpProxyEntity
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public DateTime ExpirationTime { get; set; }

        public string ProxyId { get; set; }

        public bool WillExpireIn(int milliSeconds = 60 * 1000)
        {
            return ExpirationTime - DateTime.Now <= TimeSpan.FromMilliseconds(milliSeconds);
        }

        public double ValidMilliSeconds
        {
            get
            {
                return (ExpirationTime - DateTime.Now).TotalMilliseconds;
            }
        }

        public override string ToString()
        {
            return IP + ":" + Port.ToString();
        }
    }
    public enum HttpProxyMode
    {
        None = 0,
        Manual = 1,
        Auto = 2
    }

    public enum HttpProxyStartStatus
    {
        OK,
        NetworkDown,
        UserNotAllowed,
        ProxyServiceInvalid,
        InvalidConfig,
        Unknown
    }
}
