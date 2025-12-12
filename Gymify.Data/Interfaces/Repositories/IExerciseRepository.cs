using Gymify.Data.Entities;
using Gymify.Data.Enums;

namespace Gymify.Data.Interfaces.Repositories;

public interface IExerciseRepository : IRepository<Exercise>
{
    Task<Exercise> GetByNameAsync(string name, bool ukranianVer);
    Task<IEnumerable<Exercise>> FindByNameAsync(string name, bool ukranianVer);
    Task<(List<Exercise> Exercises, int TotalCount)> GetFilteredAsync(
        string? search,
        ExerciseType? type,
        bool pendingOnly,
        int page,
        int pageSize,
        bool ukranianVer);
    Task<IEnumerable<Exercise>> GetUnapprovedAsync();
}
