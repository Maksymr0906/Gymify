using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Persistence.SeedData.Models;

public record CaseItemSeedData
{
    public required Guid CaseId { get; init; }
    public required Guid ItemId { get; init; }
}
