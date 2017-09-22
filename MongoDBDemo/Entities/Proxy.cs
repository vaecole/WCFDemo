using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBDemo.Entities
{
    /// <summary>
    /// 代理IP使用日志
    /// </summary>
    [CollectionName("ProxyUseLog")]
    public class ProxyUseLogEntity : MongoEntity
    {
        public string UserId { get; set; }
        public string TaskId { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public DateTime StartUseTime { get; set; }
        public DateTime RecycleTime { get; set; }
        public double UsedMinutes { get; set; }
        public DateTime DistributionTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string Source { get; set; }

    }

}
