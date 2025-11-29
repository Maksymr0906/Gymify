using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 1. Дістаємо ID поточного юзера
            var userIdClaim = User.FindFirst("UserProfileId");
            if (userIdClaim == null) return RedirectToAction("Login", "Account"); // На всяк випадок

            var userId = Guid.Parse(userIdClaim.Value);

            // 2. Отримуємо список чатів через сервіс
            // (Ми його вже реалізували раніше: GetUserChatsAsync)
            var chats = await _chatService.GetUserChatsAsync(userId);

            // 3. Передаємо список у View
            return View(chats);
        }

        [HttpGet]
        public async Task<IActionResult> Start(Guid userId)
        {
            var currentUserId = Guid.Parse(User.FindFirst("UserProfileId").Value);

            // Отримуємо ID чату (створюємо, якщо нема)
            var chatId = await _chatService.GetOrCreatePrivateChatAsync(currentUserId, userId);

            // Переходимо на Index, але передаємо chatId, щоб JS його відкрив
            return RedirectToAction("Index", new { openChatId = chatId });
        }

        // API: Отримати список чатів (JSON)
        [HttpGet]
        public async Task<IActionResult> GetMyChats()
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId").Value);
            var chats = await _chatService.GetUserChatsAsync(userId);
            return Ok(chats);
        }

        // API: Отримати історію (JSON)
        [HttpGet]
        public async Task<IActionResult> GetHistory(Guid chatId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("UserProfileId").Value);
                var messages = await _chatService.GetChatHistoryAsync(chatId, userId);
                return Ok(messages);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }


    }
}
