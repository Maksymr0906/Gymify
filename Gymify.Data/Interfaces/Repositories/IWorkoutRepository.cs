using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IWorkoutRepository : IRepository<Workout>
{
    Task<Workout> GetByIdWithDetailsAsync(Guid id);
    Task<ICollection<Workout>> GetAllByUserIdAsync(Guid userId);
    Task<List<Workout>> GetLastWorkouts(Guid userId);
    Task<DateTime?> GetFirstWorkoutDateAsync(Guid userId, bool onlyMy, string? authorName);
    Task<ICollection<Workout>> GetUserWorkoutsFilteredAsync(Guid userId, DateTime startDate, DateTime endDate, string? authorName, bool onlyMy, bool byDescending);
}
