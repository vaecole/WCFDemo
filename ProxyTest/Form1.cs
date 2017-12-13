using MWUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ProxyTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.Text = string.Empty;
            proxyTests = new List<ProxyTester>();
            bt_exportResult.Visible = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                testThread?.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误");
            }
            finally
            {
                base.OnClosed(e);
            }
        }

        private List<ProxyTester> proxyTests;
        private Thread testThread;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string testUrl = tb_url2conn.Text;
                string[] statusUrls = tb_urls_2status.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                proxyTests?.Clear();
                testThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        ZhimaProxyProvider proxyProvider = new ZhimaProxyProvider();
                        int num2Test = int.Parse(tb_num2Test.Text);
                        int max = 100;
                        int batchNum = num2Test / max + ((num2Test % max) > 0 ? 1 : 0);
                        int count = 0;
                        for (int i = 0; i < batchNum; i++)
                        {
                            int currentNum = Math.Min(max, num2Test - max * i);
                            var proxies = proxyProvider.GetProxies(currentNum);
                            Invoke((Action)(() =>
                            {
                                tb_proxyList.Text = string.Join("\r\n", proxies.Select(p => p.ToString()));
                            }));
                            if (string.IsNullOrWhiteSpace(testUrl) || proxies == null || proxies.Count() == 0)
                            {
                                return;
                            }
                            Invoke((Action)(() =>
                            {
                                lb_resultOutput.Items.Clear();
                                bt_start.Enabled = false;
                            }));
                            foreach (var item in proxies)
                            {
                                count++;
                                Invoke((Action)(() =>
                                {
                                    label1.Text = $"Current/Total: {count}/{num2Test}, Checking: " + item;
                                }));
                                ProxyTester connTester = new ProxyTester(item.Ip, item.Port, testUrl);
                                double timeSeconds = double.Parse(tb_ConnTime.Text);
                                Invoke((Action)(() =>
                                {
                                    lb_resultOutput.Items.Add($"连通性：{item} ");
                                }));
                                var duration = connTester.TestConnection(timeSeconds);
                                Invoke((Action)(() =>
                                {
                                    lb_resultOutput.Items.Add($"耗时{duration}秒 {connTester.ExceptionString}");
                                }));
                                proxyTests.Add(connTester);
                                if (duration >= timeSeconds)
                                {
                                    foreach (var url in statusUrls)
                                    {
                                        ProxyTester statusTester = new ProxyTester(item.Ip, item.Port, url);
                                        Invoke((Action)(() =>
                                        {
                                            lb_resultOutput.Items.Add($"连通性：{item} ");
                                        }));
                                        var statuscode = statusTester.TestStatusCode();
                                        Invoke((Action)(() =>
                                        {
                                            lb_resultOutput.Items.Add($"结果{statuscode} ");
                                        }));
                                        proxyTests.Add(statusTester);
                                    }
                                }
                            }
                            if (proxyTests?.Count > 0)
                            {
                                Invoke((Action)(() =>
                                {
                                    label1.Text = $"正在导出...";
                                }));
                                CsvHelper.Convert2Csv(proxyTests);
                                Invoke((Action)(() =>
                                {
                                    label1.Text = $"完成";
                                }));
                            }
                        }
                        bt_exportResult.Enabled = proxyTests?.Count > 0;
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is InvalidOperationException))
                            Invoke((Action)(() =>
                            {
                                MessageBox.Show(ex.ToString(), "错误");
                            }));
                    }
                    finally
                    {
                        Invoke((Action)(() =>
                        {
                            bt_start.Enabled = true;
                        }));
                    }
                }));
                testThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // tb_proxyList.Text = string.Join("\r\n", proxyTests.Where(pt => pt.OKsTime > 1).Select(pt => pt.Proxy));
            bt_exportResult.Enabled = false;
        }
    }

    public class ProxyTester
    {
        public static HttpStatusCode HttpGet(string url, IWebProxy proxy = null, int timeoutSeconds = 6)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = timeoutSeconds * 1000;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode;
            }
        }

        public ProxyTester(string host, int port, string testUrl)
        {
            if (string.IsNullOrWhiteSpace(host) || port < 0 || port > 65535)
            {
                throw new ArgumentException("Proxy format invalid!");
            }
            if (string.IsNullOrWhiteSpace(testUrl))
            {
                throw new ArgumentException("No testUrl!");
            }
            Host = host;
            Port = port;
            TestUrl = testUrl;
            OKsTime = TestsTime = 0;
        }
        private string Host;
        private int Port;
        private Exception Exception;
        public string TestUrl { private set; get; }
        public string Proxy { get => $"{Host}:{Port}"; }
        public double DurationSeconds { private set; get; }
        public int StatusCode { private set; get; }
        public bool TimeOuted { private set; get; }
        public int OKsTime { private set; get; }
        public int TestsTime { private set; get; }
        public string ExceptionString { get => Exception?.Message?.ToString(); }
        public double TestConnection(double timeoutSeconds)
        {
            IWebProxy proxy = new WebProxy(Host, Port);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (watch.Elapsed.TotalSeconds < timeoutSeconds)
            {
                try
                {
                    TestsTime++;
                    StatusCode = -1;
                    StatusCode = (int)HttpGet(TestUrl, proxy, (int)timeoutSeconds);
                    Thread.Sleep(500);
                }
                catch (TimeoutException ex)
                {
                    TimeOuted = true;
                    Exception = ex;
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException is TimeoutException || ex.Message.Contains("timed out"))
                    {
                        TimeOuted = true;
                    }
                    Exception = ex;
                    break;
                }
                if (StatusCode == 200)
                {
                    OKsTime++;
                }
            }
            DurationSeconds = watch.Elapsed.TotalSeconds;
            watch.Stop();
            return DurationSeconds;
        }

        public int TestStatusCode(double timeoutSeconds = 6)
        {
            IWebProxy proxy = new WebProxy(Host, Port);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                TestsTime++;
                StatusCode = -1;
                StatusCode = (int)HttpGet(TestUrl, proxy, (int)timeoutSeconds);
            }
            catch (TimeoutException ex)
            {
                TimeOuted = true;
                Exception = ex;
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                if (res != null)
                {
                    StatusCode = (int)res.StatusCode;
                }
                if (ex.InnerException is TimeoutException || ex.Message.Contains("timed out"))
                {
                    TimeOuted = true;
                }
                Exception = ex;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is TimeoutException || ex.Message.Contains("timed out"))
                {
                    TimeOuted = true;
                }
                Exception = ex;
            }
            if (StatusCode == 200)
            {
                OKsTime++;
            }
            DurationSeconds = watch.Elapsed.TotalSeconds;
            watch.Stop();
            return StatusCode;
        }
    }
}
