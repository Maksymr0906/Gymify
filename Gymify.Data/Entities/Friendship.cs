namespace Gymify.Data.Entities;

public class Friendship
{
    public Guid UserProfileId1 { get; set; }
    public Guid UserProfileId2 { get; set; }
    public UserProfile UserProfile1 { get; set; } = null!;
    public UserProfile UserProfile2 { get; set; } = null!;
}
