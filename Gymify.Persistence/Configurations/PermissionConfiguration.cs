using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class PermissionConfiguration
    : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        var permissions = Enum
               .GetValues<PermissionType>()
               .Select(p => new Permission
               {
                   Id = (int)p,
                   Name = p.ToString()
               });

        builder.HasData(permissions);
    }
}
