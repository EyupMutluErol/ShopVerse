using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Areas.Admin.Models;

public class CategoryUpdateViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [Display(Name = "Kategori Adı")]
    public string Name { get; set; }

    [Display(Name = "Açıklama")]
    public string Description { get; set; }
    public IFormFile? ImageFile { get; set; }
    public string? ExistingImageUrl { get; set; } // Yüklenmezse eskisini tutacağız
}
