using Gymify.Application.DTOs.Comment;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class CommentService(IUnitOfWork unitOfWork) : ICommentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<List<CommentDto>> GetCommentDtos(Guid currentProfileUserId,Guid targetId, CommentTargetType targetType)
    {
        var comments = await _unitOfWork.CommentRepository.GetCommentsByTargetIdAndTypeAsync(targetId, targetType);

        List<CommentDto> commentDtos = new();

        foreach (var comment in comments)
        {
            var avatar = await _unitOfWork.ItemRepository.GetByIdAsync(comment.Author.Equipment.AvatarId);

            commentDtos.Add(new CommentDto
            {
                Id = comment.Id,
                AuthorId = comment.AuthorId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                AuthorName = comment.Author.ApplicationUser.UserName,
                AuthorAvatarUrl = avatar.ImageURL,
                CanDelete = true ? comment.Author.Id == currentProfileUserId : false,
                TargetId = targetId,
                TargetType = targetType
            });
        }

        return commentDtos;
    }

    public async Task<CommentDto> UploadComment(Guid currentProfileUserId, Guid targetId, CommentTargetType targetType, string content)
    {
        var currentUser = await _unitOfWork.UserProfileRepository.GetAllCredentialsAboutUserByIdAsync(currentProfileUserId);

        var avatar = await _unitOfWork.ItemRepository.GetByIdAsync(currentUser.Equipment.AvatarId);

        var comment = new CommentDto
        {
            Id = Guid.NewGuid(),
            Content = content,
            AuthorId = currentProfileUserId,
            AuthorName = currentUser.ApplicationUser.UserName,
            AuthorAvatarUrl = avatar.ImageURL,
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

        try
        {
            await _unitOfWork.CommentRepository.CreateAsync(comment);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex)
        {
            // Це допоможе вам побачити реальну причину помилки в консолі
            throw new Exception($"Database Error: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task DeleteCommentByIdAsync(Guid commentId)
    {
        await _unitOfWork.CommentRepository.DeleteByIdAsync(commentId);
        await _unitOfWork.SaveAsync();
    }
}
