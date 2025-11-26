using Gymify.Application.DTOs.UserExercise;
using Gymify.Application.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.ViewModels.Workout;
public class WorkoutDetailsViewModel
{
    public Guid AuthorId { get; set; } 
    public Guid WorkoutId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string CurrentUserAvatarUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int TotalXP { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsOwner { get; set; }
    public List<UserExerciseDto> Exercises { get; set; } = new();
    public CommentsSectionViewModel Comments { get; set; } = new();
}
