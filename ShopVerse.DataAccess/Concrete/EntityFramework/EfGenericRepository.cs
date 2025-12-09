using Microsoft.EntityFrameworkCore;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Abstract;
using System.Linq.Expressions;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfGenericRepository<T> : IGenericRepository<T> where T : class, IEntity, new()
{
    private readonly ShopVerseContext _context;

    public EfGenericRepository(ShopVerseContext context)
    {
        _context = context;
    }

    public async Task AddAsync(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null)
    {
        return filter == null ? await _context.Set<T>().ToListAsync() : await _context.Set<T>().Where(filter).ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }
}
