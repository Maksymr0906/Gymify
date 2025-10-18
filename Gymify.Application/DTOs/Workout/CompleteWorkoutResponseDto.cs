using Gymify.Application.DTOs.Achievement;

namespace Gymify.Application.DTOs.Workout;

public class CompleteWorkoutResponseDto
{
    public WorkoutDto WorkoutDto { get; set; }
    public ICollection<AchievementDto> AchievementDtos { get; set; }

}
