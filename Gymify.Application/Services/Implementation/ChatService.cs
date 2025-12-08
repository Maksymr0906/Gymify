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
            Guid? targetUserId = null;

            string chatName = "Unknown";
            string chatImage = "/images/default.png";

            if (uc.Chat.Type == ChatType.Private)
            {
                var otherMember = uc.Chat.Members.FirstOrDefault(m => m.UserProfileId != userId);

                if (otherMember != null)
                {
                    chatName = otherMember.UserProfile.ApplicationUser?.UserName ?? "Unknown";
                    chatImage = otherMember.UserProfile.Equipment?.Avatar?.ImageURL ?? "https://localhost:7102/Images/DefaultAvatar.png";

                    targetUserId = otherMember.UserProfileId;
                }
            }
            else
            {
                chatName = uc.Chat.Name ?? "Group Chat";
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
        var membership = await _unitOfWork.UserChatRepository.GetByChatAndUserAsync(chatId, currentUserId);
        if (membership == null) throw new UnauthorizedAccessException("You are not a member of this chat.");

        var messages = await _unitOfWork.MessageRepository.GetMessagesByChatIdAsync(chatId);

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ChatId = m.ChatId,
            SenderId = m.SenderId,
            SenderName = m.Sender.ApplicationUser?.UserName ?? "Unknown",
            SenderAvatarUrl = m.Sender.Equipment?.Avatar?.ImageURL ?? "https://localhost:7102/Images/DefaultAvatar.png",
            Content = m.Content,
            CreatedAt = m.CreatedAt,
            IsMe = m.SenderId == currentUserId 
        })
        .OrderBy(m => m.CreatedAt) 
        .ToList();
    }

    public async Task<MessageDto> SaveMessageAsync(Guid chatId, Guid senderId, string content)
    {
        if (string.IsNullOrWhiteSpace(content) || content.Length > 1000)
        {
            throw new ArgumentException("Invalid message content.");
        }

        var member = await _unitOfWork.UserChatRepository.GetByChatAndUserAsync(chatId, senderId);
        if (member == null) throw new UnauthorizedAccessException("User is not a member of this chat");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderId = senderId,
            Content = content
        };

        await _unitOfWork.MessageRepository.CreateAsync(message);

        var chat = await _unitOfWork.ChatRepository.GetByIdAsync(chatId);
        chat.LastMessageId = message.Id;
        await _unitOfWork.ChatRepository.UpdateAsync(chat);

        await _unitOfWork.SaveAsync();

        var senderProfile = await _unitOfWork.UserProfileRepository.GetAllCredentialsAboutUserByIdAsync(senderId);

        var avatarUrl = senderProfile?.Equipment?.Avatar?.ImageURL ?? "https://localhost:7102/Images/DefaultAvatar.png";
        var senderName = senderProfile?.ApplicationUser?.UserName ?? "Unknown";

        return new MessageDto
        {
            Id = message.Id,
            ChatId = chatId,
            SenderId = senderId,
            SenderName = senderName,
            SenderAvatarUrl = avatarUrl,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            IsMe = true
        };
    }
    public async Task<Guid> GetOrCreatePrivateChatAsync(Guid currentUserId, Guid targetUserId)
    {

        var friendship = await _unitOfWork.FriendshipRepository.GetByUsersAsync(currentUserId, targetUserId);
        if (friendship != null)
        {
            return friendship.ChatId;
        }

        var newChat = new Chat
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Private,
        };
        await _unitOfWork.ChatRepository.CreateAsync(newChat);

        await _unitOfWork.UserChatRepository.CreateAsync(new UserChat { ChatId = newChat.Id, UserProfileId = currentUserId });
        await _unitOfWork.UserChatRepository.CreateAsync(new UserChat { ChatId = newChat.Id, UserProfileId = targetUserId });

        await _unitOfWork.SaveAsync();

        return newChat.Id;
    }

    public async Task<MessageDto> EditMessageAsync(Guid messageId, Guid userId, string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent) || newContent.Length > 1000)
        {
            throw new ArgumentException("Invalid message content.");
        }

        var msg = await _unitOfWork.MessageRepository.GetByIdAsync(messageId);
        if (msg == null) throw new Exception("Message not found");
        if (msg.SenderId != userId) throw new UnauthorizedAccessException("Not your message");

        msg.Content = newContent;

        await _unitOfWork.MessageRepository.UpdateAsync(msg);
        await _unitOfWork.SaveAsync();

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

        await _unitOfWork.MessageRepository.DeleteByIdAsync(messageId);

        var chat = await _unitOfWork.ChatRepository.GetByIdAsync(chatId);

        if (chat.LastMessageId == messageId)
        {
            await _unitOfWork.SaveAsync();

            var newLastMsg = await _unitOfWork.MessageRepository.FindLastMessageAsync(chatId);

            chat.LastMessageId = newLastMsg?.Id;
            chat.LastMessage = newLastMsg; 

            await _unitOfWork.ChatRepository.UpdateAsync(chat);
            await _unitOfWork.SaveAsync();
            
            return new ChatDto
            {
                ChatId = chatId,
                LastMessageContent = newLastMsg?.Content,
                LastMessageTime = newLastMsg?.CreatedAt
            };
        }
        else
        {
            await _unitOfWork.SaveAsync();
            return null; 
        }
    }
}