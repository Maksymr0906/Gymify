using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gymify.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<GymifyDbContext>(options =>
        {
            options.UseMySql(configuration.GetConnectionString("GymifyConnectionString"),
                new MySqlServerVersion(new Version(8, 3, 0)));
        });

        return services;
    }
}
