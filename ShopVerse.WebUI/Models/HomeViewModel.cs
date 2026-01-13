using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebUI.Models;

public class HomeViewModel
{
    public List<Product> FeaturedProducts { get; set; }
    public List<Category> Categories { get; set; }
    public List<Campaign> ActiveCampaigns { get; set; }
}
