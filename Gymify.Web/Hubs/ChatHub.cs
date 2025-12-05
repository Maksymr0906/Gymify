using Gymify.Application.DTOs.Chat;
using Gymify.Application.DTOs.Comment;
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
        public async Task SendMessage(CreateMessageRequestDto request)
        {
            // MANUAL VALIDATION
            if (!TryValidate(request, out var errorMessage))
            {
                // This throws an error back to the specific client's "catch" block in JS
                throw new HubException(errorMessage);
            }

            var senderId = Guid.Parse(Context.User.FindFirst("UserProfileId").Value);

            // Pass DTO or mapped values to service
            var messageDto = await _chatService.SaveMessageAsync(request.ChatId, senderId, request.Content);

            await Clients.Group(request.ChatId.ToString()).SendAsync("ReceiveMessage", messageDto);
        }

        public async Task EditMessage(EditMessageRequestDto request)
        {
            if (!TryValidate(request, out var errorMessage))
            {
                throw new HubException(errorMessage);
            }

            var senderId = Guid.Parse(Context.User.FindFirst("UserProfileId").Value);

            var messageDto = await _chatService.EditMessageAsync(request.MessageId, senderId, request.Content);

            await Clients.Group(messageDto.ChatId.ToString()).SendAsync("MessageEdited", messageDto);
        }

        private bool TryValidate(object obj, out string error)
        {
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, context, results, true);

            if (!isValid)
            {
                // Return the first error message (e.g., "Message must be between 1 and 1000...")
                error = results.FirstOrDefault()?.ErrorMessage ?? "Validation error";
                return false;
            }

            error = null;
            return true;
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