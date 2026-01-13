using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebUI.Models
{
    public class CartItemModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalPrice => SalePrice * Quantity;
    }
}