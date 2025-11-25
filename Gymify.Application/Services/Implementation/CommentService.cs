using Gymify.Application.DTOs.Comment;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class CommentService(IUnitOfWork unitOfWork) : ICommentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<List<CommentDto>> GetCommentDtos(Guid currentProfileUserId, Guid targetId, CommentTargetType targetType)
    {
        // Цей метод репозиторію ВЖЕ має робити Include(x => x.Author.UserEquipment.Avatar)
        var comments = await _unitOfWork.CommentRepository.GetCommentsByTargetIdAndTypeAsync(targetId, targetType);

        // Мапимо дані в пам'яті (швидко)
        var commentDtos = comments.Select(comment => new CommentDto
        {
            Id = comment.Id,
            AuthorId = comment.AuthorId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,

            // Безпечний доступ до імені (Author може бути null, якщо юзера видалили, хоча це рідкість)
            AuthorName = comment.Author?.ApplicationUser?.UserName ?? "Unknown User",

            // ✅ БЕРЕМО АВАТАР З НАВІГАЦІЙНИХ ВЛАСТИВОСТЕЙ (БЕЗ ЗАЙВИХ ЗАПИТІВ)
            // Припускаємо, що навігаційна властивість в UserProfile називається 'UserEquipment' або 'Equipment'
            AuthorAvatarUrl = comment.Author?.Equipment?.Avatar?.ImageURL ?? "/images/default-avatar.png",

            // Спрощена логіка CanDelete
            CanDelete = comment.AuthorId == currentProfileUserId,

            TargetId = targetId,
            TargetType = targetType
        }).ToList();

        return commentDtos;
    }

    public async Task<CommentDto> UploadComment(Guid currentProfileUserId, Guid targetId, CommentTargetType targetType, string content)
    {
        // Отримуємо дані автора, включаючи Equipment та Avatar
        // Переконайтесь, що GetAllCredentials... робить Include(UserEquipment.Avatar)
        var currentUser = await _unitOfWork.UserProfileRepository.GetAllCredentialsAboutUserByIdAsync(currentProfileUserId);

        if (currentUser == null) throw new Exception("User not found");

        // Дістаємо URL безпечно
        var avatarUrl = currentUser.Equipment?.Avatar?.ImageURL ?? "/images/default-avatar.png";

        var commentDto = new CommentDto
        {
            Id = Guid.NewGuid(),
            Content = content,
            AuthorId = currentProfileUserId,
            AuthorName = currentUser.ApplicationUser?.UserName ?? "Unknown",
            AuthorAvatarUrl = avatarUrl,
            TargetId = targetId,
            TargetType = targetType,
            CreatedAt = DateTime.UtcNow,
            CanDelete = true // Автор завжди може видалити свій коментар
        };

        // Зберігаємо
        await CreateCommentAsync(commentDto);

        return commentDto;
    }

    public async Task CreateCommentAsync(CommentDto model)
    {
        var comment = new Comment
        {
            Id = model.Id,
            Content = model.Content,
            AuthorId = model.AuthorId,
            TargetId = model.TargetId,     // Просто GUID
            TargetType = model.TargetType, // Enum
            CreatedAt = model.CreatedAt,
        };

        try
        {
            await _unitOfWork.CommentRepository.CreateAsync(comment);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex)
        {
            // Логування внутрішньої помилки для дебагу
            throw new Exception($"Database Error: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task DeleteCommentByIdAsync(Guid commentId)
    {
        // Тут можна додати перевірку прав, якщо потрібно
        await _unitOfWork.CommentRepository.DeleteByIdAsync(commentId);
        await _unitOfWork.SaveAsync();
    }
}