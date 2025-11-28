using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class FriendInviteRepository(GymifyDbContext context) : IFriendInviteRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task CreateAsync(FriendInvite invite)
    {
        await _context.FriendInvites.AddAsync(invite);
    }

    public Task DeleteAsync(FriendInvite invite)
    {
        _context.FriendInvites.Remove(invite);
        return Task.CompletedTask;
    }

    public async Task<FriendInvite?> GetInviteAsync(Guid senderId, Guid receiverId)
    {
        return await _context.FriendInvites
            .FirstOrDefaultAsync(i => i.SenderProfileId == senderId && i.ReceiverProfileId == receiverId);
    }

    public async Task<FriendInvite?> GetInviteAnyDirectionAsync(Guid user1Id, Guid user2Id)
    {
        return await _context.FriendInvites
            .FirstOrDefaultAsync(i =>
                (i.SenderProfileId == user1Id && i.ReceiverProfileId == user2Id) ||
                (i.SenderProfileId == user2Id && i.ReceiverProfileId == user1Id));
    }

    public async Task<List<FriendInvite>> GetIncomingInvitesAsync(Guid receiverId)
    {
        return await _context.FriendInvites
            .Include(i => i.SenderProfile)
                .ThenInclude(p => p.ApplicationUser) 
            .Include(i => i.SenderProfile)
                .ThenInclude(p => p.Equipment).ThenInclude(e => e.Avatar) 
            .Where(i => i.ReceiverProfileId == receiverId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }
}