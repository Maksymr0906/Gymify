using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IWorkoutRepository : IRepository<Workout>
{
    Task<Workout> GetByIdWithDetailsAsync(Guid id);
    Task<ICollection<Workout>> GetAllByUserIdAsync(Guid userId);
    Task<ICollection<Workout>> GetAllUserWorkoutsInDateRange(Guid userId, DateTime startDate, DateTime endDate);
}
