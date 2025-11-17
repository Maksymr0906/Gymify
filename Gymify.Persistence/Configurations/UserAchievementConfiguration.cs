using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class UserAchievementConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<UserAchievement>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.HasKey(ua => new { ua.UserProfileId, ua.AchievementId });

        builder.Property(ua => ua.Progress)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ua => ua.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ua => ua.UnlockedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasData(_seedDataOptions.UserAchievements);
    }
}
