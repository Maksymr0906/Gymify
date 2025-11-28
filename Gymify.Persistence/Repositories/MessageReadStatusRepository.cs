using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class MessageReadStatusRepository(GymifyDbContext context) : IMessageReadStatusRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task CreateAsync(MessageReadStatus status)
    {
        if (!await _context.MessageReadStatuses.AnyAsync(s => s.MessageId == status.MessageId && s.UserProfileId == status.UserProfileId))
        {
            await _context.MessageReadStatuses.AddAsync(status);
        }
    }

    public async Task<bool> IsReadAsync(Guid messageId, Guid userId)
    {
        return await _context.MessageReadStatuses
            .AnyAsync(s => s.MessageId == messageId && s.UserProfileId == userId);
    }
}
