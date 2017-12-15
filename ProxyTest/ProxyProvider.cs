using MWUtility;
using MWUtility.Net;
using Newtonsoft.Json;
using ProxyTest.Common;
using ProxyTest.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyTest
{
    class ZhimaProxyProvider
    {
        private string cacheKey = "whiteIpList_zhima";
        public ZhimaProxyProvider()
        {
            whiteIpList_zhima = WMedis.Instance.Peek<List<string>>(cacheKey);
            if (whiteIpList_zhima == null)
            {
                whiteIpList_zhima = new List<string>();
                lastCount_whiteIpList_zhima = whiteIpList_zhima.Count;
            }
            EnsureSelfInWhiteList();
            MonitorBalance();
        }

        public List<HttpProxyEntity> GetProxies(int num)
        {
            List<HttpProxyEntity> res = new List<HttpProxyEntity>();
            var innerRes = GetProxies(num, true);
            if (innerRes != null)
            {
                foreach (var item in innerRes)
                {
                    res.Add(new HttpProxyEntity()
                    {
                        ExpirationTime = item.ExpirationTime,
                        Ip = item.Ip,
                        Port = item.Port,
                        Source = GetType().Name
                    });
                }
            }
            return res;
        }

        public HttpProxyEntity GetOneProxy()
        {
            return GetProxies(1)?[0];
        }

        private List<string> whiteIpList_zhima;
        private int lastCount_whiteIpList_zhima;
        public bool AddIp2WhiteList(string ip)
        {
            try
            {
                if (whiteIpList_zhima.Contains(ip))
                {
                    return true;
                }
                LogHelper.Info($"AddIp2WhiteList: " + ip);
                var res = JsonConvert.DeserializeObject<ZhimaResultEntity<object>>(Get(GlobalConfigs.ZhimadailiApiWhiteList, ip));
                if (res.success)
                {
                    whiteIpList_zhima.Add(ip);
                    WMedis.Instance.Cache(cacheKey, whiteIpList_zhima,
                        (whiteIpList_zhima.Count - lastCount_whiteIpList_zhima) > 10);// 每新增10个，立即持久化一次
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            return false;
        }

        #region Private Methods

        private List<ZhimaProxyInfo> GetProxies(int num, bool retryIfFailed = true)
        {
            if (num <= 0)
            {
                return null;
            }
            var result = Get(GlobalConfigs.ZhimadailiApi, num);
            if (string.IsNullOrWhiteSpace(result))
            {
                return null;
            }
            try
            {
                var proxies = JsonConvert.DeserializeObject<ZhimaResultEntity<IEnumerable<ZhimaProxyInfo>>>(result)?.data?.ToList();
                if (proxies?.Count > 0)
                {
                    return proxies;
                }
                else
                {
                    var entity = JsonConvert.DeserializeObject<ZhimaResultEntity<object>>(result);
                    LogHelper.Error(GetType().Name + entity.msg);
                    if (entity.msg != null)
                    {
                        EnsureSelfInWhiteList(entity);
                        return GetProxies(num, false);
                    }
                }
            }
            catch (Exception ex) { LogHelper.Error(ex); }
            return null;
        }


        private bool stopMonitorBalance = false;
        private Task monitorBalanceTask;
        private void MonitorBalance()
        {
            monitorBalanceTask = new Task(() =>
            {
                Thread.Sleep(4 * 1000); // 这里有3秒的访问限制
                while (!stopMonitorBalance)
                {
                    string result = string.Empty;
                    try
                    {
                        result = Get(GlobalConfigs.ZhimadailiApiBalance);
                        LogHelper.Info(result);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                    Thread.Sleep(24 * 60 * 60 * 1000); // 一天检查一次
                }
            });
            monitorBalanceTask.Start();
        }

        private void EnsureSelfInWhiteList(ZhimaResultEntity<object> entity)
        {
            if (entity.msg.Contains("白名单"))
            {
                string ip = string.Empty;
                StringBuilder sb = new StringBuilder();
                int iplength = 0;
                string self_ip = string.Empty;
                for (int i = 0; i < entity.msg.Length; i++)
                {
                    if (entity.msg[i] >= '0' && entity.msg[i] <= '9' || entity.msg[i] == '.')
                    {
                        iplength++;
                        sb.Append(entity.msg[i]);
                    }
                    else
                    {
                        if (iplength >= 7)
                        {
                            self_ip = sb.ToString();
                            break;
                        }
                        else
                        {
                            sb.Clear();
                            iplength = 0;
                        }
                    }
                }
                if (self_ip.Length >= 7)
                {
                    AddIp2WhiteList(self_ip);
                }
            }
        }

        private void EnsureSelfInWhiteList()
        {
            try
            {
                AddIp2WhiteList(GlobalConfigs.InterIpAddress);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        private static object _lock = new object();
        private string Get(string url, params object[] urlParams)
        {
            try
            {
                var httpclient = HttpClientFactory.Create(string.Format(url, urlParams));
                var result = httpclient.GetAsync(string.Empty).Result;
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsStringAsync().Result;
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    LogHelper.Error(ex.InnerException);
                }
            }
            return string.Empty;
        }


        #endregion

    }

}
