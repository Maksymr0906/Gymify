using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IExerciseRepository : IRepository<Exercise>
{
    Task<Exercise> GetByNameAsync(string name);
    Task<IEnumerable<Exercise>> FindByNameAsync(string name);
}
