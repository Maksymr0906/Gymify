using Gymify.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.DTOs.Friends;

public class FriendDto
{
    public Guid ProfileId { get; set; }
    public string UserName { get; set; }
    public string AvatarUrl { get; set; }
    public Guid? ChatId { get; set; } 
    public int Level { get; set; }
    public DateTime? SentAt { get; set; } 
    public UserRelationshipStatus Status { get; set; }
}
