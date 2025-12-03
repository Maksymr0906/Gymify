using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gymify.Application.DTOs.UserExercise;

public class UserExerciseRequestDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "ExerciseNameLength")]
    [Display(Name = "ExerciseName")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required")]
    [Range(1, 100, ErrorMessage = "MinMaxRange")] 
    [Display(Name = "Label_Sets")]
    public int? Sets { get; set; } 

    [Required(ErrorMessage = "Required")]
    [Range(1, 1000, ErrorMessage = "MinMaxRange")]
    [Display(Name = "Label_Reps")]
    public int? Reps { get; set; }

    [Required(ErrorMessage = "Required")]
    [Range(0, 500, ErrorMessage = "MinMaxRange")]
    [Display(Name = "Label_Kg")]
    public int? Weight { get; set; }

    [Range(0, 300, ErrorMessage = "MinMaxRange")]
    [Display(Name = "Label_Min")]
    public int? DurationMinutes { get; set; }
}