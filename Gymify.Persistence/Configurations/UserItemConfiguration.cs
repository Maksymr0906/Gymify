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
        builder.HasKey(ui => new {ui.UserId, ui.ItemId});

        builder.HasData(_seedDataOptions.UserItems);
    }
}
