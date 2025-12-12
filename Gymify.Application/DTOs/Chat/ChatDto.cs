using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.DTOs.Chat
{
    public class ChatDto
    {
        public Guid ChatId { get; set; }
        public Guid? TargetUserId { get; set; } 
        public Guid? LastMessageId { get; set; } 
        public string ChatName { get; set; } = string.Empty; 
        public string ChatAvatarUrl { get; set; } = string.Empty;
        public string? LastMessageContent { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public bool IsPrivate { get; set; }
        public int UnreadCount { get; set; } 
    }
}
