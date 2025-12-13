using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebUI.Models;

public class CartViewModel
{
    public List<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
    public decimal GrandTotal => CartItems.Sum(x => x.TotalPrice); // Sepetin toplam tutarı 
}


