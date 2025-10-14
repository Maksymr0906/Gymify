using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class FriendshipConfiguration
    : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(f => new { f.UserProfileId1, f.UserProfileId2 });

        builder.HasOne(f => f.UserProfile1)
            .WithMany(u => u.Friendships1)
            .HasForeignKey(f => f.UserProfileId1)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.UserProfile2)
            .WithMany(u => u.Friendships2)
            .HasForeignKey(f => f.UserProfileId2)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
