using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class MessageRepository(GymifyDbContext context)
    : Repository<Message>(context), IMessageRepository
{
}
