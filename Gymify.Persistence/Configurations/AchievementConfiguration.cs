using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class AchievementConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<Achievement>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.IconUrl)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.TargetProperty)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.ComparisonType)
            .IsRequired()
            .HasMaxLength(5);

        builder.HasIndex(a => a.Name).IsUnique();

        builder.HasData(_seedDataOptions.Achievements.Select(a => new Achievement
        {
            Id = a.Id,
            CreatedAt = a.CreatedAt,
            Name = a.Name,
            Description = a.Description,
            IconUrl = a.IconUrl,
            TargetProperty = a.TargetProperty,
            TargetValue = a.TargetValue,
            ComparisonType = a.ComparisonType,
            RewardItemId = a.RewardItemId
        }));
    }
}
