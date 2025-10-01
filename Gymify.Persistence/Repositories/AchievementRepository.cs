using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class AchievementRepository(GymifyDbContext context)
    : Repository<Achievement>(context), IAchievementRepository
{
}
