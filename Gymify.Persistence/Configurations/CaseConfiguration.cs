using Gymify.Data.Enums;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class CaseConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<Case>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.ImageUrl)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Type)
            .IsRequired();

        builder.HasData(_seedDataOptions.Cases.Select(c => new Case
        {
            Id = c.Id,
            CreatedAt = c.CreatedAt,
            Name = c.Name,
            Description = c.Description,
            ImageUrl = c.ImageUrl,
            Type = (CaseType)c.CaseType
        }));
    }
}
