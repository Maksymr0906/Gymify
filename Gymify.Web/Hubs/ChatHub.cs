using Gymify.Application.DTOs.Chat;
using Gymify.Application.DTOs.Comment;
using Gymify.Application.Services.Implementation; // Переконайтеся, що тут правильний namespace трекера
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;

namespace Gymify.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ChatMembersTrackerService _chatMembersTrackerService;

        public ChatHub(IChatService chatService, ChatMembersTrackerService chatMembersTrackerService)
        {
            _chatService = chatService;
            _chatMembersTrackerService = chatMembersTrackerService;
        }

        public async Task JoinChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

            // === НОВЕ: Фіксуємо, що юзер дивиться цей чат ===
            if (Guid.TryParse(chatId, out Guid cId) &&
                Guid.TryParse(Context.UserIdentifier, out Guid userId))
            {
                _chatMembersTrackerService.UserJoinedChat(userId, cId);
            }
        }

        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);

            // === НОВЕ: Юзер вийшов з чату ===
            if (Guid.TryParse(Context.UserIdentifier, out Guid userId))
            {
                _chatMembersTrackerService.UserLeftChat(userId);
            }
        }

        // === НОВЕ: Обробка закриття вкладки / розриву з'єднання ===
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (Guid.TryParse(Context.UserIdentifier, out Guid userId))
            {
                // Якщо юзер відключився, він точно не дивиться чат
                _chatMembersTrackerService.UserLeftChat(userId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageRequestDto request)
        {
            if (!TryValidate(request, out var errorMessage))
            {
                throw new HubException(errorMessage);
            }

            // Використовуємо UserIdentifier, якщо IUserIdProvider налаштований повертати UserProfileId
            // Або ваш старий метод: Context.User.FindFirst("UserProfileId").Value
            var senderId = Guid.Parse(Context.UserIdentifier ?? Context.User.FindFirst("UserProfileId").Value);

            var messageDto = await _chatService.SaveMessageAsync(request.ChatId, senderId, request.Content);

            await Clients.Group(request.ChatId.ToString()).SendAsync("ReceiveMessage", messageDto);
        }

        public async Task EditMessage(EditMessageRequestDto request)
        {
            if (!TryValidate(request, out var errorMessage))
            {
                throw new HubException(errorMessage);
            }

            var senderId = Guid.Parse(Context.UserIdentifier ?? Context.User.FindFirst("UserProfileId").Value);

            var messageDto = await _chatService.EditMessageAsync(request.MessageId, senderId, request.Content);

            await Clients.Group(messageDto.ChatId.ToString()).SendAsync("MessageEdited", messageDto);
        }

        public async Task DeleteMessage(string messageIdStr, string chatIdStr)
        {
            var userId = Guid.Parse(Context.UserIdentifier ?? Context.User.FindFirst("UserProfileId").Value);
            var messageId = Guid.Parse(messageIdStr);
            // chatIdStr не обов'язково використовувати тут, якщо ми беремо його з повідомлення в сервісі,
            // але для відправки події групі він потрібен.

            var updatedChatInfo = await _chatService.DeleteMessageAsync(messageId, userId);

            // Повідомляємо клієнтів, що повідомлення видалено
            await Clients.Group(chatIdStr).SendAsync("MessageDeleted", messageIdStr);

            // Якщо змінилося останнє повідомлення (прев'ю чату), оновлюємо його
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

        private bool TryValidate(object obj, out string error)
        {
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, context, results, true);

            if (!isValid)
            {
                error = results.FirstOrDefault()?.ErrorMessage ?? "Validation error";
                return false;
            }

            error = null;
            return true;
        }
    }
}