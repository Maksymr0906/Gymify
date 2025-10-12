using Gymify.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Gymify.Web.Seed;

public class IdentitySeeder
{
    public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole<Guid>> roleManager,
                                           UserManager<ApplicationUser> userManager)
    {
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        var admin = await userManager.FindByEmailAsync("example1@gmail.com");
        if (admin != null && !await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");

        var user = await userManager.FindByEmailAsync("example2@gmail.com");
        if (user != null && !await userManager.IsInRoleAsync(user, "User"))
            await userManager.AddToRoleAsync(user, "User");
    }
}
