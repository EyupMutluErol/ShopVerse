using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Models;

public class ResetPasswordViewModel
{
    [Required]
    public string Token { get; set; } // URL'den gelen gizli anahtar

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
    public string ConfirmPassword { get; set; }
}
