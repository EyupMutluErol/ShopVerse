using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopVerse.Entities.Concrete;

public class OrderDetail:BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; } // Satış anındaki birim fiyat
    [Range(1, int.MaxValue, ErrorMessage = "Sipariş adedi en az 1 olmalıdır.")]
    public int Quantity { get; set; }
    public decimal TotalPrice => Price * Quantity;
}