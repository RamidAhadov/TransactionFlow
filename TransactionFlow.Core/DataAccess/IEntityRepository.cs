using System.Linq.Expressions;
using TransactionFlow.Entities;

namespace TransactionFlow.Core.DataAccess;

public interface IEntityRepository<T> where T: class, IEntity, new()
{
    List<T> GetList(Expression<Func<T, bool>>? filter = null);
    T? Get(Expression<Func<T, bool>> filter);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void DeleteRange(List<T> entities);
    Task<List<T>> GetListAsync(Expression<Func<T, bool>>? filter = null);
    Task<T?> GetAsync(Expression<Func<T, bool>> filter);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task UpdateRangeAsync(List<T> entities);
    Task<T> DeleteAsync(T entity);
    Task DeleteRangeAsync(List<T> entities);
    Task<T> DeleteAsync(Expression<Func<T, bool>> filter);
}