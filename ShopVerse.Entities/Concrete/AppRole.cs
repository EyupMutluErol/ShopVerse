using Microsoft.AspNetCore.Identity;
using ShopVerse.Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace ShopVerse.Entities.Concrete;

public class AppRole:IdentityRole,IEntity
{
    [StringLength(100, ErrorMessage = "Rol açıklaması en fazla 100 karakter olabilir.")]
    public string? Description { get; set; }
}
