using Microsoft.AspNetCore.Identity;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebUI.Utils;

public static class DataSeeder
{
    public static async Task SeedAdminAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new AppRole
            {
                Name = "Admin",
                Description = "Yönetici rolü, tüm yetkilere sahiptir."
            });
        }

        if (!await roleManager.RoleExistsAsync("Member"))
        {
            await roleManager.CreateAsync(new AppRole
            {
                Name = "Member",
                Description = "Standart kullanıcı rolü."
            });
        }

        var adminUser = await userManager.FindByEmailAsync("admin@shopverse.com");

        if (adminUser == null)
        {
            var newAdmin = new AppUser
            {
                UserName = "Admin",
                Email = "admin@shopverse.com",
                Name = "System",
                Surname = "Admin",
                ImageUrl = "admin-avatar.png"
            };

            var result = await userManager.CreateAsync(newAdmin, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }
}
