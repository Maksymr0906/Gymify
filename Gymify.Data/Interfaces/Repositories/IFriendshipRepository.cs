using Gymify.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Data.Interfaces.Repositories;

public interface IFriendshipRepository
{
    Task CreateAsync(Friendship friendship);
    Task DeleteAsync(Friendship friendship);
    Task<Friendship?> GetByUsersAsync(Guid user1Id, Guid user2Id);
}
