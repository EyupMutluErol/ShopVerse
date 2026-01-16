using System.ComponentModel.DataAnnotations.Schema;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Entities.Concrete;

public class Coupon : BaseEntity
{
    public string Code { get; set; } // Kupon Kodu (Örn: YAZ2026)

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } // İndirim Miktarı (Örn: 50 veya 10)

    public bool IsPercentage { get; set; } // Yüzde mi? (True ise %10, False ise 50 TL)

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinCartAmount { get; set; } // Minimum Sepet Tutarı (Örn: Sepet 500 TL altıysa kupon çalışmaz)

    public DateTime ExpirationDate { get; set; } // Son Kullanma Tarihi
    public bool IsActive { get; set; }

    // ========================================================================
    // YENİ EKLENEN ALANLAR (ÜRÜN BAZLI FİYAT ARALIĞI)
    // ========================================================================
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MinProductPrice { get; set; } // Sadece fiyatı X TL'den yüksek ürünlere uygula (Opsiyonel)

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MaxProductPrice { get; set; } // Sadece fiyatı Y TL'den düşük ürünlere uygula (Opsiyonel)
    // ========================================================================

    // İlişkiler
    public int? CategoryId { get; set; }
    public Category? Category { get; set; } // Nullable olması daha güvenli

    public string? UserId { get; set; }
    public AppUser? AppUser { get; set; }
}