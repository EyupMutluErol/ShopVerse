using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Areas.Admin.Models;

public class CampaignAddViewModel
{
    [Display(Name = "Kampanya Başlığı")]
    [Required(ErrorMessage = "Kampanya başlığı zorunludur.")]
    public string Title { get; set; }

    [Display(Name = "Açıklama (Alt Başlık)")]
    [Required(ErrorMessage = "Kampanya açıklaması zorunludur.")]
    public string Description { get; set; }

    
    [Display(Name = "Başlangıç Tarihi")]
    [Required(ErrorMessage = "Başlangıç tarihi seçilmelidir.")]
    public DateTime? StartDate { get; set; }

    [Display(Name = "Bitiş Tarihi")]
    [Required(ErrorMessage = "Bitiş tarihi seçilmelidir.")]
    public DateTime? EndDate { get; set; }

    [Display(Name = "İndirim Oranı (%)")]
    [Required(ErrorMessage = "İndirim oranı giriniz.")]
    [Range(1, 100, ErrorMessage = "1-100 arası değer giriniz.")]
    public int? DiscountPercentage { get; set; }

    [Display(Name = "Hedef Kategori")]
    public int? TargetCategoryId { get; set; } 

    [Display(Name = "Kampanya Görseli")]
    [Required(ErrorMessage = "Lütfen bir banner görseli seçiniz.")]
    public IFormFile ImageFile { get; set; }

    public bool IsActive { get; set; } = true;

    public decimal? MinProductPrice { get; set; }
    public decimal? MaxProductPrice { get; set; }
}