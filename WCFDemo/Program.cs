using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
using System.Threading;
using System.Security.Cryptography;
using Octopus.Entity;
using WCFDemo.Entities;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace WCFDemo
{
    class Program
    {
        static void Main(params string[] paras)
        {
            Octopus.Utility.LogHelper.LogDebug("Debug test info.");
            Octopus.Utility.LogHelper.LogError("Error test info.");
            Octopus.Utility.LogHelper.LogInfo("Info test info.");
        }
    }

    internal class Filter
    {
        private const string FilterFileName = "userfilter.ini";
        public static string AppStartPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static bool IsFilteredUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }
            var filterFileFullPath = Path.Combine(AppStartPath, FilterFileName);
            if (!File.Exists(filterFileFullPath))
            {
                return false;
            }

            List<string> betaUsers = new List<string>();
            using (var fr = File.OpenText(filterFileFullPath))
            {
                while (fr.Peek() > 0)
                {
                    var line = fr.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                    {
                        if (line == "all_open") // 对所有用户开放
                        {
                            return true;
                        }
                        if (line == "all_close")// 对所有用户关闭
                        {
                            return false;
                        }
                        betaUsers.Add(line);
                    }
                }
            }
            return betaUsers.Exists(user => user.Equals(userId, StringComparison.OrdinalIgnoreCase));
        }
    }


}
