using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public class PendingExerciseConfiguration : IEntityTypeConfiguration<PendingExercise>
{
    public void Configure(EntityTypeBuilder<PendingExercise> builder)
    {
        builder.Property(pe => pe.SubmittedAt)
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(pe => pe.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(pe => pe.Type)
               .IsRequired();

        builder.Property(pe => pe.Description)
               .HasMaxLength(500);

        builder.Property(pe => pe.VideoURL)
               .HasMaxLength(255);

        builder.Property(pe => pe.IsApproved)
               .IsRequired()
               .HasDefaultValue(false);

        builder.HasOne(pe => pe.SubmittedByUser)
               .WithMany()
               .HasForeignKey(pe => pe.SubmittedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pe => pe.Name)
               .IsUnique(false);
    }
}
