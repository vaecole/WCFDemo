using Octopus.API.SDK.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using WCFDemo;

namespace Octopus.TaskReviewer.Proxy
{
    /// <summary>
    /// 每个IP平台承诺为15000秒，大概计算，按照如下策略检查和更换：
    /// # 启动时检查一次
    /// # 过600秒检查一次
    /// # 过600-12*1 秒检查一次
    /// # 过600-12*2 秒检查一次
    /// # ...
    /// # 过600-12*49秒检查一次
    /// # IP过期，上报此IP作废并更换新的IP，从新检查
    /// # 检查如果IP不可用，立即上报此IP作废并更换新IP，从新检查
    /// </summary>
    public class AutoProxyManager
    {
        private HttpProxyEntity _currentProxy;
        public string CurrentProxy
        {
            get
            {
                if (_currentProxy != null)
                {
                    return _currentProxy.IP.ToMosaicString(50) + ":" + _currentProxy.Port.ToString().ToMosaicString(50);
                }
                return null;
            }
        }

        private System.Threading.Timer _proxyMoniter;

        private static AutoProxyManager _instance = new AutoProxyManager();
        public static AutoProxyManager Instance { get { return _instance; } }

        public HttpProxyEntity CurrentProxyEntity
        {
            get
            {
                return _currentProxy;
            }
        }

        private static bool _whiteListAdded = false;
        public HttpProxyStartStatus StartProxing()
        {
            if (!_whiteListAdded)
            {
                AddClientIp2WhiteList();
                _whiteListAdded = true;
            }
            if (UseCurrentProxy())
            {
                return HttpProxyStartStatus.OK;
            }
            badProxyRetry = 0;// 每次启动的时候重置重试限制
            return UseNewProxy();
        }

        public void Stop()
        {
            if (_proxyMoniter != null)
            {
                _proxyMoniter.Dispose();
                _proxyMoniter = null;
            }
        }

        private int BadProxyRetryMax = 2;
        private int badProxyRetry = 0;
        private HttpProxyStartStatus UseNewProxy()
        {
            if (IsConnectInternet())
            {
                try
                {
                    while (badProxyRetry < BadProxyRetryMax)
                    {
                        RecycleCurrentProxy(); // 先回收
                        var serverRes = API.SDK.Extend.HttpProxy.Instance.GetProxy(TokenManager.CachedToken, GlobalCache.CurrentTaskId);
                        if (serverRes.IsSuccess)
                        {
                            _currentProxy = serverRes.Data;
                            _currentProxy.IP = Utility.EncryptHelper.Decrypt(_currentProxy.IP);
                            Output("AutoProxyManager: UseNewProxy " + _currentProxy.ToString());
                            if (UseCurrentProxy())
                            {
                                return HttpProxyStartStatus.OK;
                            }
                        }
                        else
                        {
                            Output($"{serverRes.Error}, {serverRes.Error}，重试{badProxyRetry + 1}/{BadProxyRetryMax + 1}...", true);
                        }
                        badProxyRetry++;
                    }
                }
                catch (Exception ex)
                {
                    Log.Default.Error(ex);
                }
                return HttpProxyStartStatus.ProxyServiceInvalid;
            }
            else { return HttpProxyStartStatus.NetworkDown; }
        }

        private bool UseCurrentProxy()
        {
            if (CheckIfHealthy() == HealthStatus.Healthy)
            {
                Output("AutoProxyManager: UseCurrentProxy " + _currentProxy.ToString());
                GlobalCache.MainForm.Invoke(() =>
                {
                    GeckoSettings.HttpProxy = _currentProxy.IP;
                    GeckoSettings.HttpProxyPort = _currentProxy.Port;
                });
                StartMonitoring();
                HideOutput();
                return true;
            }
            return false;
        }

        private void StartMonitoring()
        {
            // 15000/25 = 600
            CurrentHealthCheckInterval = (int)_currentProxy.ValidMilliSeconds / 25;
            // 600/50 = 12
            HealthCheckIntervalMSecondsStepLength = CurrentHealthCheckInterval / 50;
            Output("AutoProxyManager: StartMonitoring dueTime " + CurrentHealthCheckInterval.ToString());
            if (_proxyMoniter == null)
            {
                _proxyMoniter = new System.Threading.Timer(callback => _proxyMoniter_Elapsed(), null, CurrentHealthCheckInterval, Timeout.Infinite);
            }
            else
            {
                _proxyMoniter.Change(CurrentHealthCheckInterval, Timeout.Infinite);
            }
        }

        private const int HealthCheckIntervalMSecondsMin = 60 * 1000;
        private int HealthCheckIntervalMSecondsStepLength = 12 * 1000;
        private int CurrentHealthCheckInterval = 600 * 1000;
        private void _proxyMoniter_Elapsed()
        {
            if (_currentProxy.WillExpireIn(CurrentHealthCheckInterval)) // 如果在下个检查时间片过期，则提前换新
            {
                Output("AutoProxyManager: Moniter Check expired");
                UseNewProxy();
            }
            else
            {
                Output($"定期检查代理IP...", true);
                var result = CheckIfHealthy();
                if (result == HealthStatus.Healthy) // 健康检查
                {
                    var dueTime = Math.Max(HealthCheckIntervalMSecondsMin, CurrentHealthCheckInterval -= HealthCheckIntervalMSecondsStepLength);
                    Output("AutoProxyManager: Moniter Check, proxy is Healthy, next dueTime " + dueTime.ToString());
                    _proxyMoniter.Change(dueTime, Timeout.Infinite);
                }
                else
                {
                    // TODO: 其他情况也要处理
                    Output($"AutoProxyManager: Moniter Check Healthy result {result}");
                    UseNewProxy();
                }
            }
            HideOutput();
        }

