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

    public Order GetOrderWithDetails(int orderId)
    {
        return _context.Orders
            .Include(x => x.OrderDetails)
            .ThenInclude(y => y.Product)
            .FirstOrDefault(x => x.Id == orderId);
    }

    public int GetTotalOrderCount()
    {
        return _context.Orders.Count();
    }

    public decimal GetTotalTurnover()
    {
        return _context.Orders.Any() ? _context.Orders.Sum(x => x.TotalPrice) : 0;
    }
}
