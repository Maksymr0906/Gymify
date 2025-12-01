using Gymify.Application.DTOs.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.Services.Interfaces;

public interface IFriendsService
{
    Task<List<FriendDto>> SearchPotentialFriendsAsync(string query, Guid currentUserId);
    Task SendFriendRequestAsync(Guid senderId, Guid receiverId);
    Task AcceptFriendRequestAsync(Guid senderId, Guid currentUserId);
    Task DeclineFriendRequestAsync(Guid senderId, Guid currentUserId);
    Task CancelFriendRequestAsync(Guid receiverId, Guid currentUserId);
    Task<List<FriendDto>> GetFriendsAsync(Guid userId);
    Task<List<FriendDto>> GetIncomingInvitesAsync(Guid userId);
    Task<List<FriendDto>> GetOutgoingInvitesAsync(Guid userId);
}
