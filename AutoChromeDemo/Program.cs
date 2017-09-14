using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoChromeDemo
{
    static class Program
    {
        static void Main(string[] args)
        {
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://skieer:skieer@elk.skieer.com");
            // driver.Navigate().GoToUrl("http://elk.skieer.com/app/kibana#/discover?_g=(refreshInterval:(display:Off,pause:!f,value:0),time:(from:now-14d,mode:quick,to:now))&_a=(columns:!(_source),index:'bzy_nginx-*',interval:d,query:(query_string:(analyze_wildcard:!t,query:'url:{0}')),sort:!('@timestamp',desc),uiState:(spy:(mode:(fill:!f,name:table)),vis:(legendOpen:!f)))");
            string metaUrl = "http://elk.skieer.com/app/kibana#/discover?_g=(refreshInterval:(display:Off,pause:!f,value:0),time:(from:now-14d,mode:quick,to:now))&_a=(columns:!(_source),index:'bzy_nginx-*',interval:d,query:(query_string:(analyze_wildcard:!t,query:'url:{0}')),sort:!('@timestamp',desc),uiState:(spy:(mode:(fill:!f,name:table)),vis:(legendOpen:!f)))";
            string[] urlKeys = new string[] {
                "token",
                "\"/api/alldata?\"",
                "\"/api/alldata/getdataoftaskbytimeandpaging?\"",
                "\"/api/alldata/getlastdataoftaskbypaging?\"",
                "\"/api/notexportdata?\"",
                "\"/api/notexportdata/gettop?\"",
                "\"/api/notexportdata/update?\"",
                "\"/api/task/RemoveDataByTaskId?\"",
                "\"/api/alldata/getdataoftaskbyoffset?\"",
                "\"/api/taskgroup\"",
                "\"/api/task?\"",
            };

            foreach (var urlKey in urlKeys)
            {
                driver.Navigate().GoToUrl(string.Format(metaUrl, urlKey));
                driver.Navigate().Refresh();
                IWebElement downloadLink = null;
                bool elementExist = false;
                while (!elementExist)
                {
                    try
                    {
                        downloadLink = driver.FindElement(By.CssSelector(".agg-table-controls > a:nth-child(3)"));
                        elementExist = true;
                    }
                    catch (Exception)
                    {
                        elementExist = false;
                        System.Threading.Thread.Sleep(500);
                    }
                }
                System.Threading.Thread.Sleep(5000);
                downloadLink.Click();
                DirectoryInfo dirP = new DirectoryInfo(@"C:\Users\Administrator\Downloads\ApiUseLog");
                if (!dirP.Exists)
                    dirP.Create();
                while (!File.Exists(@"C:\Users\Administrator\Downloads\New Saved Search.csv"))
                {
                    System.Threading.Thread.Sleep(500);
                }
                // FileInfo file = new FileInfo(@"C:\Users\marko\Downloads\New Saved Search.csv");// (@"C:\Users\marko\Downloads\"+ urlKey.Replace('\"',' ').Replace('/','-').TrimStart('-'));
                File.Move(@"C:\Users\Administrator\Downloads\New Saved Search.csv", Path.Combine(dirP.FullName, urlKey.Replace('\"', '\0').Replace('/', '-').Replace('?', '\0').TrimStart('-').TrimEnd('-') + ".csv"));
            }

        }
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }
    }
}
