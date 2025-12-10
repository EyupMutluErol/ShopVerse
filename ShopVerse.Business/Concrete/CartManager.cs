using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Concrete;

public class CartManager:GenericManager<Cart>,ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartManager(ICartRepository cartRepository) : base(cartRepository)
    {
        _cartRepository = cartRepository;
    }
}
