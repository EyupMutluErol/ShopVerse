using System.ComponentModel.DataAnnotations;

namespace ShopVerse.Entities.Concrete;

public class OrderDetail:BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Satış fiyatı hatalı.")]
    public decimal Price { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Sipariş adedi en az 1 olmalıdır.")]
    public int Quantity { get; set; }
}
