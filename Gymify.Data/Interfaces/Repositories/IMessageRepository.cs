using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IMessageRepository : IRepository<Message>
{
    Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, int skip = 0, int take = 50);
}
