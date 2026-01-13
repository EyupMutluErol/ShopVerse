using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Concrete;

public class CouponManager:GenericManager<Coupon>, ICouponService
{
    private readonly ICouponRepository _couponRepository;

    public CouponManager(ICouponRepository couponRepository) : base(couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public Coupon GetByCode(string code)
    {
        return _couponRepository.GetByCode(code);
    }
}
