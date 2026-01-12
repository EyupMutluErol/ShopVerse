namespace ShopVerse.WebUI.Areas.Admin.Models;

public class RoleAssignViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public List<RoleAssignItem> Roles { get; set; }
}

public class RoleAssignItem
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public bool Exists { get; set; } // Kullanıcı bu role sahip mi ?
}
