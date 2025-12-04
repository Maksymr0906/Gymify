using System.ComponentModel.DataAnnotations;

namespace Gymify.Application.DTOs.UserExercise;

public class AddUserExerciseDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public Guid WorkoutId { get; set; }

    [Required(ErrorMessage = "Msg_ExerciseNameRequired")]
    [MinLength(3, ErrorMessage = "Msg_NameMinLength")]   // Новий ключ
    [MaxLength(100, ErrorMessage = "Msg_NameMaxLength")] // Новий ключ
    public string Name { get; set; }

    [Range(0, 100, ErrorMessage = "Msg_SetsMustBePositive")]
    public int? Sets { get; set; }

    [Range(0, 1000, ErrorMessage = "Msg_RepsPositive")]
    public int? Reps { get; set; } = 0;

    [Range(0.0, 500.0, ErrorMessage = "Msg_WeightPositive")]
    public double? Weight { get; set; } = 0.0;

    [Range(0.0, 360, ErrorMessage = "Msg_DurationPositive")]
    public double? Duration { get; set; } = 0.0;

    public int ExerciseType { get; set; } = 1;

}
