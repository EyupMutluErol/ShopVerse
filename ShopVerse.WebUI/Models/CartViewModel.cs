using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebUI.Models;

public class CartViewModel
{
    public List<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
    public decimal SubTotal => CartItems.Sum(x => x.TotalPrice);
    public decimal ShippingCost => SubTotal >= 500 ? 0 : 39.90m;
    public decimal GrandTotal => CartItems.Sum(x => x.TotalPrice); 
}


