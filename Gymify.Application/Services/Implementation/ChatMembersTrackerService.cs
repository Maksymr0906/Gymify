using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.Services.Implementation
{
    public class ChatMembersTrackerService
    {
        private static readonly ConcurrentDictionary<Guid, Guid> _onlineUsers = new();

        public void UserJoinedChat(Guid userId, Guid chatId)
        {
            _onlineUsers.AddOrUpdate(userId, chatId, (key, oldValue) => chatId);
        }

        public void UserLeftChat(Guid userId)
        {
            _onlineUsers.TryRemove(userId, out _);
        }

        public bool IsUserActiveInChat(Guid userId, Guid chatId)
        {
            return _onlineUsers.TryGetValue(userId, out var activeChatId) && activeChatId == chatId;
        }

        public bool IsUserBrowsingChats(Guid userId)
        {
            return _onlineUsers.ContainsKey(userId);
        }
    }
}
