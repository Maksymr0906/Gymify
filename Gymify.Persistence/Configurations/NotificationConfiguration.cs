using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class NotificationConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<Notification>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(n => n.Type)
            .IsRequired();

        builder.HasData(
            _seedDataOptions.Notifications.Select(n => new Notification
            {
                Id = n.Id,
                CreatedAt = n.CreatedAt,
                Content = n.Content,
                Type = (NotificationType)n.Type,
                UserProfileId = n.UserProfileId,
            })
        );
    }
}
