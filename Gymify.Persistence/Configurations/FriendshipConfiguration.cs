using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.ToTable("Friendships");

        // Композитний ключ
        builder.HasKey(f => new { f.UserProfileId1, f.UserProfileId2 });

        builder.Property(f => f.CreatedAt)
             .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // Зв'язок з User1
        builder.HasOne(f => f.UserProfile1)
            .WithMany() // Можна додати колекцію Friendships в UserProfile
            .HasForeignKey(f => f.UserProfileId1)
            .OnDelete(DeleteBehavior.Restrict);

        // Зв'язок з User2
        builder.HasOne(f => f.UserProfile2)
            .WithMany()
            .HasForeignKey(f => f.UserProfileId2)
            .OnDelete(DeleteBehavior.Restrict);

        // Зв'язок з Чатом (1 Дружба = 1 Чат)
        builder.HasOne(f => f.Chat)
            .WithOne()
            .HasForeignKey<Friendship>(f => f.ChatId)
            .OnDelete(DeleteBehavior.Cascade); // Видаляємо дружбу -> видаляємо чат (опціонально)
    }
}