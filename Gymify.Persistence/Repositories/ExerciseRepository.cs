using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class ExerciseRepository(GymifyDbContext context)
    : Repository<Exercise>(context), IExerciseRepository
{
}
