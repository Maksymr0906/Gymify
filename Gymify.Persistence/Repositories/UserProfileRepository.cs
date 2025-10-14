using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class UserProfileRepository(GymifyDbContext context)
    : Repository<UserProfile>(context), IUserProfileRepository
{
}
