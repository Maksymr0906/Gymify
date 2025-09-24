using Gymify.Data.Entities;
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
           .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(ue => ue.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ue => ue.Sets)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ue => ue.Reps)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ue => ue.Weight)
            .HasDefaultValue(0);

        builder.Property(ue => ue.EarnedXP)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasData(_seedDataOptions.UserExercises);
    }
}
