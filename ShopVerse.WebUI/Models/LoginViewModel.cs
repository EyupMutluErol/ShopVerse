using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress]
    [Display(Name = "E-Posta")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; }

    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }
}
