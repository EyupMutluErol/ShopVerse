using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Areas.Admin.Models;

public class ProductAddViewModel
{
    [Required(ErrorMessage = "Ürün adı zorunludur.")]
    [Display(Name = "Ürün Adı")]
    public string Name { get; set; }

    [Display(Name = "Açıklama")]
    [Required(ErrorMessage = "Lütfen ürün açıklamasını giriniz.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Fiyat zorunludur.")]
    [Display(Name = "Fiyat")]
    public decimal? Price { get; set; }

    [Display(Name = "İndirim Oranı (%)")]
    [Range(0, 100, ErrorMessage = "0-100 arası giriniz.")]
    public int? DiscountRate { get; set; } = 0;

    [Required(ErrorMessage = "Stok zorunludur.")]
    [Display(Name = "Stok")]
    public int? Stock { get; set; }

    [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    [Display(Name = "Kategori")]
    public int? CategoryId { get; set; } // Dropdown'dan seçilen ID buraya gelecek

    [Display(Name = "Ürün Görseli")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Anasayfada Göster")]
    public bool IsHome { get; set; }

    [Display(Name = "Satışta (Aktif)")]
    public bool IsActive { get; set; } = true;
}
