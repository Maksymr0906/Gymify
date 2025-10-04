using Gymify.Application.Extensions;
using Gymify.Persistence;
using Gymify.Persistence.SeedData;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<AuthorizationOptions>(configuration.GetSection("AuthorizationOptions"));
services.Configure<SeedDataOptions>(configuration.GetSection("SeedDataOptions"));

services.AddRazorPages();

services
    .AddPersistence(configuration)
    .AddApplication();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
});

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
