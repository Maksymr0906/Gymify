using System.Text.RegularExpressions;

namespace Gymify.Application.Helpers
{
    public static class YouTubeHelper
    {
        // Регулярний вираз, який ловить ID відео з усіх можливих форматів посилань YouTube
        private static readonly Regex YoutubeVideoRegex = new Regex(
            @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)",
            RegexOptions.IgnoreCase);

        public static string? GetVideoId(string videoUrl)
        {
            if (string.IsNullOrWhiteSpace(videoUrl)) return null;

            var match = YoutubeVideoRegex.Match(videoUrl);
            return match.Success ? match.Groups[1].Value : null;
        }

        public static string? GetEmbedUrl(string videoUrl)
        {
            var videoId = GetVideoId(videoUrl);
            return videoId != null ? $"https://www.youtube.com/embed/{videoId}" : null;
        }

        // Опціонально: Отримати картинку-прев'ю
        public static string GetThumbnailUrl(string videoUrl)
        {
            var videoId = GetVideoId(videoUrl);
            return videoId != null ? $"https://img.youtube.com/vi/{videoId}/0.jpg" : "/images/no-video.png";
        }
    }
}