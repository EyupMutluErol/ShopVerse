using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Areas.Admin.Models;

public class CouponAddViewModel
{
    [Display(Name = "Kupon Kodu")]
    [Required(ErrorMessage = "Kupon kodu zorunludur.")]
    [StringLength(20, ErrorMessage = "Kupon kodu en fazla 20 karakter olabilir.")]
    public string Code { get; set; }

    [Display(Name = "İndirim Tipi")]
    public bool IsPercentage { get; set; }

    [Display(Name = "İndirim Tutarı / Oranı")]
    [Required(ErrorMessage = "İndirim değerini giriniz.")]
    [Range(0.01, 100000, ErrorMessage = "Geçerli bir indirim tutarı giriniz.")]
    public decimal? DiscountAmount { get; set; }

    [Display(Name = "Min. Sepet Tutarı")]
    [Required(ErrorMessage = "Minimum sepet tutarı zorunludur.")]
    public decimal? MinCartAmount { get; set; }

    [Display(Name = "Son Kullanma Tarihi")]
    [Required(ErrorMessage = "Son kullanma tarihi seçilmelidir.")]
    public DateTime? ExpirationDate { get; set; }


    [Display(Name = "Kategori")]
    public int? CategoryId { get; set; } 

    [Display(Name = "Kullanıcı")]
    public string? UserId { get; set; } 


    [Display(Name = "Aktiflik Durumu")]
    public bool IsActive { get; set; } = true;

    public decimal? MinProductPrice { get; set; }
    public decimal? MaxProductPrice { get; set; }
}