using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Column attribute için gerekli

namespace ShopVerse.Entities.Concrete;

public class Campaign : BaseEntity
{
    [Display(Name = "Kampanya Başlığı")]
    [Required(ErrorMessage = "Başlık zorunludur.")]
    public string Title { get; set; }

    [Display(Name = "Açıklama / Alt Başlık")]
    public string Description { get; set; }

    [Display(Name = "Banner Görseli")]
    public string ImageUrl { get; set; }

    [Display(Name = "İndirim Oranı (%)")]
    [Range(1, 99, ErrorMessage = "1 ile 99 arası bir değer giriniz.")]
    public int DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);

    public bool IsActive { get; set; } = true;

    // ========================================================================
    // YENİ EKLENEN ALANLAR (ÜRÜN BAZLI FİYAT ARALIĞI)
    // ========================================================================
    [Display(Name = "Min. Ürün Fiyatı")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MinProductPrice { get; set; } // Bu fiyattan ucuz ürünleri kapsama

    [Display(Name = "Max. Ürün Fiyatı")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MaxProductPrice { get; set; } // Bu fiyattan pahalı ürünleri kapsama
    // ========================================================================

    // İlişkiler
    public int? TargetCategoryId { get; set; }
    public Category? TargetCategory { get; set; }
}