using Microsoft.EntityFrameworkCore;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;
using System.Globalization;

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

    public List<SalesChartDto> GetSalesTrend(int lastMonths)
    {
        var startDate = DateTime.Now.AddMonths(-lastMonths);

        // 1. Veritabanından Net Ciro mantığına uygun veriyi çek
        var query = _context.Orders
            .Where(x => x.OrderDate >= startDate &&
                        !x.IsDeleted && // Silinmiş kayıtları hariç tut
                        x.OrderStatus != ShopVerse.Entities.Enums.OrderStatus.Canceled && // İptalleri dahil etme
                        x.OrderStatus != ShopVerse.Entities.Enums.OrderStatus.Refunded)  // İadeleri dahil etme
            .GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalAmount = g.Sum(x => x.TotalPrice)
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToList();

        // 2. DTO'ya çevir
        var result = query.Select(x => new SalesChartDto
        {
            Date = new DateTime(x.Year, x.Month, 1).ToString("MMMM yyyy", new CultureInfo("tr-TR")),
            TotalSales = x.TotalAmount
        }).ToList();

        return result;
    }
}
