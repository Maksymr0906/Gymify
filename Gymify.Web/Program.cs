using Gymify.Application.Extensions;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Persistence;
using Gymify.Persistence.SeedData;
using Gymify.Web.Hubs;
using Gymify.Web.Seed;
using Gymify.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("uk")
    };

    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;

    options.RequestCultureProviders.Insert(0, new Microsoft.AspNetCore.Localization.QueryStringRequestCultureProvider());
});


services.Configure<SeedDataOptions>(configuration.GetSection("SeedDataOptions"));

services
    .AddPersistence(configuration)
    .AddApplication();

services.AddSignalR();
services.AddSingleton<IUserIdProvider, CustomUserIdProviderService>();


services.AddScoped<INotifierService, SignalRNotifierService>();

services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<GymifyDbContext>()
.AddDefaultTokenProviders();

services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.LogoutPath = "/Logout";
    options.AccessDeniedPath = "/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});
services.AddControllersWithViews()
        .AddViewLocalization()          
        .AddDataAnnotationsLocalization();  


var app = builder.Build(); 
var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()!.Value;
app.UseRequestLocalization(localizationOptions);

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await IdentitySeeder.SeedRolesAndAdminAsync(roleManager, userManager);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}"
);

app.MapHub<NotificationHub>("/notificationHub");

app.Run();
