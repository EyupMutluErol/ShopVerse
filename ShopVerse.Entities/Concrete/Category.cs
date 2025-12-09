using System.ComponentModel.DataAnnotations;

namespace ShopVerse.Entities.Concrete;

public class Category:BaseEntity
{
    [Required(ErrorMessage = "Lütfen kategori adını giriniz.")]
    [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
    public string Name { get; set; }
    [StringLength(1000, ErrorMessage = "Açıklama alanı en fazla 1000 karakter olabilir.")]
    public string Description { get; set; }
    [StringLength(250)]
    public string? ImageUrl { get; set; }
    public List<Product> Products { get; set; }
}
