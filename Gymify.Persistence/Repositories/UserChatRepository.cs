using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserChatRepository(GymifyDbContext context) : IUserChatRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task CreateAsync(UserChat userChat)
    {
        await _context.UserChats.AddAsync(userChat);
    }

    public Task DeleteAsync(UserChat userChat)
    {
        _context.UserChats.Remove(userChat);
        return Task.CompletedTask;
    }

    public async Task<UserChat?> GetByChatAndUserAsync(Guid chatId, Guid userId)
    {
        return await _context.UserChats
            .FirstOrDefaultAsync(uc => uc.ChatId == chatId && uc.UserProfileId == userId);
    }

    public async Task<List<UserChat>> GetUserChatsAsync(Guid userId)
    {
        return await _context.UserChats
            .Include(uc => uc.Chat) // Підтягуємо сам чат
            .Where(uc => uc.UserProfileId == userId)
            .ToListAsync();
    }
}