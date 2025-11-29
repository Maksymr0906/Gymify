using Gymify.Application.DTOs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.Services.Interfaces
{
    public interface IChatService
    {
        Task<List<ChatDto>> GetUserChatsAsync(Guid userId);
        Task<List<MessageDto>> GetChatHistoryAsync(Guid chatId, Guid currentUserId);
        Task<MessageDto> SaveMessageAsync(Guid chatId, Guid senderId, string content);
        Task<Guid> GetOrCreatePrivateChatAsync(Guid currentUserId, Guid targetUserId);
        Task<MessageDto> EditMessageAsync(Guid messageId, Guid userId, string newContent);
        Task DeleteMessageAsync(Guid messageId, Guid userId);
    }
}
