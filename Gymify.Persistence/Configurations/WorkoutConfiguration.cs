using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class WorkoutConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<Workout>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Description)
            .HasMaxLength(500);

        builder.Property(w => w.Conclusion)
            .HasMaxLength(500);

        builder.Property(w => w.IsPrivate)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasData(_seedDataOptions.Workouts);
    }
}
