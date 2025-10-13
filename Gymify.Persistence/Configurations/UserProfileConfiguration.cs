using Gymify.Data.Entities;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class UserProfileConfiguration(SeedDataOptions seedDataOptions)
    : IEntityTypeConfiguration<UserProfile>
{
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions;

    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.Property(u => u.Level)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.CurrentXP)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasOne(p => p.ApplicationUser)
            .WithOne(u => u.UserProfile)
            .HasForeignKey<UserProfile>(p => p.ApplicationUserId);

        builder.HasData(_seedDataOptions.UserProfiles);
    }
}
