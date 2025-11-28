using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("Chats");

        builder.Property(c => c.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired(false);

        // Налаштування One-to-One (необов'язкове) для LastMessage
        // Якщо повідомлення видаляється, в чаті поле LastMessageId стає null
        builder.HasOne(c => c.LastMessage)
            .WithOne()
            .HasForeignKey<Chat>(c => c.LastMessageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}