namespace Gymify.Data.Entities;

public class UserCase
{
    public Guid UserProfileId { get; set; }
    public Guid CaseId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public Case Case { get; set; } = null!;
}
