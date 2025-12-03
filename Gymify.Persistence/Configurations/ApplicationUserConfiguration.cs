using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public class ApplicationUserConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<ApplicationUser>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasData(_seedDataOptions.ApplicationUsers);
    }
}
