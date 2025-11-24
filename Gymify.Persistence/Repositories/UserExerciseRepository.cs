using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserExerciseRepository(GymifyDbContext context)
    : Repository<UserExercise>(context), IUserExerciseRepository
{
    private readonly GymifyDbContext _context = context;
    public async Task<ICollection<UserExercise>> GetAllByWorkoutIdAsync(Guid workoutId)
    {
        return await Entities.Where(w => w.WorkoutId == workoutId).ToListAsync();
    }
    public async Task DeleteRangeAsync(ICollection<UserExercise> exercises)
    {
        if (exercises == null || exercises.Count == 0)
            return;

        Entities.RemoveRange(exercises);
        await _context.SaveChangesAsync();
    }
}
