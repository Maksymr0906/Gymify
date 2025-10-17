using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class CaseItemConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<CaseItem>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<CaseItem> builder)
    {
        builder.HasKey(ui => new {ui.CaseId, ui.ItemId});

        builder.HasData(_seedDataOptions.CaseItems);
    }
}
