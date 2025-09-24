using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class UserRoleConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<UserRole>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(r => new {r.UserId, r.RoleId});

        builder.HasData(_seedDataOptions.UserRoles);
    }
}
