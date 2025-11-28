using Gymify.Application.Helpers;
using Gymify.Data.Enums;

namespace Gymify.Application.DTOs.Exercise;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BaseXP { get; set;} = 0;
    public ExerciseType Type { get; set; }
    public bool IsApproved { get; set; }
    public string VideoURL { get; set; } = string.Empty;
    public string? VideoEmbedUrl => YouTubeHelper.GetEmbedUrl(VideoURL);
    public string ThumbnailUrl => YouTubeHelper.GetThumbnailUrl(VideoURL);
}
