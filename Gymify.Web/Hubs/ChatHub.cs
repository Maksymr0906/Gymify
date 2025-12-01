using Gymify.Application.DTOs.Chat;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Gymify.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // Клієнт викликає цей метод, коли відкриває конкретний чат
        public async Task JoinChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        // Клієнт викликає, коли закриває чат
        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        }

        // ВІДПРАВКА ПОВІДОМЛЕННЯ
        public async Task SendMessage(string chatIdStr, string content)
        {
            var userId = Guid.Parse(Context.UserIdentifier); // Працює завдяки CustomUserIdProvider
            var chatId = Guid.Parse(chatIdStr);

            // 1. Зберігаємо в БД
            var messageDto = await _chatService.SaveMessageAsync(chatId, userId, content);

            // 2. Відправляємо всім у кімнаті (включаючи себе)
            // Клієнт сам розбереться, це "моє" повідомлення чи "чуже" по SenderId
            await Clients.Group(chatIdStr).SendAsync("ReceiveMessage", messageDto);

            // 3. (Опціонально) Сповістити інших учасників, щоб оновили список чатів (Sidebar), 
            // навіть якщо вони не в цьому чаті зараз. Це складніше, поки пропустимо.
        }

        public async Task EditMessage(string messageIdStr, string newContent)
        {
            var userId = Guid.Parse(Context.UserIdentifier);
            var messageId = Guid.Parse(messageIdStr);

            var updatedMsg = await _chatService.EditMessageAsync(messageId, userId, newContent);

            // Сповіщаємо групу (чат), що повідомлення змінилось
            await Clients.Group(updatedMsg.ChatId.ToString()).SendAsync("MessageEdited", updatedMsg);
        }

        public async Task DeleteMessage(string messageIdStr, string chatIdStr)
        {
            var userId = Guid.Parse(Context.UserIdentifier);
            var messageId = Guid.Parse(messageIdStr);
            var chatId = Guid.Parse(chatIdStr);

            // Видаляємо і отримуємо нове прев'ю (якщо треба)
            var updatedChatInfo = await _chatService.DeleteMessageAsync(messageId, userId);

            // 1. Сповіщаємо про видалення самого повідомлення (щоб зникло з діалогу)
            await Clients.Group(chatIdStr).SendAsync("MessageDeleted", messageIdStr);

            // 2. Якщо змінилося останнє повідомлення - сповіщаємо про оновлення прев'ю
            if (updatedChatInfo != null)
            {
                await Clients.Group(chatIdStr).SendAsync("ChatPreviewUpdated", new
                {
                    chatId = updatedChatInfo.ChatId,
                    content = updatedChatInfo.LastMessageContent ?? "No messages yet",
                    createdAt = updatedChatInfo.LastMessageTime
                });
            }
        }
    }
}