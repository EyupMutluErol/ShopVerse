using Microsoft.AspNetCore.Identity;
using ShopVerse.Entities.Abstract;

namespace ShopVerse.Entities.Concrete;

public class AppUser:IdentityUser,IEntity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string? City { get; set; }
}
