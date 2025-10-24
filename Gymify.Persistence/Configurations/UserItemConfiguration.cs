using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class UserItemConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<UserItem>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<UserItem> builder)
    {
        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasData(
            _seedDataOptions.UserItems.Select(uc => new UserItem
            {
                Id = uc.Id,
                CreatedAt = uc.CreatedAt,
                UserProfileId = uc.UserProfileId,
                ItemId = uc.ItemId
            })
        );
    }
}
