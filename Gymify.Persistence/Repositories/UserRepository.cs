using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class UserRepository(GymifyDbContext context)
    : Repository<User>(context), IUserRepository
{
}
