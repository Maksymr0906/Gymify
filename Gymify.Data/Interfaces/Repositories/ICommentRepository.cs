using Gymify.Data.Entities;
using Gymify.Data.Enums;

namespace Gymify.Data.Interfaces.Repositories;

public interface ICommentRepository : IRepository<Comment>
{
    Task<ICollection<Comment>> GetCommentsByTargetIdAndTypeAsync(Guid targetId, CommentTargetType targetType);
    Task<ICollection<Comment>> GetUnapprovedAsync();
}
