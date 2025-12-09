using ShopVerse.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShopVerse.Entities.Concrete;

public class Order:BaseEntity
{
    [StringLength(50)]
    public string OrderNumber { get; set; } 
    public decimal TotalPrice { get; set; } 
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public OrderStatus OrderStatus { get; set; }
    [Required(ErrorMessage = "Lütfen adres bilgisini giriniz.")]
    [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir.")]
    public string AddressLine { get; set; }
    [Required(ErrorMessage = "Lütfen şehir bilgisini giriniz.")]
    [StringLength(50)]
    public string City { get; set; }
    public string UserId { get; set; } 
    public AppUser AppUser { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
}
