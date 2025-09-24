using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class UserEquipmentConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<UserEquipment>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<UserEquipment> builder)
    {
        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("GETUTCDATE()");

        builder.HasData(_seedDataOptions.UserEquipments);
    }
}
