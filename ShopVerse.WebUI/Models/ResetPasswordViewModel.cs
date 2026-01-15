namespace ShopVerse.WebUI.Models;

public class ResetPasswordViewModel
{
    public string Token { get; set; } // Şifre sıfırlama bileti
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
