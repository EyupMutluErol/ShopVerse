using Microsoft.AspNetCore.Identity;
using ShopVerse.Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace ShopVerse.Entities.Concrete;

public class AppUser:IdentityUser,IEntity
{
    [Required(ErrorMessage = "Lütfen adınızı giriniz.")]
    [StringLength(50, ErrorMessage = "İsim alanı en fazla 50 karakter olabilir.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Lütfen soyadınızı giriniz.")]
    [StringLength(50, ErrorMessage = "Soyisim alanı en fazla 50 karakter olabilir.")]
    public string Surname { get; set; }
    [StringLength(50, ErrorMessage = "Şehir adı en fazla 50 karakter olabilir.")]
    public string? City { get; set; }
}
