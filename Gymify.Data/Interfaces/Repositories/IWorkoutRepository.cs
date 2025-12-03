using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IWorkoutRepository : IRepository<Workout>
{
    Task<Workout> GetByIdWithDetailsAsync(Guid id);
    Task<ICollection<Workout>> GetAllByUserIdAsync(Guid userId);
    Task<ICollection<Workout>> GetLastWorkouts(Guid userId, int count = 6);
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
