using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class CaseRepository(GymifyDbContext context)
    : Repository<Case>(context), ICaseRepository
{
}
