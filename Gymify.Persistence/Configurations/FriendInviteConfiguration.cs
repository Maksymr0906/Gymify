using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public class FriendInviteConfiguration : IEntityTypeConfiguration<FriendInvite>
{
    public void Configure(EntityTypeBuilder<FriendInvite> builder)
    {
        builder.ToTable("FriendInvites");

        // ❗ ГОЛОВНА ЗМІНА: Композитний ключ
        // Це гарантує, що А не може надіслати Б два інвайти одночасно
        builder.HasKey(fi => new { fi.SenderProfileId, fi.ReceiverProfileId });

        builder.Property(fi => fi.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasOne(fi => fi.SenderProfile)
            .WithMany()
            .HasForeignKey(fi => fi.SenderProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fi => fi.ReceiverProfile)
            .WithMany()
            .HasForeignKey(fi => fi.ReceiverProfileId)
            .OnDelete(DeleteBehavior.Cascade); // Якщо видаляють отримувача, інвайт зникає
    }
}