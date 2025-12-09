using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserChatRepository
{
    Task CreateAsync(UserChat userChat);
    Task DeleteAsync(UserChat userChat);
    Task<UserChat?> GetByChatAndUserAsync(Guid chatId, Guid userId);
    Task<List<UserChat>> GetUserChatsAsync(Guid userId);
    Task<List<UserChat>> GetUserChatsWithDetailsAsync(Guid userId);
    Task<List<UserChat>> GetChatMembersAsync(Guid chatId);
}
