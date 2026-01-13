using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Abstract;

public interface ICouponRepository:IGenericRepository<Coupon>
{
    Coupon GetByCode(string code);
}
