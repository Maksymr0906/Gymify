namespace Gymify.Data.Entities;

public class Workout : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public bool IsPrivate { get; set; } = false;
    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public ICollection<UserExercise> Exercises { get; set; } = [];
}
