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

    public async Task<ICollection<Workout>> GetUserWorkoutsFilteredAsync(Guid userId, DateTime startDate, DateTime endDate, string? authorName, bool onlyMy)
    {
        var query = Entities
            .Include(w => w.UserProfile)
                .ThenInclude(up => up.ApplicationUser)
            .Where(w => w.CreatedAt >= startDate && w.CreatedAt <= endDate);

        if (onlyMy)
        {
            query = query.Where(w => w.UserProfileId == userId);
        }
        else if (!string.IsNullOrWhiteSpace(authorName))
        {
            var loweredName = authorName.Trim().ToLower();
            query = query.Where(w => w.UserProfile.ApplicationUser.UserName.ToLower().Contains(loweredName));
        }

        return await query.OrderByDescending(w => w.CreatedAt).ToListAsync();
    }
}
