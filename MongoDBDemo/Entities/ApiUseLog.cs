using MongoDB.Bson;
using System;

namespace MongoDBDemo.Entities
{

    public class BaseModel<TId>
    {
        public TId Id { get; set; }
    }

    public class RequestLog : BaseModel<ObjectId>
    {
        public string UserId { get; set; }

        public string ApiId { get; set; }

        public string Url { get; set; }

        public string Paramaters { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
