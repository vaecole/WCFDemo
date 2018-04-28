using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MWUtility.Net
{
    public static class HttpClientFactory
    {
        private static readonly ConcurrentDictionary<string, HttpClient> _httpClientCache = new ConcurrentDictionary<string, HttpClient>();

        public static HttpClient Create(string baseAddress, IWebProxy proxy = null)
        {
            return _httpClientCache.GetOrAdd(baseAddress, ba =>
            {
                HttpClient client;
                if (proxy != null)
                {
                    HttpMessageHandler handler = new HttpClientHandler()
                    {
                        Proxy = proxy,
                    };
                    client = new HttpClient(handler);
                }
                else
                {
                    client = new HttpClient();
                }
                client.BaseAddress = new Uri(ba);
                client.DefaultRequestHeaders.Connection.Add("keep-alive");
                return client;
            });
        }

        public static HttpStatusCode HttpGet(string url, IWebProxy proxy = null, int timeOutSeconds = 4)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = timeOutSeconds * 1000;
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode;
            }
        }

        public static string Get(string url, params object[] urlParams)
        {
            var httpclient = Create(string.Format(url, urlParams));
            var result = httpclient.GetAsync(string.Empty).Result;
            // result.EnsureSuccessStatusCode();
            if(!result.IsSuccessStatusCode)
            {
                throw new Exception(result.StatusCode.ToString());
            }
            return result.Content.ReadAsStringAsync().Result;
        }
    }

}
