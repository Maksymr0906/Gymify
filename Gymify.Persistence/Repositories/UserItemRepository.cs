using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserItemRepository(GymifyDbContext context) : IUserItemRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task<UserItem> CreateAsync(UserItem entity)
    {
        await _context.UserItems.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<UserItem>> GetAllAsync()
    {
        return await _context.UserItems
            .Include(uc => uc.Item)
            .Include(uc => uc.UserProfile)
            .ToListAsync();
    }

    public async Task<ICollection<UserItem>> GetAllByUserIdAsync(Guid userId)
    {
        var entities = await _context.UserItems
            .Include(uc => uc.Item)
            .Include(uc => uc.UserProfile)
            .Where(uc => uc.UserProfileId == userId)
            .ToListAsync();

        return entities;
    }

    public async Task<UserItem> UpdateAsync(UserItem entity)
    {
        _context.UserItems.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<UserItem> DeleteByIdAsync(Guid id)
    {
        var entity = await _context.UserItems.FirstOrDefaultAsync(uc => uc.ItemId == id);
        if (entity == null)
            throw new Exception("UserCase not found");

        _context.UserItems.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
