using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class UserExerciseConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<UserExercise>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<UserExercise> builder)
    {
        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(ue => ue.NameEn)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ue => ue.NameUk)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ue => ue.Type)
            .IsRequired();

        builder.Property(ue => ue.Duration)
               .HasConversion<TimeSpan>()
               .HasDefaultValue(null);

        builder.Property(ue => ue.EarnedXP)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasData(
            _seedDataOptions.UserExercises.Select(ue => new UserExercise
            {
                Id = ue.Id,
                CreatedAt = ue.CreatedAt,
                NameEn = ue.NameEn,
                NameUk = ue.NameUk,
                Duration = ue.Duration,
                EarnedXP = ue.EarnedXP,
                Reps = ue.Reps,
                Sets = ue.Sets,
                Weight = ue.Weight,
                WorkoutId = ue.WorkoutId,
                Type = (ExerciseType)ue.Type
            })
        );
    }
}
