using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class ExerciseConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<Exercise>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.Name)
           .IsRequired()
           .HasMaxLength(100);

        builder.Property(e => e.Type)
           .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.VideoURL)
            .HasMaxLength(255);

        builder.Property(e => e.BaseXP)
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(e => e.DifficultyMultiplier)
            .IsRequired();

        builder.HasData(
            _seedDataOptions.Exercises.Select(e => new Exercise
            {
                Id = e.Id,
                CreatedAt = e.CreatedAt,
                Name = e.Name,
                BaseXP = e.BaseXP,
                Description = e.Description,
                VideoURL = e.VideoURL,
                DifficultyMultiplier = e.DifficultyMultiplier,
                Type = (ExerciseType)e.Type,
                IsApproved = e.IsApproved
            })
        );
    }
}
