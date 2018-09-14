using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBDemo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBDemo.Repository
{
    public class RequestLogRepo : MongoRepository<RequestLog, ObjectId>
    {
        public RequestLogRepo(string connectionString, string databaseName, string collectionName) : base(connectionString, databaseName, collectionName)
        {
        }

        public Task<List<RequestLog>> QueryLastestByApiId(string apiId, int start, int take, DateTime startTime)
        {
            var startId = ObjectId.GenerateNewId(startTime);
            var filter = Builders<RequestLog>.Filter.Where(r => r.Id > startId && r.ApiId == apiId);
            return GetLastestPageAsync(filter, start, take);
        }
    }
}
