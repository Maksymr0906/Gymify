using Gymify.Application.DTOs.Chat;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class ChatService(IUnitOfWork unitOfWork) : IChatService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<List<ChatDto>> GetUserChatsAsync(Guid userId)
    {
        var userChats = await _unitOfWork.UserChatRepository.GetUserChatsWithDetailsAsync(userId);
        var result = new List<ChatDto>();

        foreach (var uc in userChats)
        {
            // 1. ОГОЛОШЕННЯ: Ініціалізуємо null на початку кожної ітерації
            Guid? targetUserId = null;

            string chatName = "Unknown";
            string chatImage = "/images/default.png";

            if (uc.Chat.Type == ChatType.Private)
            {
                // Шукаємо співрозмовника
                var otherMember = uc.Chat.Members.FirstOrDefault(m => m.UserProfileId != userId);

                if (otherMember != null)
                {
                    chatName = otherMember.UserProfile.ApplicationUser?.UserName ?? "Unknown";
                    chatImage = otherMember.UserProfile.Equipment?.Avatar?.ImageURL ?? "/images/default-avatar.png";

                    // 2. ПРИСВОЄННЯ: Якщо це приватний чат, беремо ID друга
                    targetUserId = otherMember.UserProfileId;
                }
            }
            else
            {
                // Для груп targetUserId залишається null
                chatName = uc.Chat.Name ?? "Group Chat";
                // chatImage = ...
            }

            result.Add(new ChatDto
            {
                ChatId = uc.ChatId,
                ChatName = chatName,
                ChatAvatarUrl = chatImage,
                LastMessageContent = uc.Chat.LastMessage?.Content,
                LastMessageTime = uc.Chat.LastMessage?.CreatedAt,
                IsPrivate = uc.Chat.Type == ChatType.Private,
                TargetUserId = targetUserId,
                LastMessageId = uc.Chat.LastMessage?.Id
            });
        }

        return result;
    }

    public async Task<List<MessageDto>> GetChatHistoryAsync(Guid chatId, Guid currentUserId)
    {
        // Перевірка доступу: чи є юзер учасником чату?
        var membership = await _unitOfWork.UserChatRepository.GetByChatAndUserAsync(chatId, currentUserId);
        if (membership == null) throw new UnauthorizedAccessException("You are not a member of this chat.");

        var messages = await _unitOfWork.MessageRepository.GetMessagesByChatIdAsync(chatId);

        // Мапимо і розвертаємо (щоб старі були зверху, як у діалозі)
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ChatId = m.ChatId,
            SenderId = m.SenderId,
            SenderName = m.Sender.ApplicationUser?.UserName ?? "Unknown",
            SenderAvatarUrl = m.Sender.Equipment?.Avatar?.ImageURL ?? "/images/default-avatar.png",
            Content = m.Content,
            CreatedAt = m.CreatedAt,
            IsMe = m.SenderId == currentUserId // Важливо для CSS класів (справа/зліва)
        })
        .OrderBy(m => m.CreatedAt) // Старі зверху
        .ToList();
    }

    public async Task<MessageDto> SaveMessageAsync(Guid chatId, Guid senderId, string content)
    {
        // 1. Перевірка доступу (чи є юзер учасником)
        var member = await _unitOfWork.UserChatRepository.GetByChatAndUserAsync(chatId, senderId);
        if (member == null) throw new UnauthorizedAccessException("User is not a member of this chat");

        // 2. Створення повідомлення
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderId = senderId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.MessageRepository.CreateAsync(message);

        // 3. Оновлення "Останнього повідомлення" в чаті (для сортування списку)
        var chat = await _unitOfWork.ChatRepository.GetByIdAsync(chatId);
        chat.LastMessageId = message.Id;
        await _unitOfWork.ChatRepository.UpdateAsync(chat);

        await _unitOfWork.SaveAsync();

        // 4. Повертаємо DTO для відправки клієнтам
        // Дістаємо профіль автора з екіпіруванням (щоб взяти аватар)
        var senderProfile = await _unitOfWork.UserProfileRepository.GetAllCredentialsAboutUserByIdAsync(senderId);

        var avatarUrl = senderProfile?.Equipment?.Avatar?.ImageURL ?? "/images/default-avatar.png";
        var senderName = senderProfile?.ApplicationUser?.UserName ?? "Unknown";

        return new MessageDto
        {
            Id = message.Id,
            ChatId = chatId,
            SenderId = senderId,
            SenderName = senderName, // <--- ЗАПОВНЮЄМО
            SenderAvatarUrl = avatarUrl, // <--- ЗАПОВНЮЄМО
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            IsMe = true
        };
    }
    public async Task<Guid> GetOrCreatePrivateChatAsync(Guid currentUserId, Guid targetUserId)
    {
        // 1. Спроба знайти існуючий приватний чат
        // Це складний запит: шукаємо чат типу Private, де є обидва юзери
        // Але ми можемо схитрувати: якщо вони друзі, у них в таблиці Friendship вже є ChatId.

        var friendship = await _unitOfWork.FriendshipRepository.GetByUsersAsync(currentUserId, targetUserId);
        if (friendship != null)
        {
            return friendship.ChatId;
        }

        // Якщо вони не друзі (але ти дозволяєш писати), або Friendship.ChatId чомусь пустий (стара база),
        // то треба шукати в таблиці UserChats перетином.
        // ... (для простоти поки опустимо складний SQL пошук, покладемося на Friendship) ...

        // 2. Якщо чату немає - створюємо новий
        var newChat = new Chat
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Private,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.ChatRepository.CreateAsync(newChat);

        // Додаємо учасників
        await _unitOfWork.UserChatRepository.CreateAsync(new UserChat { ChatId = newChat.Id, UserProfileId = currentUserId });
        await _unitOfWork.UserChatRepository.CreateAsync(new UserChat { ChatId = newChat.Id, UserProfileId = targetUserId });

        await _unitOfWork.SaveAsync();

        return newChat.Id;
    }

    public async Task<MessageDto> EditMessageAsync(Guid messageId, Guid userId, string newContent)
    {
        var msg = await _unitOfWork.MessageRepository.GetByIdAsync(messageId);
        if (msg == null) throw new Exception("Message not found");
        if (msg.SenderId != userId) throw new UnauthorizedAccessException("Not your message");

        msg.Content = newContent;
        // До речі, у сутності Message варто додати поле IsEdited (bool)
        // msg.IsEdited = true; 

        await _unitOfWork.MessageRepository.UpdateAsync(msg);
        await _unitOfWork.SaveAsync();

        // Повертаємо оновлений DTO (можна спрощено)
        return new MessageDto
        {
            Id = msg.Id,
            ChatId = msg.ChatId,
            Content = msg.Content,
            IsEdited = true
        };
    }

    public async Task<ChatDto?> DeleteMessageAsync(Guid messageId, Guid userId)
    {
        var msg = await _unitOfWork.MessageRepository.GetByIdAsync(messageId);
        if (msg == null) return null;
        if (msg.SenderId != userId) throw new UnauthorizedAccessException("Not your message");

        var chatId = msg.ChatId;

        // Видаляємо
        await _unitOfWork.MessageRepository.DeleteByIdAsync(messageId);
        // (Save поки не робимо, зробимо в кінці разом з оновленням чату)

        // Перевіряємо, чи це було останнє повідомлення в чаті
        var chat = await _unitOfWork.ChatRepository.GetByIdAsync(chatId);

        if (chat.LastMessageId == messageId)
        {
            // Так, це було останнє. Треба знайти нове останнє.
            // Важливо: Оскільки ми ще не зробили SaveAsync, поточне повідомлення ще технічно є в базі,
            // тому беремо "передостаннє" або робимо Save спочатку.
            // Простіше зробити Save спочатку.
            await _unitOfWork.SaveAsync();

            // Шукаємо нове останнє
            var newLastMsg = await _unitOfWork.MessageRepository.FindLastMessageAsync(chatId);

            chat.LastMessageId = newLastMsg?.Id;
            chat.LastMessage = newLastMsg; // Для оновлення навігаційної властивості

            await _unitOfWork.ChatRepository.UpdateAsync(chat);
            await _unitOfWork.SaveAsync();

            // Повертаємо інфо для оновлення UI
            return new ChatDto
            {
                ChatId = chatId,
                LastMessageContent = newLastMsg?.Content,
                LastMessageTime = newLastMsg?.CreatedAt
            };
        }
        else
        {
            // Це було не останнє повідомлення, просто зберігаємо видалення
            await _unitOfWork.SaveAsync();
            return null; // UI оновлювати не треба (в плані прев'ю)
        }
    }
}