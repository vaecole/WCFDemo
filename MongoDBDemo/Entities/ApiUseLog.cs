using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBDemo.Entities
{
    [CollectionName("RequestLog")]
    public class RequestLog : MongoEntity
    {
        public string UserId { get; set; }

        public string ApiId { get; set; }

        public string Url { get; set; }

        public string Paramaters { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    [CollectionName("RequestLog_DataAPI_")]
    public class RequestLog_DataAPI : MongoEntity
    {
        public string UserId { get; set; }
        public string Para { get; set; }
    }
}
