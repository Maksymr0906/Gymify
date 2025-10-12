using Microsoft.AspNetCore.Identity;

namespace Gymify.Data.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid UserProfileId { get; set; }

    public UserProfile? UserProfile { get; set; }
}
