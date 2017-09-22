using MongoDBDemo.Entities;
using MongoDBDemo.Query;
using MongoDBDemo.TextFileHelper;
using System;
using System.Linq;

namespace MongoDBDemo
{
    public static class Program
    {
        static void Main(params string[] agrs)
        {
            DateTime start = DateTime.Now.AddDays(-20);
            DateTime end = DateTime.Now;
            string filePath = nameof(ProxyUseLogEntity) + ".csv";
            if (agrs.Count() > 0)
            {
                if (!DateTime.TryParse(agrs[0], out start))
                {
                    Console.WriteLine(nameof(agrs) + 0 + " invalid");
                }
                else
                {
                    int days = 1;
                    int.TryParse(agrs[0], out days);
                    start = DateTime.Now.AddDays(0 - days);
                }
            }
            if (agrs.Count() > 1)
            {
                if (!DateTime.TryParse(agrs[1], out end))
                {
                    Console.WriteLine(nameof(agrs) + 0 + " invalid");
                }
            }
            if (agrs.Count() > 2)
            {
                filePath = agrs[2];
            }

            var dal = new ProxyUseLogEntityDAL("mongodb://10.105.193.68:27017","UseLog");
            var entities = dal.QueryUserLogFrom2(start, end);
            entities.Convert2Csv(filePath);
        }
    }

}