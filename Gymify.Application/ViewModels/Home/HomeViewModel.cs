using Gymify.Application.DTOs.Workout;

namespace Gymify.Application.ViewModels.Home;

public class HomeViewModel
{
    public int Level { get; set; }
    public double ProgressPercentage { get; set; }
    public int XpEarnedInThisLevel { get; set; }
    public int XpNeededForThisLevel { get; set; }
    public List<WorkoutDto> LastTrainings { get; set; } = new();
}
