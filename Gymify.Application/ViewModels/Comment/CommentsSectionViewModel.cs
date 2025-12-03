using Gymify.Application.DTOs.Comment;
using Gymify.Data.Enums;

namespace Gymify.Application.ViewModels.Comment;

public class CommentsSectionViewModel
{
    public Guid TargetId { get; set; }

    public CommentTargetType TargetType { get; set; }

    public List<CommentDto> CommentDtos { get; set; } = new();

    public string CurrentUserAvatarUrl { get; set; } = "/Images/DefaultAvatar.png";
}