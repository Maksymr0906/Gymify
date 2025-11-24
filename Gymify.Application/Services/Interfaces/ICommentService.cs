using Gymify.Application.DTOs.Comment;

namespace Gymify.Application.Services.Interfaces;

public interface ICommentService
{
    Task CreateCommentAsync(CommentDto model);
    Task DeleteCommentByIdAsync(Guid commentId);
}
