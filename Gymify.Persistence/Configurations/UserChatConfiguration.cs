using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public class UserChatConfiguration : IEntityTypeConfiguration<UserChat>
{
    public void Configure(EntityTypeBuilder<UserChat> builder)
    {
        builder.ToTable("UserChats");

        builder.HasKey(uc => new { uc.ChatId, uc.UserProfileId });

        builder.Property(uc => uc.JoinedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasOne(uc => uc.Chat)
            .WithMany(c => c.Members)
            .HasForeignKey(uc => uc.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uc => uc.UserProfile)
            .WithMany() // Можна додати UserChats в UserProfile
            .HasForeignKey(uc => uc.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}