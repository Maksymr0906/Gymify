using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class PendingExercise : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ExerciseType Type { get; set; }
    public string? Description { get; set; }
    public string? VideoURL { get; set; }
    public Guid SubmittedByUserId { get; set; } // UserProfile який додав
    public UserProfile SubmittedByUser { get; set; } = null!;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; } = false; // флаг схвалення адміном

}
