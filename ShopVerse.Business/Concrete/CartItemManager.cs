using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Concrete;

public class CartItemManager:GenericManager<CartItem>,ICartItemService
{
    private readonly ICartItemRepository _cartItemRepository;

    public CartItemManager(ICartItemRepository cartItemRepository):base(cartItemRepository)
    {
        _cartItemRepository = cartItemRepository;
    }
}
