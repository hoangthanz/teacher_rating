using System.Linq.Expressions;

namespace teacher_rating.Common.Contracts;

public interface IRepositoryBase<TEntity>: IDisposable where TEntity : class
{
    IQueryable<TEntity> FindAll();
    IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression);
    void Create(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}