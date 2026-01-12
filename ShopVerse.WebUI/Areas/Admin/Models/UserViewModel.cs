namespace ShopVerse.WebUI.Areas.Admin.Models;

public class UserViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
}
