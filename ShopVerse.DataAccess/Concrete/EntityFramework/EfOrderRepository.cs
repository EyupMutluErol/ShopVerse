using Microsoft.EntityFrameworkCore;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfOrderRepository : EfGenericRepository<Order>, IOrderRepository
{
    private readonly ShopVerseContext _context;
    public EfOrderRepository(ShopVerseContext context) : base(context)
    {
        _context = context;
    }

    public List<Order> GetOrdersByUserId(string userId)
    {
        return _context.Orders
            .Include(x=>x.OrderDetails)
            .ThenInclude(y=>y.Product)
            .Where(x=>x.AppUserId == userId)
            .OrderByDescending(x=>x.OrderDate)
            .ToList();
    }
}
