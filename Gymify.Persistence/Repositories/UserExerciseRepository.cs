using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class UserExerciseRepository(GymifyDbContext context)
    : Repository<UserExercise>(context), IUserExerciseRepository
{
}
