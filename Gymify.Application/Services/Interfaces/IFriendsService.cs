using Gymify.Application.DTOs.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.Services.Interfaces;

public interface IFriendsService
{
    Task SendFriendRequestAsync(Guid senderId, Guid receiverId);
    Task AcceptFriendRequestAsync(Guid senderId, Guid currentUserId);
    Task DeclineFriendRequestAsync(Guid senderId, Guid currentUserId);
    Task<List<FriendDto>> GetFriendsAsync(Guid userId);
    Task<List<FriendDto>> GetIncomingInvitesAsync(Guid userId);
}
