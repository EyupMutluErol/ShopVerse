using System.ComponentModel.DataAnnotations;

namespace ShopVerse.Entities.Concrete;

public class CartItem:BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int CartId { get; set; }
    public Cart Cart { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Sepete en az 1 adet ürün eklemelisiniz.")]
    public int Quantity { get; set; } 
}
