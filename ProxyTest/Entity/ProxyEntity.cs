using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyTest.Entity
{
    public class HttpProxyEntity
    {
        public HttpProxyEntity()
        {
            Ip = string.Empty;
            Source = string.Empty;
            ExpirationTime = DateTime.MaxValue;
            StartUsingTime = DateTime.Now;
        }

        /// <summary>
        /// 唯一id，mongodb生成
        /// </summary>
        public string ProxyId { get; set; }

        /// <summary>
        /// 代理的Ip地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 代理的端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 代理的有效时间
        /// 代理这个代理在获取到这个代理的时刻起，多久后是可用的，过了这个时间，代理会失效
        /// </summary>
        public DateTime ExpirationTime { get; set; }

        public string Source { get; set; }

        public override string ToString()
        {
            return Ip + ":" + Port.ToString();
        }

        public int UserBalance { get; set; }

        public readonly DateTime StartUsingTime;
        public RequestProxyStatus Status { get; set; }
    }

    [Flags]
    public enum RequestProxyStatus
    {
        OK,
        Using,
        UserLimitationReached,
        TimeLimitationReached,
        NoNumberBalance,
        UserNotAllowed = 5,
        ServiceUnavailable = 503,
    }
}
