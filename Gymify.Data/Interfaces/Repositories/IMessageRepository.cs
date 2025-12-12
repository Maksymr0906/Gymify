using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Data.Interfaces.Repositories;

public interface IMessageRepository : IRepository<Message>
{
    Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, int skip = 0, int take = 50);
    Task<Message> FindLastMessageAsync(Guid chatId);
    Task<int> CountUnreadMessagesAsync(Guid chatId, Guid userId);
}
