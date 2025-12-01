using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class MessageRepository(GymifyDbContext context)
    : Repository<Message>(context), IMessageRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, int skip = 0, int take = 50)
    {
        return await _context.Messages
            .AsNoTracking()
            .Where(m => m.ChatId == chatId)
            .Include(m => m.Sender)
                .ThenInclude(u => u.ApplicationUser)
            .Include(m => m.Sender.Equipment.Avatar)
            .OrderByDescending(m => m.CreatedAt) // Спочатку нові (для пагінації вверх)
            .Skip(skip)
            .Take(take)
            .ToListAsync(); // Повернемо в зворотньому порядку на клієнті або тут
    }

    public async Task<Message> FindLastMessageAsync(Guid chatId)
    {
        return await Entities
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();
    }
}
