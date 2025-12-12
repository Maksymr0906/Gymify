using Gymify.Application.Helpers;
using Gymify.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.DTOs.Exercise
{
    public class AdminExerciseDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string NameUk { get; set; } = string.Empty;
        public string DescriptionUk { get; set; } = string.Empty;
        public int BaseXP { get; set; } = 0;
        public double DifficultyMultiplier { get; set; }
        public ExerciseType Type { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string VideoURL { get; set; } = string.Empty;
        public string? VideoEmbedUrl => YouTubeHelper.GetEmbedUrl(VideoURL);
        public string ThumbnailUrl => YouTubeHelper.GetThumbnailUrl(VideoURL);
    }
}
