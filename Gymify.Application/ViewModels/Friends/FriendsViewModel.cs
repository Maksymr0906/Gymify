using Gymify.Application.DTOs.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.ViewModels.Friends;

public class FriendsViewModel
{
    public List<FriendDto> Friends { get; set; } = new();
    public List<FriendDto> IncomingRequests { get; set; } = new();
}
