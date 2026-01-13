using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Abstract;

public interface ICouponService:IGenericService<Coupon>
{
    Coupon GetByCode(string code);
}
