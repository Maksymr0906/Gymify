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
        public Guid? TargetUserId { get; set; } // ID співрозмовника (для приватних)
        public Guid? LastMessageId { get; set; } // <--- ДОДАЙ ЦЕ
        public string ChatName { get; set; } = string.Empty; // Ім'я друга
        public string ChatAvatarUrl { get; set; } = string.Empty; // Аватар друга
        public string? LastMessageContent { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public bool IsPrivate { get; set; }
        public int UnreadCount { get; set; } // На майбутнє
    }
}
