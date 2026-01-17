using ShopVerse.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopVerse.Entities.Concrete;

public class Order : BaseEntity
{
    [StringLength(50)]
    public string OrderNumber { get; set; } 

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    
    public OrderStatus OrderStatus { get; set; }

   
    [Required(ErrorMessage = "Lütfen ad-soyad giriniz.")]
    [StringLength(100)]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Lütfen adres bilgisini giriniz.")]
    [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir.")]
    public string AddressLine { get; set; }

    [Required(ErrorMessage = "Lütfen şehir bilgisini giriniz.")]
    [StringLength(50)]
    public string City { get; set; }

    [Required(ErrorMessage = "Lütfen ilçe bilgisini giriniz.")]
    [StringLength(50)]
    public string District { get; set; }

    [Required(ErrorMessage = "Lütfen telefon numarası giriniz.")]
    [StringLength(20)]
    public string PhoneNumber { get; set; }

    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }

    public DateTime? DeliveryDate { get; set; }
}