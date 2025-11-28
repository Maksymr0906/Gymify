using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Data.Entities;

public class UserChat
{
    public Guid ChatId { get; set; }
    public Guid UserProfileId { get; set; }
    public Guid? LastReadMessageId { get; set; }
    public Chat Chat { get; set; } = null!;
    public UserProfile UserProfile { get; set; } = null!;
    public bool IsOwner { get; set; } = false;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
