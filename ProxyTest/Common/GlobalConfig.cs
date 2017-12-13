using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyTest.Common
{
    public class GlobalConfigs
    {

        public static int PoolRefreshInterval
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["PoolRefreshInterval"]) * 1000;
                }
                catch (NullReferenceException)
                {
                    return 120 * 1000;
                }
            }
        }

        public static int ProxyPoolSize
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["ProxyPoolSize"]);
                }
                catch (NullReferenceException)
                {
                    return 200;
                }
            }
        }

        public static int ZhimaBalanceWarningThreshhold
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["ZhimaBalanceWarningThreshhold"]);
                }
                catch (NullReferenceException)
                {
                    return 2000;
                }
            }
        }

        public static string ZhimadailiApi
        {
            get
            {
                return ConfigurationManager.AppSettings["ZhimadailiApi"] ?? "http://webapi.http.zhimacangku.com/getip?num={0}&type=2&pro=&city=0&yys=0&port=11&time=1&ts=1&ys=0&cs=0&lb=1&sb=0&pb=45&mr=1&spec=15920029580";
            }
        }

        public static string ZhimadailiApiWhiteList
        {
            get
            {
                return ConfigurationManager.AppSettings["ZhimadailiApiWhiteList"] ?? "http://web.http.cnapi.cc/index/index/save_white?neek=1176&appkey=273900ea00243a3e35267d59b3b61274&spec=15920029580&white={0}";
            }
        }

        public static string ZhimadailiApiBalance
        {
            get
            {
                return ConfigurationManager.AppSettings["ZhimadailiApiBalance"] ?? "http://web.http.cnapi.cc/index/index/get_my_balance?neek=1176&appkey=273900ea00243a3e35267d59b3b61274&spec=15920029580";
            }
        }

        public static string InterIpAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["InterIpAddress"] ?? "183.14.134.143";
            }
        }
    }

}
