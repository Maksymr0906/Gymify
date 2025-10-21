using Gymify.Application.DTOs.Comment;

namespace Gymify.Application.Services.Interfaces;

public interface ICommentService
{
    Task CreateCommentAsync(CreateCommentRequestDto model);
    Task DeleteCommentByIdAsync(Guid commentId);
}
