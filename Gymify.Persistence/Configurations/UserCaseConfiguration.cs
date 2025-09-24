using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gymify.Persistence.Configurations;

public partial class UserCaseConfiguration
    : IEntityTypeConfiguration<UserCase>
{
    public void Configure(EntityTypeBuilder<UserCase> builder)
    {
        builder.HasKey(uc => new {uc.UserId, uc.CaseId});
    }
}
