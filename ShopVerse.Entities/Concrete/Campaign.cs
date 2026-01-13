using System.ComponentModel.DataAnnotations;

namespace ShopVerse.Entities.Concrete;

public class Campaign:BaseEntity
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

    public int? TargetCategoryId { get; set; }
    public Category? TargetCategory { get; set; } 
}
