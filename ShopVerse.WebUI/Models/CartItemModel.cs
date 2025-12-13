using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebUI.Models
{
    public class CartItemModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Product.Price * Quantity; // Toplam tutarı hesaplıyoruz

    }
}
