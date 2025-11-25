using Gymify.Application.DTOs.Comment;
using Gymify.Data.Enums;

namespace Gymify.Application.Services.Interfaces;

public interface ICommentService
{
    Task<CommentDto> UploadComment(Guid userId, Guid targetId, CommentTargetType targetType, string content);
    Task CreateCommentAsync(CommentDto model);
    Task DeleteCommentByIdAsync(Guid commentId);
}
