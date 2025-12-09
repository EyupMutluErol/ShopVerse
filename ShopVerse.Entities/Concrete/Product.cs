using System.ComponentModel.DataAnnotations;

namespace ShopVerse.Entities.Concrete;

public class Product:BaseEntity
{
    [Required(ErrorMessage = "Lütfen ürün adını giriniz.")]
    [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
    public string Name { get; set; }
    [StringLength(1000, ErrorMessage = "Açıklama alanı en fazla 1000 karakter olabilir.")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Lütfen ürün fiyatını giriniz.")]
    [Range(0, double.MaxValue, ErrorMessage = "Fiyat 0'dan küçük olamaz.")]
    public decimal Price { get; set; }
    [Range(0, 100, ErrorMessage = "İndirim oranı 0 ile 100 arasında olmalıdır.")]
    public int DiscountRate { get; set; } = 0; // Varsayılan 0 (İndirim yok)

    public decimal PriceWithDiscount { get; set; }
    [Required(ErrorMessage = "Lütfen stok adedini giriniz.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok adedi 0'dan küçük olamaz.")]
    public int Stock { get; set; }
    [Required(ErrorMessage = "Lütfen bir ürün görseli seçiniz.")]
    [StringLength(250)]
    public string ImageUrl { get; set; }

    // Durumlar
    public bool IsHome { get; set; } // Anasayfada göster
    public bool IsActive { get; set; } // Satışta mı?

    // İlişkiler
    [Required(ErrorMessage = "Lütfen bir kategori seçiniz.")]
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}
