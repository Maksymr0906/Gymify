using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(c => c.TargetType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.TargetId)
            .IsRequired();

        builder.HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.UserProfile)
            .WithMany(u => u.ReceivedComments)
            .HasForeignKey(c => c.TargetId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(c => c.Workout)
            .WithMany(w => w.Comments)
            .HasForeignKey(c => c.TargetId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
