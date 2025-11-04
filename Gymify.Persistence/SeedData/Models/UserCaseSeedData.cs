using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Persistence.SeedData.Models
{
    public class UserCaseSeedData
    {
        public required Guid Id { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required Guid UserProfileId { get; init; }
        public required Guid CaseId { get; init; }
    }
}
