using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Abstract;
using System.Linq.Expressions;

namespace ShopVerse.Business.Concrete;

public class GenericManager<T> : IGenericService<T> where T : class, IEntity, new()
{
    private readonly IGenericRepository<T> _repository;

    public GenericManager(IGenericRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(T entity)
    {
        await _repository.AddAsync(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        await _repository.DeleteAsync(entity);
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null)
    {
        return await _repository.GetAllAsync(filter);
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        await _repository.UpdateAsync(entity);
    }
}
