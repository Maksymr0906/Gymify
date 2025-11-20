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

    public async Task<ICollection<Workout>> GetLastWorkouts(Guid userId, int count = 4)
    {
        return await _context.Workouts
            .Where(w => w.UserProfileId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<DateTime?> GetFirstWorkoutDateAsync(Guid userId, bool onlyMy, string? authorName)
    {
        var query = Entities
            .Include(w => w.UserProfile)
            .ThenInclude(up => up.ApplicationUser)
            .AsQueryable();

        if (onlyMy)
        {
            query = query.Where(w => w.UserProfileId == userId);
        }
        else 
        {
            query = query.Where(w => w.IsPrivate == false);

            if (!string.IsNullOrWhiteSpace(authorName))
            {
                var loweredName = authorName.Trim().ToLower();
                query = query.Where(w => w.UserProfile.ApplicationUser.UserName.ToLower().Contains(loweredName));
            }
        }

        var firstWorkoutDate = await query
            .OrderBy(w => w.CreatedAt)
            .Select(w => (DateTime?)w.CreatedAt.Date)
            .FirstOrDefaultAsync();

        return firstWorkoutDate;
    }

    public async Task<ICollection<Workout>> GetUserWorkoutsFilteredAsync
        (Guid userId, DateTime startDate, DateTime endDate, 
        string? authorName, bool onlyMy, bool byDescending)
    {
        var query = Entities
            .Include(w => w.UserProfile)
                .ThenInclude(up => up.ApplicationUser)
            .Where(w => w.CreatedAt >= startDate && w.CreatedAt <= endDate);

        if (onlyMy)
        {
            query = query.Where(w => w.UserProfileId == userId);
        }
        else
        {
            query = query.Where(w => w.IsPrivate == false);

            if (!string.IsNullOrWhiteSpace(authorName))
            {
                var loweredName = authorName.Trim().ToLower();
                query = query.Where(w => w.UserProfile.ApplicationUser.UserName.ToLower().Contains(loweredName));
            }
        }

        if (byDescending)
        {
            query = query.OrderByDescending(w => w.CreatedAt);
        }
        else
        {
            query = query.OrderBy(w => w.CreatedAt);
        }

        return await query.ToListAsync();
    }

    public IQueryable<Workout> GetWorkoutsQuery(
        Guid userId,
        string? authorName,
        bool onlyMy,
        bool byDescending)
    {
        // 1. Ініціалізація та Include
        var query = Entities
            .Include(w => w.UserProfile)
                .ThenInclude(up => up.ApplicationUser)
            .AsQueryable();

        // 2. Фільтрація за користувачем та автором
        if (onlyMy)
        {
            query = query.Where(w => w.UserProfileId == userId);
        }
        else // Показуємо всі публічні, або фільтруємо по автору
        {
            query = query.Where(w => w.IsPrivate == false);

            if (!string.IsNullOrWhiteSpace(authorName))
            {
                var loweredName = authorName.Trim().ToLower();
                query = query.Where(w => w.UserProfile.ApplicationUser.UserName.ToLower().Contains(loweredName));
            }
        }

        // 3. Сортування
        if (byDescending)
        {
            query = query.OrderByDescending(w => w.CreatedAt);
        }
        else
        {
            query = query.OrderBy(w => w.CreatedAt);
        }

        return query;
    }

    /**
     * Асинхронний метод для виконання запиту з пагінацією Skip/Take.
     */
    public async Task<ICollection<Workout>> GetWorkoutsPageAsync(
        Guid userId,
        string? authorName,
        bool onlyMy,
        bool byDescending,
        int page,
        int pageSize)
    {
        // Отримуємо базовий запит
        var query = GetWorkoutsQuery(userId, authorName, onlyMy, byDescending);

        // Застосовуємо пагінацію Skip/Take
        return await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
