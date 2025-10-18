using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class UserProfileConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<UserProfile>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Level)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.CurrentXP)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.TotalWorkouts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.WorkoutStreak)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.TotalWeightLifted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.TotalKmRunned)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.StrengthExercisesCompleted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.CardioExercisesCompleted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.FlexibilityExercisesCompleted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.BalanceExercisesCompleted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.EnduranceExercisesCompleted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.MobilityExercisesCompleted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasOne(p => p.ApplicationUser)
            .WithOne(u => u.UserProfile)
            .HasForeignKey<UserProfile>(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(_seedDataOptions.UserProfiles);
    }
}
