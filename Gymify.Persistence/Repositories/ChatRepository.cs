using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class ChatRepository(GymifyDbContext context)
    : Repository<Chat>(context), IChatRepository
{
}
