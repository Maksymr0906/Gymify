using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserAchievementRepository(GymifyDbContext context) : IUserAchievementRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task<UserAchievement> CreateAsync(UserAchievement entity)
    {
        await _context.UserAchievements.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<UserAchievement> DeleteByIdAsync(Guid id)
    {
        var entity = await _context.UserAchievements.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity with ID {id} not found.");
        }

        try
        {
            _context.UserAchievements.Remove(entity);
            return entity;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Error deleting entity: {ex.Message}", ex);
        }
    }

    public async Task<ICollection<UserAchievement>> GetAllAsync()
    {
        return await _context.UserAchievements
           .Include(ua => ua.UserProfile)
           .Include(ua => ua.Achievement)
           .ToListAsync();
    }

    public async Task<ICollection<UserAchievement>> GetAllByUserId(Guid userProfileId)
    {
        return await _context.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserProfileId == userProfileId)
            .ToListAsync();
    } 

    public async Task<UserAchievement> UpdateAsync(UserAchievement entity)
    {
        _context.UserAchievements.Update(entity);
        return entity;
    }

}
