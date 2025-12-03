using System.ComponentModel.DataAnnotations;

namespace Gymify.Application.DTOs.Workout;

public class CreateWorkoutRequestDto
{
    [Required(ErrorMessage = "Required")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "NameLength")]
    [Display(Name = "Name")]
    public required string Name { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "NameLength")]
    [Display(Name = "Description")]
    public string? Description { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "NameLength")]
    [Display(Name = "Conclusion")]
    public string? Conclusion { get; set; } = string.Empty;

    public required bool IsPrivate { get; set; } = true;
    public required Guid UserProfileId { get; set; }
}
