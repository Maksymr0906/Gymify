using Gymify.Application.DTOs.Comment;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class CommentService(IUnitOfWork unitOfWork) : ICommentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CommentDto> UploadComment(Guid userId, Guid targetId, CommentTargetType targetType, string content)
    {
        var comment = new CommentDto
        {
            Id = Guid.NewGuid(),
            Content = content,
            AuthorId = userId,
            TargetId = targetId,
            TargetType = targetType,
            CreatedAt = DateTime.UtcNow,
        };

        await CreateCommentAsync(comment);

        return comment;
    }

    public async Task CreateCommentAsync(CommentDto model)
    {
        var comment = new Comment
        {
            Id = model.Id,
            Content = model.Content,
            AuthorId = model.AuthorId,
            TargetId = model.TargetId,
            TargetType = model.TargetType,
            CreatedAt = model.CreatedAt,
        };

        await _unitOfWork.CommentRepository.CreateAsync(comment);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteCommentByIdAsync(Guid commentId)
    {
        await _unitOfWork.CommentRepository.DeleteByIdAsync(commentId);
        await _unitOfWork.SaveAsync();
    }
}
