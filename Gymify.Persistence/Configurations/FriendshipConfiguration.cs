using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class FriendshipConfiguration
    : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(f => new { f.UserId1, f.UserId2 });

        builder.HasOne(f => f.User1)
            .WithMany(u => u.Friendships1)
            .HasForeignKey(f => f.UserId1)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.User2)
            .WithMany(u => u.Friendships2)
            .HasForeignKey(f => f.UserId2)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
