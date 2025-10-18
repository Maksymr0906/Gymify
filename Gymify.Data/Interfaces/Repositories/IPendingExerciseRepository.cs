using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IPendingExerciseRepository : IRepository<PendingExercise>
{
    Task<PendingExercise> GetByNameAsync(string name);
}
