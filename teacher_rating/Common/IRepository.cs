using System.Linq.Expressions;
using MongoDB.Driver;

namespace teacher_rating.Common
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task Add(TEntity obj);

        Task<TEntity> GetById(string id);

        Task<TEntity> GetDeletedById(string id);

        Task<List<TEntity>> GetByListId(List<string> ids);

        Task<IEnumerable<TEntity>> GetAll();

        Task Update(TEntity obj);

        Task Remove(long id);

        Task InsertMany(List<TEntity> objs);

        Task UpdateMany(IEnumerable<TEntity> entities);

        Task<long> CountByCondition(Expression<Func<TEntity, bool>> query);

        Task<List<TEntity>> FindByCondition(Expression<Func<TEntity, bool>> query, int skip = -1, int take = -1);

        Task<List<TEntity>> FindWithFilter(FilterDefinition<TEntity> filter);
   
    }
}