using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class UserAchievementConfiguration
    : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.HasKey(ua => new { ua.UserId, ua.AchievementId });

        builder.Property(ua => ua.Progress)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ua => ua.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ua => ua.UnlockedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");   
    }
}
