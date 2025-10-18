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
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

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

        builder.HasData(_seedDataOptions.Achievements);
    }
}
