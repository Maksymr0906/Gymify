using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class FriendInviteConfiguration
    : IEntityTypeConfiguration<FriendInvite>
{
    public void Configure(EntityTypeBuilder<FriendInvite> builder)
    {
        builder.HasKey(fi => new { fi.SenderId, fi.ReceiverId });

        builder.Property(fi => fi.SentAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(fi => fi.Status)
            .IsRequired()
            .HasDefaultValue(InviteStatus.Pending);

        builder.HasOne(fi => fi.Sender)
            .WithMany(u => u.SentFriendInvites)
            .HasForeignKey(fi => fi.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fi => fi.Receiver)
            .WithMany(u => u.ReceivedFriendInvites)
            .HasForeignKey(fi => fi.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}