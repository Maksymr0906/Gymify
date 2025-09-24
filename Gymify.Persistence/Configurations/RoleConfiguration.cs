using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class RoleConfiguration
    : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        var roles = Enum.GetValues<RoleType>()
            .Select(r => new Role
            {
                Id = (int)r,
                Name = r.ToString(),
            });

        builder.HasData(roles);
    }
}
