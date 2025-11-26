using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserExerciseRepository : IRepository<UserExercise>
{
    Task<ICollection<UserExercise>> GetAllByWorkoutIdAsync(Guid workoutId);
    Task DeleteRangeAsync(ICollection<UserExercise> exercises);
}
