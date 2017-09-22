using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDBDemo
{
    public abstract class StandardMongoDataAccessor<T> : IMongoDataAccessor<T> where T : MongoEntity, new()
    {
        private string _collectionName;
        private string _databaseServer;
        private string _databaseName;
        private MongoServer _server;
        private MongoDatabase _database;

        public StandardMongoDataAccessor(string collectionName, string databaseServer, string databaseName)
        {
            _collectionName = collectionName;
            _databaseServer = databaseServer;
            _databaseName = databaseName;
            Inialize();
        }

        #region IMongoDataAccessor<T> Members

        public void Update(IMongoQuery query, IMongoUpdate update)
        {
            this.GetOperationWrapper().Update(query, update, UpdateFlags.Multi);
        }

        public void Update(IMongoQuery query, IMongoUpdate update, UpdateFlags flag)
        {
            this.GetOperationWrapper().Update(query, update, flag);
        }

        public bool Exists(IMongoQuery query)
        {
            return this.GetOperationWrapper().Count(query) != 0;
        }

        public T Insert(T document)
        {
            GetOperationWrapper().Insert<T>(document);
            return document;
        }

        public void InsertBatch(List<T> documents)
        {
            GetOperationWrapper().InsertBatch<T>(documents);
        }

        public void InsertBatch(List<T> documents, MongoInsertOptions options)
        {
            GetOperationWrapper().InsertBatch<T>(documents, options);
        }

        public T GetSingle(IMongoQuery query, IMongoSortBy sortBy = null)
        {
            if (sortBy == null)
            {
                return GetOperationWrapper().FindOneAs<T>(query);
            }
            else
            {
                return GetOperationWrapper().FindAs<T>(query).SetSortOrder(sortBy).SingleOrDefault();
            }
        }

        public List<T> GetList(IMongoQuery query, IMongoSortBy sortBy = null)
        {
            if (sortBy == null)
                return GetOperationWrapper().FindAs<T>(query).ToList();
            else
                return GetOperationWrapper().FindAs<T>(query).SetSortOrder(sortBy).ToList();
        }

        public List<T> GetListPaged(IMongoQuery query, int pageSize, int pageIndex, out int total)
        {
            //IMongoSortBy sort = SortBy<T>.Ascending(x => x._id);
            MongoCursor<T> list = GetOperationWrapper().FindAs<T>(query);//.SetSortOrder(sort);
            total = (int)list.Count();
            List<T> result = list.Skip<T>(pageSize * (pageIndex <= 0 ? 0 : pageIndex - 1)).Take<T>(pageSize).ToList<T>();
            return result;

        }

        public List<T> GetListPaged(IMongoQuery query, int pageSize, int pageIndex)
        {
            //IMongoSortBy sort = SortBy<T>.Ascending(x => x._id);
            MongoCursor<T> list = GetOperationWrapper().FindAs<T>(query);//.SetSortOrder(sort);
            List<T> result = list.Skip<T>(pageSize * (pageIndex <= 0 ? 0 : pageIndex - 1)).Take<T>(pageSize).ToList<T>();
            return result;

        }

        public void Delete(IMongoQuery query)
        {
            GetOperationWrapper().Remove(query, RemoveFlags.None);
        }

        public long DeleteWithResult(IMongoQuery query)
        {
            var res = GetOperationWrapper().Remove(query, RemoveFlags.None);
            return res.DocumentsAffected;
        }

        public int Count(IMongoQuery query)
        {
            return (int)GetOperationWrapper().Count(query);
        }

        #endregion

        protected MongoCollection GetOperationWrapper()
        {
            try
            {
                return _database.GetCollection(_collectionName);
            }
            catch (MongoConnectionException ex)
            {
                throw new Exception("Can't connect to DataBase", ex);
            }
        }

        private void Inialize()
        {
            try
            {
                if (null == _server)
                {
                    //创建数据库链接
                    _server = new MongoClient(_databaseServer).GetServer();
                }
                if (null == _database)
                {
                    //获得数据库cnblogs
                    _database = _server.GetDatabase(_databaseName);
                }
                if (!_database.CollectionExists(_collectionName))
                {
                    _database.CreateCollection(_collectionName);
                    MongoCollection col = _database.GetCollection(_collectionName);
                    MongoEntity doc = new T();
                    IMongoIndexKeys key = doc.GetIndexKey();
                    if (null != key)
                    {
                        col.CreateIndex(key);
                    }

                    IMongoIndexKeys uniqueKey = doc.GetUniqueIndexKey();
                    if (null != uniqueKey)
                    {
                        col.CreateIndex(uniqueKey, IndexOptions.SetUnique(true));
                    }
                }
            }
            catch (MongoConnectionException ex)
            {
                throw new Exception("Can't connect to DataBase", ex);
            }
        }

        void IMongoDataAccessor<T>.Insert(T document)
        {
            throw new NotImplementedException();
        }
    }


    interface IMongoDataAccessor<T> where T : MongoEntity
    {
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        void Update(IMongoQuery query, IMongoUpdate update);

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        void Insert(T document);

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        void InsertBatch(List<T> documents);

        /// <summary>
        /// 取一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetSingle(IMongoQuery query, IMongoSortBy sortBy = null);

        /// <summary>
        /// 取数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetList(IMongoQuery query, IMongoSortBy sortBy = null);

        /// <summary>
        /// 取数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetListPaged(IMongoQuery query, int pageSize, int pageIndex, out int total);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Document"></param>
        void Delete(IMongoQuery query);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Document"></param>
        bool Exists(IMongoQuery query);
        /// <summary>
        /// 删除数据并且返回受影响行数
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        long DeleteWithResult(IMongoQuery query);

    }


    public abstract class MongoEntity
    {
        public ObjectId _id;

        public virtual IMongoIndexKeys GetIndexKey()
        {
            return null;
        }

        public virtual IMongoIndexKeys GetUniqueIndexKey()
        {
            return null;
        }
    }




    /// <summary>
    /// Attribute used to annotate Enities with to override mongo collection name. By default, when this attribute
    /// is not specified, the classname will be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CollectionName : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the CollectionName class attribute with the desired name.
        /// </summary>
        /// <param name="value">Name of the collection.</param>
        public CollectionName(string value)
        {
#if NET35
            if (string.IsNullOrEmpty(value) || value.Trim().Length == 0)
#else
            if (string.IsNullOrWhiteSpace(value))
#endif
                throw new ArgumentException("Empty collectionname not allowed", "value");

            this.Name = value;
        }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public virtual string Name { get; private set; }
    }
}
