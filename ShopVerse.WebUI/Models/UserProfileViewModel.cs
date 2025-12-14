using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebUI.Models;

public class UserProfileViewModel
{
    public AppUser User { get; set; }
    public List<Order> Orders { get; set; }
}
