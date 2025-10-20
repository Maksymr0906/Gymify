using Gymify.Application.DTOs.Comment;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class CommentService(IUnitOfWork unitOfWork) : ICommentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task CreateCommentAsync(CreateCommentRequestDto model)
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = model.Content,
            AuthorId = model.AuthorId,
            TargetId = model.TargetId,
            TargetType = model.TargetType,
            CreatedAt = DateTime.UtcNow,
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
