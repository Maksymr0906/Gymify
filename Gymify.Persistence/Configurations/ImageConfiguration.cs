using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class ImageConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<Image>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasData(_seedDataOptions.Images);
    }
}
