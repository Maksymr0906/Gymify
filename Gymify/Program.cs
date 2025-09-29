using Gymify.Persistence;
using Gymify.Persistence.SeedData;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<AuthorizationOptions>(configuration.GetSection("AuthorizationOptions"));
services.Configure<SeedDataOptions>(configuration.GetSection("SeedDataOptions"));

services.AddRazorPages();

services.AddPersistence(configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
