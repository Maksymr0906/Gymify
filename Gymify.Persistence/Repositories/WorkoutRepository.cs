using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class WorkoutRepository(GymifyDbContext context)
    : Repository<Workout>(context), IWorkoutRepository
{
}
