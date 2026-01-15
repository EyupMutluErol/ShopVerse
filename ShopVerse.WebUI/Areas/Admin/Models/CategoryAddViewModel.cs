using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Areas.Admin.Models;

public class CategoryAddViewModel
{
    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [Display(Name = "Kategori Adı")]
    public string Name { get; set; }

    [Display(Name = "Açıklama")]
    [Required(ErrorMessage = "Lütfen açıklama alanını boş bırakmayınız.")]
    public string Description { get; set; }
    public IFormFile? ImageFile { get; set; }
}
