using Gymify.Application.DTOs.Comment;
using Gymify.Data.Enums;

namespace Gymify.Web.ViewModels.Comment;

public class CommentsSectionViewModel
{
    public Guid TargetId { get; set; }

    public CommentTargetType TargetType { get; set; }

    public List<CommentDto> Comments { get; set; } = new();

    public string CurrentUserAvatarUrl { get; set; } = "/images/default-avatar.png";
}