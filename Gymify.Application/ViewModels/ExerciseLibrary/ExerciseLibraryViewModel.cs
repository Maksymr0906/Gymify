using Gymify.Application.DTOs.Exercise;
using Gymify.Data.Enums;

namespace Gymify.Application.ViewModels.ExerciseLibrary
{
    public class ExerciseLibraryViewModel
    {
        public List<ExerciseDto> Exercises { get; set; } = new();
        public string SearchTerm { get; set; }
        public ExerciseType? TypeFilter { get; set; }
        public bool ShowPendingOnly { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}