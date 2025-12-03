using Gymify.Data.Entities;
using Gymify.Data.Enums;
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
           .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(i => i.NameEn)
           .IsRequired()
           .HasMaxLength(100);

        builder.Property(i => i.DescriptionEn)
            .HasMaxLength(500);
        
        builder.Property(i => i.NameUk)
           .IsRequired()
           .HasMaxLength(100);

        builder.Property(i => i.DescriptionUk)
            .HasMaxLength(500);

        builder.Property(i => i.ImageURL)
            .HasMaxLength(255);

        builder.HasData(
            _seedDataOptions.Items.Select(i => new Item
            {
                Id = i.Id,
                CreatedAt = i.CreatedAt,
                NameEn = i.NameEn,
                DescriptionEn = i.DescriptionEn,
                NameUk = i.NameUk,
                DescriptionUk = i.DescriptionUk,
                Type = (ItemType)i.Type,
                Rarity = (ItemRarity)i.Rarity,
                ImageURL = i.ImageURL
            })
        );
    }
}
