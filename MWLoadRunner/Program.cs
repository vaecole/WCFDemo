using MWUtility;
using MWUtility.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MWLoadRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string urlTplt = "http://localhost:6008/Json/ProxyService/getProxy/{0}/{1}/{2}";
            string userPrefix = "marko";
            string taskPrefix = "proxyTestTask";
            string ipPrefix = "20.13.14.";
            for (int i = 1; i < 1000; i++)
            {
                int randomUser = new Random().Next(1, 1000);
                var user = userPrefix + randomUser;
                var task = taskPrefix + randomUser;
                var ip = ipPrefix + (randomUser % 255);
                var url = string.Format(urlTplt, user, task, ip);
                Thread.Sleep(new Random().Next(1, 2000));
                Run(url);
            }
        }

        private static void Run(string url)
        {
            try
            {
                new Task(() =>
                {
                    LogHelper.TempDebug(url + " " + HttpClientFactory.Get(url));
                }).Start();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }
    }
}
