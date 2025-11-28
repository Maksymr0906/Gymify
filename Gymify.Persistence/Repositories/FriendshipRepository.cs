using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class FriendshipRepository(GymifyDbContext context) : IFriendshipRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task CreateAsync(Friendship friendship)
    {
        await _context.Friendships.AddAsync(friendship);
    }

    public Task DeleteAsync(Friendship friendship)
    {
        _context.Friendships.Remove(friendship);
        return Task.CompletedTask;
    }

    public async Task<Friendship?> GetByUsersAsync(Guid user1Id, Guid user2Id)
    {
        return await _context.Friendships
            .FirstOrDefaultAsync(f =>
                (f.UserProfileId1 == user1Id && f.UserProfileId2 == user2Id) ||
                (f.UserProfileId1 == user2Id && f.UserProfileId2 == user1Id));
    }
}