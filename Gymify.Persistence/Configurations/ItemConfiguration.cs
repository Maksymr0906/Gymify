using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class ItemConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<Item>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(i => i.Name)
           .IsRequired()
           .HasMaxLength(100);

        builder.Property(i => i.Description)
            .HasMaxLength(500);

        builder.Property(i => i.ImageURL)
            .HasMaxLength(255);

        builder.HasData(_seedDataOptions.Items);
    }
}
