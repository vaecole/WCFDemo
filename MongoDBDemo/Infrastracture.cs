using MongoDB.Bson;
using MongoDB.Driver;
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
        private IMongoClient _client;
        private IMongoDatabase _database;

        public StandardMongoDataAccessor(string collectionName, string databaseServer, string databaseName)
        {
            _collectionName = collectionName;
            _databaseServer = databaseServer;
            _databaseName = databaseName;
            Inialize();
        }

        #region IMongoDataAccessor<T> Members

        public void Update(FilterDefinition<T> query, UpdateDefinition<T> update)
        {
            this.Collection.UpdateMany(query, update);
        }

        public bool Exists(FilterDefinition<T> query)
        {
            return Collection.CountDocuments(query) != 0;
        }

        public T Insert(T document)
        {
            Collection.InsertOne(document);
            return document;
        }

        public void InsertBatch(List<T> documents)
        {
            Collection.InsertMany(documents);
        }

        public List<T> GetList(FilterDefinition<T> query, SortDefinition<T> sortBy = null)
        {
            if (sortBy == null)
                return Collection.Find(query).ToList();
            else
                return Collection.Find(query).Sort(sortBy).ToList();
        }

        public List<T> GetListPaged(FilterDefinition<T> query, int pageSize, int pageIndex, out int total)
        {
            var list = Collection.Find(query);
            total = (int)list.CountDocuments();
            return list.Skip(pageSize * (pageIndex <= 0 ? 0 : pageIndex - 1)).Limit(pageSize).ToList();
        }


        public void Delete(FilterDefinition<T> query)
        {
            Collection.DeleteMany(query);
        }

        #endregion

        protected IMongoCollection<T> Collection
        {
            get
            {
                try
                {
                    return _database.GetCollection<T>(_collectionName);
                }
                catch (MongoConnectionException ex)
                {
                    throw new Exception("Can't connect to DataBase", ex);
                }
            }
        }

        private void Inialize()
        {
            try
            {
                if (null == _client)
                {
                    //创建数据库链接
                    _client = new MongoClient(_databaseServer);
                }
                if (null == _database)
                {
                    //获得数据库cnblogs
                    _database = _client.GetDatabase(_databaseName);
                }
                if (!_database.ListCollectionNames().ToList().Exists(c => c.Equals(_collectionName)))
                {
                    _database.CreateCollection(_collectionName);
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
        void Update(FilterDefinition<T> query, UpdateDefinition<T> update);

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
        /// 取数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetList(FilterDefinition<T> query, SortDefinition<T> sortBy = null);

        /// <summary>
        /// 取数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetListPaged(FilterDefinition<T> query, int pageSize, int pageIndex, out int total);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Document"></param>
        void Delete(FilterDefinition<T> query);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Document"></param>
        bool Exists(FilterDefinition<T> query);

    }


    public abstract class MongoEntity
    {
        public ObjectId _id;
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
