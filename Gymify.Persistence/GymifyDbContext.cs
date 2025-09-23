using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gymify.Persistence;

public class GymifyDbContext(
    DbContextOptions<GymifyDbContext> options,
    IOptions<AuthorizationOptions> authOptions,
    IOptions<SeedDataOptions> seedDataOptions)
    : DbContext(options)
{
    private readonly AuthorizationOptions _authorizationOptions = authOptions.Value;
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions.Value;


}
