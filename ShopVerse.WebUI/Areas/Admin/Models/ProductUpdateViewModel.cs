using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Areas.Admin.Models;

public class ProductUpdateViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ürün adı zorunludur.")]
    [Display(Name = "Ürün Adı")]
    public string Name { get; set; }
    [Display(Name = "Açıklama")]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Fiyat")]
    public decimal Price { get; set; }

    [Range(0, 100)]
    [Display(Name = "İndirim Oranı (%)")]
    public int? DiscountRate { get; set; }

    [Required]
    [Display(Name = "Stok Adedi")]
    public int Stock { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public IFormFile? ImageFile { get; set; }
    public string? ExistingImageUrl { get; set; } // Resmi değiştirmezse eskisi kalsın

    public bool IsHome { get; set; }
    public bool IsActive { get; set; }
}
