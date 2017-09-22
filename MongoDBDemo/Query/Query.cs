using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDBDemo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBDemo.Query
{
    public class ProxyUseLogEntityDAL : StandardMongoDataAccessor<ProxyUseLogEntity>
    {
        public ProxyUseLogEntityDAL(string dbserver, string dbname)
            : base("HttpProxy", dbserver, dbname)
        {

        }

        public void Save(ProxyUseLogEntity entity)
        {
            this.Insert(entity);
        }
        public List<ProxyUseLogEntity> GetListOfTask(string taskId)
        {
            IMongoQuery query = Query<ProxyUseLogEntity>.Where(e => e.TaskId == taskId);
            return this.GetList(query, SortBy<ProxyUseLogEntity>.Descending(e => e.DistributionTime));
        }

        public List<ProxyUseLogEntity> GetListOfTaskPaged(string taskId, int pageSize, int pageIndex, out int total)
        {
            IMongoQuery query = Query<ProxyUseLogEntity>.Where(e => e.TaskId == taskId);
            return this.GetListPaged(query, pageSize, pageIndex, out total);
        }

        public void SaveList(List<ProxyUseLogEntity> entities)
        {
            if (entities == null || !entities.Any())
                return;
            this.InsertBatch(entities);
        }

        public void DeleteRecordForTask(string taskId)
        {
            IMongoQuery query = Query<ProxyUseLogEntity>.Where(e => e.TaskId == taskId);
            this.Delete(query);
        }

        public bool ExistReport(string taskId)
        {
            IMongoQuery query = Query<ProxyUseLogEntity>.Where(e => e.TaskId == taskId);
            return this.Exists(query);
        }

        public List<ProxyUseLogEntity> QueryUserLogFrom2(DateTime startTime, DateTime endTime)
        {
            IMongoQuery query = Query<ProxyUseLogEntity>.Where(e => e.DistributionTime >= startTime && e.DistributionTime <= endTime);
            return GetList(query);
        }
    }

}
