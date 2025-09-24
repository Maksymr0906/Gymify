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
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(fi => fi.Status)
            .IsRequired()
            .HasDefaultValue(InviteStatus.Pending);
    }
}