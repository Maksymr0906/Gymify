using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class WorkoutRepository(GymifyDbContext context)
    : Repository<Workout>(context), IWorkoutRepository
{
    public async Task<ICollection<Workout>> GetAllByUserIdAsync(Guid userId)
    {
        return await Entities.Where(w => w.UserProfileId == userId).ToListAsync();
    }

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
}
