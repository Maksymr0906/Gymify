using Gymify.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

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

        var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
        if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin") && !await userManager.IsInRoleAsync(adminUser, "User"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            await userManager.AddToRoleAsync(adminUser, "User");
        }

        var user = await userManager.FindByEmailAsync("user@gmail.com");
        if (user != null && !await userManager.IsInRoleAsync(user, "User"))
            await userManager.AddToRoleAsync(user, "User");
    }
}
