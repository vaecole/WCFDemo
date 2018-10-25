using MongoDB.Driver;
using MongoDB.Bson.Serialization.Serializers;
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
        public RequestLogRepository(string dbserver, string collection) : base(collection, dbserver, "DataApiLogs")
        { }

        public List<RequestLog> QueryLogFrom2(DateTime startTime, DateTime endTime)
        {
            var filter = Builders<RequestLog>.Filter.Where(e => e.CreatedDate >= startTime && e.CreatedDate <= endTime);
            return GetList(filter);
        }

        public List<RequestLog> QueryLogByApiId(string id, int index, int size, out int total, DateTime startTime, DateTime endTime)
        {
            var query = Builders<RequestLog>.Filter.Where(e => e.ApiId == id && e.CreatedDate >= startTime && e.CreatedDate <= endTime);
            return GetListPaged(query, size, index, out total);
        }

        public List<RequestLog> QueryLogByUserAndApiId(string userId, string id, int index, int size, out int total)
        {
            var query = Builders<RequestLog>.Filter.Where(e => e.UserId.Equals(userId) && e.ApiId == id);
            return GetListPaged(query, size, index, out total);
        }
    }
}