        private HealthStatus CheckIfHealthy()
        {
            if (_currentProxy == null || _currentProxy?.WillExpireIn() == true)
            {
                return HealthStatus.ProxyNotReady;
            }
            if (!TokenManager.IsConnectInternet())
            {
                Output($"网络异常", true);
                return HealthStatus.NetworkNotHealthy;
            }
            // 先检查代理，不行就检查是否是网络问题，如果不是则加白名单（仅加一次），加了之后再从新测一遍
            Output("AutoProxyManager: CheckIfHealthy " + _currentProxy.ToString());
            string[] urls = new string[3] { "http://www.baidu.com", "http://www.qq.com", "http://www.163.com" };
            IWebProxy proxy = new WebProxy(_currentProxy.IP, _currentProxy.Port);
            foreach (var url in urls)
            {
                try
                {
                    if (HttpGet(url, proxy) == HttpStatusCode.OK)
                    {
                        return HealthStatus.Healthy;
                    }
                }
                catch (WebException ex)
                {
                    Output("AutoProxyManager: CheckIfHealthy get with proxy " + url + ex.Message + ex.InnerException?.Message);
                }
            }

            bool canNormallyHttpGet = false;
            foreach (var url in urls)
            {
                try
                {
                    if (HttpGet(url) == HttpStatusCode.OK)
                    {
                        canNormallyHttpGet = true;
                        break;
                    }
                }
                catch (WebException ex)
                {
                    Output("AutoProxyManager: CheckIfHealthy no proxy get " + url + ex.Message + ex.InnerException?.Message);
                    canNormallyHttpGet = false;
                }
            }
            if (!canNormallyHttpGet)
            {
                Output($"Http服务异常", true);
                return HealthStatus.HttpServiceNotHealthy;
            }
            Output($"当前代理IP {_currentProxy.ToString().ToMosaicString()}似乎不起作用", true);
            return HealthStatus.ProxyNotHealthy;
        }

        enum HealthStatus
        {
            ProxyNotReady,
            Healthy,
            NetworkNotHealthy,
            HttpServiceNotHealthy,
            ProxyNotHealthy
        }

        private void RecycleCurrentProxy()
        {
            if (_currentProxy == null)
            {
                return;
            }
            HttpProxyEntity currentProxy = _currentProxy;
            new Async(() =>
            {
                API.SDK.Extend.HttpProxy.Instance.RecycleProxy(Utility.EncryptHelper.Encrypt(currentProxy.ProxyId));
                Output("AutoProxyManager: RecycleCurrentProxy " + currentProxy.ToString());
            }).Start();
        }

        private HttpStatusCode HttpGet(string url, IWebProxy proxy = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 8 * 1000;
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode;
            }
        }

        private void Output(string msg, bool show2User = false)
        {
            Log.Default.Debug(msg);
        }

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(int Description, int ReservedValue);

        /// <summary>
        /// 用于检查网络是否可以连接互联网,true表示连接成功,false表示连接失败 
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectInternet()
        {
            int Description = 0;
            return InternetGetConnectedState(Description, 0);
        }

        private const string _whiteListURL = "http://http-web.zhimaruanjian.com/index/index/save_white?neek=1176&appkey=273900ea00243a3e35267d59b3b61274&white=";
        private static string _clientIp;
        private void AddClientIp2WhiteList()
        {
            _clientIp = API.SDK.Extend.HttpProxy.Instance.GetClientIP();
            Log.Default.Debug(Utility.HttpHelper.GetHtml(_whiteListURL + _clientIp));
        }


    }

    public class HttpProxy
    {
        private static HttpProxy _instance = new HttpProxy();
        public static HttpProxy Instance { get { return _instance; } }
        public ResultModel<HttpProxyEntity> GetProxy(TokenData token, string taskId)
        {
            var url = Constants.BaseUrl + "/api/HttpProxy?taskId=" + taskId;
            return HttpHelper.Get<ResultModel<Entites.HttpProxyEntity>>(url, token);
        }

        public void RecycleProxy(string proxyId)
        {
            var url = Constants.BaseUrl + "/api/HttpProxy/Recycle?proxyId=" + proxyId;
            HttpHelper.Delete<ErrorMessage>(url, TokenManager.CachedToken);
        }

        public string GetClientIP()
        {
            var url = Constants.BaseUrl + "/api/HttpProxy/ClientIp";
            return HttpHelper.Get(url, TokenManager.CachedToken);
        }
    }

    public class ResultModel<TResult> : ErrorMessage
    {
        public TResult Data { get; set; }
    }
}
