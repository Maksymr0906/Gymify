using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Gymify.Persistence.Configurations;

public partial class UserCaseConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<UserCase>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<UserCase> builder)
    {
        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasData(
            _seedDataOptions.UserCases.Select(uc => new UserCase
            {
                Id = uc.Id,
                CreatedAt = uc.CreatedAt,
                UserProfileId = uc.UserProfileId,
                CaseId = uc.CaseId
            })
        );
    }
}
