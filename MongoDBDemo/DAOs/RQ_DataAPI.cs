using MongoDB.Driver;
using MongoDBDemo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Builders;

namespace MongoDBDemo.DAOs
{
    public class RQ_DataAPIDAO : StandardMongoDataAccessor<RequestLog_DataAPI>
    {
        public RQ_DataAPIDAO(string dbserver) : base("RequestLog_archive0915", dbserver, "ApiLogs")
        { }


        public long GetTotalCount()
        {
            return GetOperationWrapper().Count(Query<RequestLog_DataAPI>.Where(r => true));
        }
        public IEnumerable<RequestLog_DataAPI> QueryDistinct(int limiteOnce = 1000)
        {
            var cursor = GetOperationWrapper().FindAllAs<RequestLog_DataAPI>();
            return cursor.Distinct();
        }

    }
}
