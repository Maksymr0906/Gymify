using Gymify.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Data.Interfaces.Repositories;

public interface IFriendInviteRepository
{
    Task CreateAsync(FriendInvite invite);
    Task DeleteAsync(FriendInvite invite);
    Task<FriendInvite?> GetInviteAsync(Guid senderId, Guid receiverId);
    Task<FriendInvite?> GetInviteAnyDirectionAsync(Guid user1Id, Guid user2Id);
    Task<List<FriendInvite>> GetIncomingInvitesAsync(Guid receiverId);
    Task<List<FriendInvite>> GetOutgoingInvitesAsync(Guid senderId);
}
