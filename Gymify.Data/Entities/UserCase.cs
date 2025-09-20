namespace Gymify.Data.Entities;

public class UserCase
{
    public Guid UserId { get; set; }
    public Guid CaseId { get; set; }
    public User User { get; set; } = null!;
    public Case Case { get; set; } = null!;
}
