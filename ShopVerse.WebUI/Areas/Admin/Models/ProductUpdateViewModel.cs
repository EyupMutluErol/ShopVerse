using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Areas.Admin.Models;

public class ProductUpdateViewModel
{
    public int Id { get; set; }

    [Display(Name = "Ürün Adı")]
    [Required(ErrorMessage = "Ürün adı zorunludur.")]
    [MaxLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir.")]
    public string Name { get; set; }

    [Display(Name = "Açıklama")]
    [Required(ErrorMessage = "Ürün açıklaması zorunludur.")]
    [MinLength(10, ErrorMessage = "Açıklama en az 10 karakter olmalıdır.")]
    public string Description { get; set; }

    [Display(Name = "Fiyat")]
    [Required(ErrorMessage = "Fiyat bilgisi zorunludur.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
    public decimal? Price { get; set; }
    

    [Display(Name = "İndirim Oranı (%)")]
    [Range(0, 100, ErrorMessage = "0 ile 100 arasında bir değer giriniz.")]
    public int? DiscountRate { get; set; }

    [Display(Name = "Stok Adedi")]
    [Required(ErrorMessage = "Stok bilgisi zorunludur.")]
    [Range(0, 100000, ErrorMessage = "Geçerli bir stok adedi giriniz.")]
    public int? Stock { get; set; }

    [Display(Name = "Kategori")]
    [Required(ErrorMessage = "Lütfen bir kategori seçiniz.")]
    public int? CategoryId { get; set; }

    [Display(Name = "Yeni Görsel Yükle (Opsiyonel)")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingImageUrl { get; set; }

    [Display(Name = "Anasayfada Göster")]
    public bool IsHome { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; }
}