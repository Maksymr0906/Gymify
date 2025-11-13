using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class WorkoutRepository(GymifyDbContext context)
    : Repository<Workout>(context), IWorkoutRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task<Workout> GetByIdWithDetailsAsync(Guid id)
    {
        var workout = await Entities
            .Include(w => w.Exercises)           
            .Include(w => w.UserProfile)         
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workout == null)
            throw new Exception("Workout not found");

        return workout;
    }
    public async Task<ICollection<Workout>> GetAllByUserIdAsync(Guid userId)
    {
        return await Entities.Where(w => w.UserProfileId == userId).ToListAsync();
    }

    public async Task<ICollection<Workout>> GetAllUserWorkoutsInDateRange(Guid userId, DateTime startDate, DateTime endDate)
    {
        var workouts = await _context.Workouts
            .Where(w => w.UserProfileId == userId &&
                        w.CreatedAt >= startDate && 
                        w.CreatedAt <= endDate)    
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();

        return workouts;
    }
}
