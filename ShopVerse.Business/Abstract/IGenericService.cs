using ShopVerse.Entities.Abstract;
using System.Linq.Expressions;

namespace ShopVerse.Business.Abstract;

public interface IGenericService<T> where T : class,IEntity,new()
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null);
}
