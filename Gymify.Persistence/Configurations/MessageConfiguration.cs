using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gymify.Persistence.Configurations;

public partial class MessageConfiguration
    : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(m => m.IsRead)
            .IsRequired();

        builder.HasOne(m => m.SenderProfile)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.ReceiverProfile)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
