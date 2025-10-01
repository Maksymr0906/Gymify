using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class CommentRepository(GymifyDbContext context)
    : Repository<Comment>(context), ICommentRepository
{
}
