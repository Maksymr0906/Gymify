using Gymify.Application.DTOs.Workout;

namespace Gymify.Application.DTOs.WorkoutsFeed;

public class WorkoutDayDto
{
    public DateTime Date { get; set; }
    public List<WorkoutDto> Workouts { get; set; } = new();
    public int WorkoutCount => Workouts.Count;
    public int TotalXpForDay { get; set; }
}
