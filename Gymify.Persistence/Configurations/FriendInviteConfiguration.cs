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
        builder.HasKey(fi => new { fi.SenderProfileId, fi.ReceiverProfileId });

        builder.Property(fi => fi.SentAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(fi => fi.Status)
            .IsRequired()
            .HasDefaultValue(InviteStatus.Pending);

        builder.HasOne(fi => fi.SenderProfile)
            .WithMany(u => u.SentFriendInvites)
            .HasForeignKey(fi => fi.SenderProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fi => fi.ReceiverProfile)
            .WithMany(u => u.ReceivedFriendInvites)
            .HasForeignKey(fi => fi.ReceiverProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}