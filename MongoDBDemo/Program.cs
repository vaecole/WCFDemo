using System.Configuration;
using MongoDBDemo.Entities;
using MongoDBDemo.Query;
using MongoDBDemo.TextFileHelper;
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

            HashSet<string> hs = new HashSet<string>();

            string[] ids = new string[] { "AllData001", "AllData002", "AllData003", "NotExportData001", null };
            var dal = new RequestLogRepository(ConfigurationManager.AppSettings["MongoAdr"], "RequestLog_DataAPI");
            foreach (var id in ids)
            {
                int total = int.MaxValue;
                int count = 0;
                int index = 0;
                int size = 50000;

                do
                {
                    var users = dal.QueryLogByApiId(id, index++, size, out total).Select(log => log.UserId);
                    count += users.Count();
                    users = users.Distinct();
                    foreach (var u in users)
                    {
                        hs.Add(u);
                    }
                    Console.WriteLine("Data api " + (id ?? "NotExportData000") + " count " + hs.Count);
                } while (count < total);
            }
            Console.WriteLine("Total dataapi users count " + hs.Count);
            File.AppendAllLines($"{filePath}_dataapiUser.csv", hs);

            hs.Clear();
            dal = new RequestLogRepository(ConfigurationManager.AppSettings["MongoAdr"], "RequestLog_AdvancedAPI");
            foreach (var id in ids)
            {
                int total = int.MaxValue;
                int count = 0;
                int index = 0;
                int size = 50000;

                do
                {
                    var users = dal.QueryLogByApiId(id, index++, size, out total).Select(log => log.UserId);
                    count += users.Count();
                    users = users.Distinct();
                    foreach (var u in users)
                    {
                        hs.Add(u);
                    }
                    Console.WriteLine("AdvancedAPI api " + (id ?? "NotExportData000") + " count " + hs.Count);
                } while (count < total);
            }
            Console.WriteLine("Total advancedapi users count " + hs.Count);
            File.AppendAllLines($"{filePath}_advancedapiUser.csv", hs);

        }
    }

}