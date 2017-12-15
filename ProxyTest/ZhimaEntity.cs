using MWUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyTest
{
    class ZhimaResultEntity<DataType>
    {
        public int code { get; set; }
        public bool success { get; set; }
        public string msg { get; set; }
        public DataType data { get; set; }
    }

    public class ZhimaProxyInfo
    {
        /// <summary>
        /// 代理IP
        /// </summary>
        [Newtonsoft.Json.JsonProperty("ip")]
        public string Ip
        {
            get;
            set;
        }

        /// <summary>
        /// 端口
        /// </summary>
        [Newtonsoft.Json.JsonProperty("port")]
        public int Port
        {
            get;
            set;
        }
        [Newtonsoft.Json.JsonProperty("expire_time")]
        public string expire_time
        {
            set
            {
                try
                {
                    ExpirationTime = DateTime.Parse(value);
                    DistributionTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex);
                    ExpirationTime = DateTime.Now;
                }
            }
        }

        public string city;
        public string isp;

        /// <summary>
        /// 代理IP的有效时间
        /// </summary>
        public DateTime ExpirationTime
        {
            get;
            set;
        }

        public DateTime DistributionTime
        {
            get;
            set;
        }

        public double Duration
        {
            get
            {
                return (ExpirationTime - DateTime.Now).TotalMilliseconds;
            }
        }

        public bool Enable
        {
            get
            {
                //有效时间小于1分钟的代理是无效的
                return Duration > 1000 * 60 * 1;
            }
        }

        private int InvalidReportTimesMax
        {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["ProxyInvalidReportTimesMax"]);
            }
        }
        public override string ToString()
        {
            return Ip + ":" + Port;
        }

        public string Source
        {
            get;
            set;
        }
    }

}
