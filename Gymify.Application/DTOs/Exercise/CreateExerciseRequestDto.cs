using Gymify.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Gymify.Application.DTOs.Exercise;

public class CreateExerciseRequestDto
{
    [Required(ErrorMessage = "Required")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "NameLength")]
    [Display(Name = "ExerciseName")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required")] 
    [Display(Name = "ExerciseType")]
    public ExerciseType Type { get; set; }
}