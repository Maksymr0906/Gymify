using AutoMapper.Execution;
using Gymify.Application.DTOs.Chat;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Application.Services.Implementation;

public class ChatService(IUnitOfWork unitOfWork, INotificationService notificationService, ChatMembersTrackerService chatMembersTrackerService) : IChatService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INotificationService _notificationService = notificationService;
    private readonly ChatMembersTrackerService _chatMembersTrackerService = chatMembersTrackerService;

    public async Task<List<Guid>> GetChatParticipantIdsAsync(Guid chatId)
    {
        var chat = await _unitOfWork.UserChatRepository.GetChatMembersAsync(chatId);

        if (chat == null) return new List<Guid>();

        return chat.Select(u => u.UserProfileId).ToList();
    }

    public async Task<List<ChatDto>> GetUserChatsAsync(Guid userId)
    {
        var userChats = await _unitOfWork.UserChatRepository.GetUserChatsWithDetailsAsync(userId);
        var result = new List<ChatDto>();

        foreach (var uc in userChats)
        {
            string chatName = "Group Chat";
            string chatImage = "https://localhost:7102/Images/DefaultAvatar.png";
            Guid? targetUserId = null;

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

            var allMessages = await _unitOfWork.MessageRepository.GetMessagesByChatIdAsync(uc.ChatId);

            int unreadCount = 0;
            foreach (var msg in allMessages)
            {
                if (msg.SenderId != userId)
                {
                    bool isRead = await _unitOfWork.MessageReadStatusRepository.IsReadAsync(msg.Id, userId);
                    if (!isRead)
                    {
                        unreadCount++;
                    }
                }
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
                LastMessageId = uc.Chat.LastMessage?.Id,
                UnreadCount = unreadCount 
            });
        }

        return result.OrderByDescending(c => c.LastMessageTime).ToList();
    }

    public async Task MarkChatAsReadAsync(Guid chatId, Guid userId)
    {
        var allMessages = await _unitOfWork.MessageRepository.GetMessagesByChatIdAsync(chatId);

        var receivedMessages = allMessages.Where(m => m.SenderId != userId);

        foreach (var msg in receivedMessages)
        {

            await _unitOfWork.MessageReadStatusRepository.CreateAsync(new MessageReadStatus
            {
                MessageId = msg.Id,
                UserProfileId = userId
            });
        }

        await _unitOfWork.SaveAsync();
    }

    public async Task<List<MessageDto>> GetChatHistoryAsync(Guid chatId, Guid currentUserId)
    {
        var membership = await _unitOfWork.UserChatRepository.GetByChatAndUserAsync(chatId, currentUserId);
        if (membership == null) throw new UnauthorizedAccessException("You are not a member of this chat.");

        await MarkChatAsReadAsync(chatId, currentUserId);

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
            throw new ArgumentException("Invalid content");

        var member = await _unitOfWork.UserChatRepository.GetByChatAndUserAsync(chatId, senderId);
        if (member == null) throw new UnauthorizedAccessException("Not a member");

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

        await _unitOfWork.MessageReadStatusRepository.CreateAsync(new MessageReadStatus
        {
            MessageId = message.Id,
            UserProfileId = senderId
        });

        await _unitOfWork.SaveAsync();

        var senderProfile = await _unitOfWork.UserProfileRepository.GetAllCredentialsAboutUserByIdAsync(senderId);

        var members = await _unitOfWork.UserChatRepository.GetChatMembersAsync(chatId);
        var recipients = members.Where(m => m.UserProfileId != senderId).ToList();

        foreach (var recipient in recipients)
        {
            bool isWatchingNow = _chatMembersTrackerService.IsUserActiveInChat(recipient.UserProfileId, chatId);

            bool isBrowsingList = _chatMembersTrackerService.IsUserBrowsingChats(recipient.UserProfileId);

            if (isWatchingNow || isBrowsingList)
            {
                continue; 
            }

            var unreadCount = await _unitOfWork.MessageRepository.CountUnreadMessagesAsync(chatId, recipient.UserProfileId);

            if (unreadCount == 1)
            {
                await _notificationService.SendNotificationAsync(
                    recipient.UserProfileId,
                    $"New message from {senderProfile?.ApplicationUser?.UserName ?? "Unknown"}",
                    $"Нове повідомлення від {senderProfile?.ApplicationUser?.UserName ?? "Unknown"}",
                    $"/Chat?openChatId={chatId}"
                );
            }
        }

        return new MessageDto
        {
            Id = message.Id,
            ChatId = chatId,
            SenderId = senderId,
            SenderName = senderProfile?.ApplicationUser?.UserName ?? "Unknown",
            SenderAvatarUrl = senderProfile?.Equipment?.Avatar?.ImageURL ?? "https://localhost:7102/Images/DefaultAvatar.png",
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