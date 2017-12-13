using System.Configuration;
using MongoDBDemo.Entities;
using MongoDBDemo.Query;
using System;
using System.Linq;
using MongoDBDemo.DAOs;
using System.Collections.Generic;
using System.IO;

namespace MongoDBDemo
{
    public static class Program
    {
        static void Main(params string[] agrs)
        {
            DateTime start = DateTime.Now.AddDays(-10);
            DateTime end = DateTime.Now;
            int dayBefore = -7;
            if (agrs.Count() > 0)
            {
                if (!int.TryParse(agrs[0], out dayBefore))
                {
                    Console.WriteLine(nameof(agrs) + 0 + " invalid");
                }
            }
            start = DateTime.Now.AddDays(dayBefore);

            int dayAfter = -7;
            if (agrs.Count() > 1)
            {
                if (!int.TryParse(agrs[1], out dayAfter))
                {
                    Console.WriteLine(nameof(agrs) + 0 + " invalid");
                }
            }
            end = DateTime.Now.AddDays(dayAfter);

            string filePath = "ApiUseLog_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            if (agrs.Count() > 2)
            {
                filePath = agrs[2];
            }
            var dal = new RequestLogRepository(ConfigurationManager.AppSettings["MongoAdr"], "RequestLog_DataAPI");
            var userUsedAPIs = QueryUserUsedApis(dal);
            Console.WriteLine("Total dataapi users count " + userUsedAPIs.Count());
            File.AppendAllLines($"{filePath}_dataapiUser.csv", userUsedAPIs);

            dal = new RequestLogRepository(ConfigurationManager.AppSettings["MongoAdr"], "RequestLog_AdvancedAPI");
            userUsedAPIs = QueryUserUsedApis(dal);
            Console.WriteLine("Total advancedapi users count " + userUsedAPIs.Count());
            File.AppendAllLines($"{filePath}_advancedapiUser.csv", userUsedAPIs);

        }

        private static IEnumerable<string> QueryUserUsedApis(RequestLogRepository dal)
        {
            Dictionary<string, MyHashSet<string>> userUsedAPIs = new Dictionary<string, MyHashSet<string>>();
            string[] ids = new string[] { "AllData001", "AllData002", "AllData003", "NotExportData001", null };
            foreach (var id in ids)
            {
                int total = int.MaxValue;
                int count = 0;
                int index = 0;
                int size = 30000;
                Console.Write((id ?? "NotExportData000") + " count " + userUsedAPIs.Count + "\t");

                do
                {
                    var useLogs = dal.QueryLogByApiId(id, index++, size, out total);
                    count += useLogs.Count;
                    var users = useLogs.Select(log => log.UserId).Distinct();
                    foreach (var u in users)
                    {
                        if (!userUsedAPIs.ContainsKey(u))
                        {
                            userUsedAPIs.Add(u, new MyHashSet<string>());
                        }
                    }
                    foreach (var ul in useLogs)
                    {
                        userUsedAPIs[ul.UserId].Add(id);
                    }
                    Console.Write($"{100F * count / total}% {userUsedAPIs.Count}\t");
                } while (count < total);
                Console.WriteLine();
            }
            return userUsedAPIs.Select(uua => uua.Key + "," + uua.Value.ToString());
        }

        public class MyHashSet<T> : HashSet<T>
        {
            public MyHashSet() : base() { }
            public override string ToString()
            {
                if (Count > 0)
                {
                    return string.Join(",", this);
                }
                return base.ToString();
            }
        }
    }

}