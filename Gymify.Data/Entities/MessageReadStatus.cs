using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Data.Entities;

public class MessageReadStatus
{
    public Guid MessageId { get; set; }
    public Message Message { get; set; } = null!;

    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;

    public DateTime ReadAt { get; set; }
}
