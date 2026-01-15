using ShopVerse.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopVerse.Entities.Concrete;

public class Order:BaseEntity
{
    [StringLength(50)]
    public string OrderNumber { get; set; } // Sipariş No (Örn: SP-20251213-99)
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public OrderStatus OrderStatus { get; set; } // Enum yapısı

    // --- Kargo Bilgileri ---
    [Required(ErrorMessage = "Lütfen ad-soyad giriniz.")]
    [StringLength(100)]
    public string FullName { get; set; } // Kargo kime gidecek?

    [Required(ErrorMessage = "Lütfen adres bilgisini giriniz.")]
    [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir.")]
    public string AddressLine { get; set; }

    [Required(ErrorMessage = "Lütfen şehir bilgisini giriniz.")]
    [StringLength(50)]
    public string City { get; set; }

    [Required(ErrorMessage = "Lütfen ilçe bilgisini giriniz.")]
    [StringLength(50)]
    public string District { get; set; } // İlçe bilgisi 

    [Required(ErrorMessage = "Lütfen telefon numarası giriniz.")]
    [StringLength(20)]
    public string PhoneNumber { get; set; } // Kurye için gerekli

    // İlişkiler
    public string? AppUserId { get; set; } // AppUser ID'si string ise bu doğru
    public AppUser? AppUser { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
}