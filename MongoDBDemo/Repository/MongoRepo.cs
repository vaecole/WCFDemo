using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBDemo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDBDemo.Repository
{


    public class MongoRepository
    {
        protected IMongoDatabase Database { get; private set; }

        public string ConnectionString { get; private set; }

        public string DatabaseName { get; private set; }

        public string CollectionName { get; private set; }

        protected IMongoCollection<BsonDocument> Collection => Database.GetCollection<BsonDocument>(CollectionName);

        public MongoRepository(string connectionString, string databaseName, string collectionName)
        {
            ConnectionString = connectionString;
            CollectionName = collectionName;
            DatabaseName = databaseName;

            var client = new MongoClient(ConnectionString);
            Database = GetDatabase(client, DatabaseName);

            CreateCollectionIfNotExists(Database);
        }


        private IMongoDatabase GetDatabase(MongoClient client, string databaseName)
        {
            return client.GetDatabase(databaseName);
        }

        private void CreateCollectionIfNotExists(IMongoDatabase database)
        {
            if (!database.ListCollectionNames().ToList().Any(name => name.Equals(CollectionName)))
            {
                database.CreateCollection(CollectionName);
            }
        }
    }



    public class MongoRepository<T, TId> : MongoRepository where T : BaseModel<TId>
    {
        protected new IMongoCollection<T> Collection => Database.GetCollection<T>(CollectionName);

        public MongoRepository(string connectionString, string databaseName, string collectionName)
            : base(connectionString, databaseName, collectionName)
        {
        }

        public async Task AddAsync(T model)
        {
            await Collection.InsertOneAsync(model);
        }


        public async Task AddAsync(IEnumerable<T> models)
        {
            await Collection.InsertManyAsync(models);
        }

        public async Task<T> GetAsync(TId id)
        {
            var filter = Builders<T>.Filter.Eq(m => m.Id, id);

            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await Collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<T>> GetPageAsync(FilterDefinition<T> filter, int start, int take)
        {
            return await Collection
                .Find(filter)
                .SortBy(m => m.Id)
                .Skip(start)
                .Limit(take)
                .ToListAsync();
        }

        public async Task<List<T>> GetLastestPageAsync(FilterDefinition<T> filter, int start, int take)
        {
            return await Collection
                .Find(filter)
                .SortByDescending(m => m.Id)
                .Skip(start)
                .Limit(take)
                .ToListAsync();
        }

        public async Task RemoveAsync(TId id)
        {
            var filter = Builders<T>.Filter.Eq(m => m.Id, id);

            await Collection.DeleteOneAsync(filter);
        }

        public async Task UpdateAsync(T model)
        {
            var filter = Builders<T>.Filter.Eq(m => m.Id, model.Id);

            await Collection.ReplaceOneAsync(filter, model, new UpdateOptions { IsUpsert = false });
        }
    }
}
