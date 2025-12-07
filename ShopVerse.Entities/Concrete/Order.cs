using ShopVerse.Entities.Enums;

namespace ShopVerse.Entities.Concrete;

public class Order:BaseEntity
{
    public string OrderNumber { get; set; } 
    public decimal TotalPrice { get; set; } 
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public OrderStatus OrderStatus { get; set; }
    public string AddressLine { get; set; }
    public string City { get; set; }
    public string UserId { get; set; } 
    public AppUser AppUser { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
}
