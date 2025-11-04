using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserItemRepository(GymifyDbContext context) : Repository<UserItem>(context) , IUserItemRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task<ICollection<UserItem>> GetAllByUserIdAsync(Guid userId)
    {
        var entities = await _context.UserItems
            .Include(uc => uc.Item)
            .Include(uc => uc.UserProfile)
            .Where(uc => uc.UserProfileId == userId)
            .ToListAsync();

        return entities;
    }

    public async Task<UserItem> DeleteByItemIdAsync(Guid itemId)
    {
        var entity = await _context.UserItems.FirstOrDefaultAsync(uc => uc.ItemId == itemId);
        if (entity == null)
            throw new Exception("UserCase not found");

        _context.UserItems.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
