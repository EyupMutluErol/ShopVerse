using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfCouponRepository:EfGenericRepository<Coupon>, ICouponRepository
{
    private readonly ShopVerseContext _context;

    public EfCouponRepository(ShopVerseContext context) : base(context)
    {
        _context = context;
    }

    public Coupon GetByCode(string code)
    {
        return _context.Coupons.FirstOrDefault(x => x.Code == code);
    }
}
