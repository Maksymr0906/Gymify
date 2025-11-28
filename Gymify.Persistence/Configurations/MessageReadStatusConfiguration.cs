using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MessageReadStatusConfiguration : IEntityTypeConfiguration<MessageReadStatus>
{
    public void Configure(EntityTypeBuilder<MessageReadStatus> builder)
    {
        // Унікальність: один юзер може прочитати повідомлення тільки один раз
        builder.HasKey(mrs => new { mrs.MessageId, mrs.UserProfileId });

        builder.HasOne(mrs => mrs.Message)
            .WithMany(m => m.ReadStatuses)
            .HasForeignKey(mrs => mrs.MessageId)
            .OnDelete(DeleteBehavior.Cascade); // Якщо видалили повідомлення - статус теж зникає

        builder.HasOne(mrs => mrs.UserProfile)
            .WithMany() // Можна додати навігацію в UserProfile, якщо треба
            .HasForeignKey(mrs => mrs.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}