using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IWorkoutRepository : IRepository<Workout>
{
    Task<Workout> GetByIdWithDetailsAsync(Guid id);
    Task<ICollection<Workout>> GetAllByUserIdAsync(Guid userId);
    Task<ICollection<Workout>> GetLastWorkouts(Guid userId, int count = 4);
    Task<DateTime?> GetFirstWorkoutDateAsync(Guid userId, bool onlyMy, string? authorName);
    Task<ICollection<Workout>> GetUserWorkoutsFilteredAsync(Guid userId, DateTime startDate, DateTime endDate, string? authorName, bool onlyMy, bool byDescending);
    Task<ICollection<Workout>> GetWorkoutsPageAsync(
    Guid userId,
    string? authorName,
    bool onlyMy,
    bool byDescending,
    int page,
    int pageSize);
    IQueryable<Workout> GetWorkoutsQuery(
        Guid userId,
        string? authorName,
        bool onlyMy,
        bool byDescending);
}
