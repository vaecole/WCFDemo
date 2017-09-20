using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDBDemo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBDemo.DAOs
{
    public class RequestLogRepository : StandardMongoDataAccessor<RequestLog>
    {
        public RequestLogRepository(string dbserver) : base("RequestLog_archive0915", dbserver, "ApiLogs")
        { }

        public List<RequestLog> QueryLogFrom2(DateTime startTime, DateTime endTime)
        {
            IMongoQuery query = Query<RequestLog>.Where(e => e.CreatedDate >= startTime && e.CreatedDate <= endTime);
            return GetList(query);
        }

        public List<RequestLog> QueryDataAPILogFrom2(DateTime startTime, DateTime endTime)
        {
            IMongoQuery query = Query<RequestLog>.Where(e => e.CreatedDate >= startTime && e.CreatedDate <= endTime && e.Url.Contains("http://dataapi.bazhuayu.com/api/"));
            return GetList(query);
        }

    }
}
