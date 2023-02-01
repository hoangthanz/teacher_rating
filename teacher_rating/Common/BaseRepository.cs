using System.Linq.Expressions;
using CKLS.Services.Models.MongoDb.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack;

namespace teacher_rating.Common
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public readonly IMongoContext Context;
        public IMongoCollection<TEntity> DbSet;

        public BaseRepository(IMongoContext context)
        {
            Context = context;

            DbSet = Context.GetCollection<TEntity>(typeof(TEntity).Name);
        }
        
        public async Task<List<TEntity>> FindByCondition(Expression<Func<TEntity, bool>> query, int skip = -1,
            int take = -1)
        {
            // Return the enumerable of the collection
            if (skip != -1 && take != -1)
                return await DbSet.Find<TEntity>(query).Skip(skip).Limit(take).ToListAsync();
            return await DbSet.Find<TEntity>(query).ToListAsync();
        }

        public async Task<List<TEntity>> FindWithFilter(FilterDefinition<TEntity> filter)
        {
            return await DbSet.Find<TEntity>(filter).ToListAsync();
        }


        [Obsolete("Obsolete")]
        public async Task<long> CountByCondition(Expression<Func<TEntity, bool>> query)
        {
            // Return the enumerable of the collection
            return await DbSet.Find<TEntity>(query).CountAsync();
        }

        public async Task Add(TEntity obj)
        {
            await DbSet.InsertOneAsync(obj);
        }

        public Task<TEntity> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetDeletedById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task InsertMany(List<TEntity> objs)
        {
            await DbSet.InsertManyAsync(objs);
        }

        private async Task UpdateManyAsync(IEnumerable<TEntity> entities)
        {
            var updates = new List<WriteModel<TEntity>>();
            var filterBuilder = Builders<TEntity>.Filter;

            foreach (var doc in entities)
            {
                foreach (var prop in typeof(TEntity).GetProperties())
                {
                    if (prop.Name == "Id")
                    {
                        var filter = filterBuilder.Eq(prop.Name, prop.GetValue(doc));
                        updates.Add(new ReplaceOneModel<TEntity>(filter, doc));
                        break;
                    }
                }
            }

            BulkWriteResult result = await DbSet.BulkWriteAsync(updates);
        }

        public async Task UpdateMany(IEnumerable<TEntity> entities)
        {
            await UpdateManyAsync(entities);
        }

        public virtual async Task<TEntity> GetById(long id)
        {
            var idp = id;
            var filters = new List<FilterDefinition<TEntity>>();
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            var filterDelete = Builders<TEntity>.Filter.Eq("is_del", false);
            filters.Add(filter);
            filters.Add(filterDelete);
            var complexFilter = Builders<TEntity>.Filter.And(filters);
            var data = await DbSet.FindAsync(complexFilter);
            return data.SingleOrDefault();
        }
        
        public virtual async Task<TEntity> GetByIdString(string id)
        {
            var idp = ObjectId.Parse(id);
            var filters = new List<FilterDefinition<TEntity>>();
            var filter = Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id));
            var filterDelete = Builders<TEntity>.Filter.Eq("is_del", false);
            filters.Add(filter);
            filters.Add(filterDelete);
            var complexFilter = Builders<TEntity>.Filter.And(filters);
            var data = await DbSet.FindAsync(complexFilter);
            return data.SingleOrDefault();
        }

        public virtual async Task<TEntity> GetDeletedById(long id)
        {
            var idp = id;
            var filters = new List<FilterDefinition<TEntity>>();
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            var filterDelete = Builders<TEntity>.Filter.Eq("is_del", true);
            filters.Add(filter);
            filters.Add(filterDelete);
            var complexFilter = Builders<TEntity>.Filter.And(filters);
            var data = await DbSet.FindAsync(complexFilter);
            return data.SingleOrDefault();
        }

        public virtual async Task<List<TEntity>> GetByListId(List<long> ids)
        {
            var data = await DbSet.FindAsync(Builders<TEntity>.Filter.In("_id", ids));
            return data.ToList();
        }

        public virtual async Task<List<TEntity>> GetByListId(List<string> ids)
        {
            var data = await DbSet.FindAsync(Builders<TEntity>.Filter.In("_id", ids));
            return data.ToList();
        }
        
        public virtual async Task<TEntity> GetByName(string name)
        {
            var filters = new List<FilterDefinition<TEntity>>();
            var filter = Builders<TEntity>.Filter.Eq("name", name);
            var filterDelete = Builders<TEntity>.Filter.Eq("is_del", false);
            filters.Add(filter);
            filters.Add(filterDelete);
            var complexFilter = Builders<TEntity>.Filter.And(filters);
            var data = await DbSet.FindAsync(complexFilter);
            return data.SingleOrDefault();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
            return all.ToList();
        }

        public async Task Update(TEntity obj)
        {
            await DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(obj.GetId().ToString())),
                obj);
        }

        public async Task Remove(long id)
        {
            await DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id));
        }
        
        public async Task Remove(string id)
        {
            await DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id));
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}