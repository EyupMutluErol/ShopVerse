using System.ComponentModel.DataAnnotations.Schema;

namespace ShopVerse.Entities.Concrete;

public class Coupon:BaseEntity
{
    public string Code { get; set; } // Kupon Kodu (Örn: YAZ2026)
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } // İndirim Miktarı (Örn: 50 veya 10)
    public bool IsPercentage { get; set; }    // Yüzde mi? (True ise %10, False ise 50 TL)
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinCartAmount { get; set; } // Minimum Sepet Tutarı (Örn: 500 TL altı kullanamazsın)
    public DateTime ExpirationDate { get; set; } // Son Kullanma Tarihi
    public bool IsActive { get; set; }
    public int? CategoryId { get; set; }
    public Category Category { get; set; }
}
