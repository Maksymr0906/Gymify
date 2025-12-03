using System.ComponentModel.DataAnnotations;

namespace Gymify.Application.DTOs.UserExercise;

public class AddUserExerciseDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public Guid WorkoutId { get; set; }

    [Required(ErrorMessage = "Msg_ExerciseNameRequired")]
    [Length(3, 100, ErrorMessage = "Msg_NameLength")]
    public string Name { get; set; }

    [Range(0, 100, ErrorMessage = "Msg_SetsMustBePositive")]
    public int? Sets { get; set; }

    [Range(0, 1000, ErrorMessage = "Msg_RepsPositive")]
    public int? Reps { get; set; } = 0;

    [Range(0.0, 500.0, ErrorMessage = "Msg_WeightPositive")]
    public double? Weight { get; set; } = 0.0;

    [Range(0, 360, ErrorMessage = "Msg_DurationPositive")]
    public double? Duration { get; set; } = 0;

    public int ExerciseType { get; set; } = 1;

}
