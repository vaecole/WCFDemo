using MongoDBDemo.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace MongoDBDemo
{
    public static class Program
    {
        static void Main(params string[] agrs)
        {
            DateTime start = DateTime.Now.AddDays(-10);
            DateTime end = DateTime.Now;
            int dayBefore = -2;
            if (agrs.Length > 0)
            {
                if (!int.TryParse(agrs[0], out dayBefore))
                {
                    Console.WriteLine(nameof(agrs) + 0 + " invalid");
                }
            }
            start = DateTime.Now.AddDays(dayBefore);

            string[] collectionNames = (ConfigurationManager.AppSettings["CollectionNames"]
                ?? "RequestLog_DataAPI,RequestLog_AdvancedAPI").Split(',');

            foreach (var item in collectionNames)
            {
                var dal = new RequestLogRepo(ConfigurationManager.AppSettings["MongoAdr"],"ApiLogs", item);
                QueryUsers2File(dal, start, item);
            }
        }

        private static int QueryUsers2File(RequestLogRepo dal, DateTime startDate, string fileName)
        {
            Dictionary<string, MyHashSet<string>> userUsedAPIs = new Dictionary<string, MyHashSet<string>>();
            string[] ids = (ConfigurationManager.AppSettings["ApiIds"] ??
                "AllData004,NotExportData002,NotExportData003,NotExportData004,TaskGroup001,Task001,Task003,Task004,Task005,Task006,Task007,Task008,Task002")
                .Split(',');
            Console.WriteLine("Quering logs after " + startDate);
            foreach (var id in ids)
            {
                int count = 0;
                int index = 0;
                int size = 10000;
                Console.Write((id + " count " + userUsedAPIs.Count + "\t"));
                int currentCount = 0;
                do
                {
                    var useLogs = dal.QueryLastestByApiId(id, index++, size, startDate).Result;
                    if (useLogs?.Count > 0)
                    {
                        count += (currentCount = useLogs.Count);
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
                        Console.Write($"Up to {useLogs.Last().CreatedDate.ToLocalTime()}, {userUsedAPIs.Count} uses found.\t");
                    }
                    else
                    {
                        break;
                    }
                } while (currentCount < size);
                Console.WriteLine();
            }
            var content2File = userUsedAPIs.Select(uua => uua.Key + "," + uua.Value.ToString());
            var userCount = content2File.Count();
            Console.WriteLine(fileName + " users count " + userCount);
            File.AppendAllLines($"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmm")}.csv", content2File);
            return userCount;
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