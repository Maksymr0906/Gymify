using Gymify.Application.DTOs.Friends;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class FriendsService(IUnitOfWork unitOfWork) : IFriendsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    // 1. ВІДПРАВКА ЗАЯВКИ
    public async Task SendFriendRequestAsync(Guid senderId, Guid receiverId)
    {
        if (senderId == receiverId) throw new InvalidOperationException("You cannot add yourself.");

        var existingFriendship = await _unitOfWork.FriendshipRepository.GetByUsersAsync(senderId, receiverId);
        if (existingFriendship != null)
            throw new InvalidOperationException("You are already friends.");

        // Б. Чи вже існує заявка (в будь-яку сторону)?
        var existingInvite = await _unitOfWork.FriendInviteRepository.GetInviteAnyDirectionAsync(senderId, receiverId);

        if (existingInvite != null)
        {
            if (existingInvite.SenderProfileId == senderId)
                throw new InvalidOperationException("Request already sent.");
            else
                // Зустрічний інвайт -> приймаємо автоматично
                await AcceptFriendRequestAsync(existingInvite.SenderProfileId, senderId);
            return;
        }

        // В. Створюємо нову заявку
        var invite = new FriendInvite
        {
            SenderProfileId = senderId,
            ReceiverProfileId = receiverId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.FriendInviteRepository.CreateAsync(invite);
        await _unitOfWork.SaveAsync();
    }

    // 2. ПРИЙНЯТТЯ (Створює Дружбу + Чат)
    public async Task AcceptFriendRequestAsync(Guid senderId, Guid currentUserId)
    {
        var invite = await _unitOfWork.FriendInviteRepository.GetInviteAsync(senderId, currentUserId);
        if (invite == null) throw new KeyNotFoundException("Invite not found.");

        // А. Створюємо ЧАТ
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Type = ChatType.Private,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.ChatRepository.CreateAsync(chat);

        // Б. Додаємо учасників у ЧАТ
        var member1 = new UserChat { ChatId = chat.Id, UserProfileId = senderId };
        var member2 = new UserChat { ChatId = chat.Id, UserProfileId = currentUserId };

        await _unitOfWork.UserChatRepository.CreateAsync(member1);
        await _unitOfWork.UserChatRepository.CreateAsync(member2);

        // В. Створюємо ДРУЖБУ і прив'язуємо до чату
        var friendship = new Friendship
        {
            UserProfileId1 = senderId,
            UserProfileId2 = currentUserId,
            ChatId = chat.Id, // <--- ВАЖЛИВО
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.FriendshipRepository.CreateAsync(friendship);

        // Г. Видаляємо заявку
        await _unitOfWork.FriendInviteRepository.DeleteAsync(invite);

        await _unitOfWork.SaveAsync();
    }

    // 3. ВІДХИЛЕННЯ / СКАСУВАННЯ
    public async Task DeclineFriendRequestAsync(Guid senderId, Guid currentUserId)
    {
        // Шукаємо або вхідний (я відхиляю), або вихідний (я скасовую)
        var invite = await _unitOfWork.FriendInviteRepository.GetInviteAnyDirectionAsync(senderId, currentUserId);

        if (invite == null) throw new KeyNotFoundException("Invite not found.");

        await _unitOfWork.FriendInviteRepository.DeleteAsync(invite);
        await _unitOfWork.SaveAsync();
    }

    // 4. ОТРИМАННЯ СПИСКУ ДРУЗІВ (DTO)
    public async Task<List<FriendDto>> GetFriendsAsync(Guid userId)
    {
        // Цей метод треба додати в репозиторій, або реалізувати логіку тут
        // Припустимо, репозиторій повертає список Friendship, де є userId
        var friendships = await _unitOfWork.FriendshipRepository.GetAllForUserAsync(userId);

        var friends = new List<FriendDto>();

        foreach (var f in friendships)
        {
            // Визначаємо, хто з двох є "другом" (не я)
            var friendProfile = f.UserProfileId1 == userId ? f.UserProfile2 : f.UserProfile1;

            friends.Add(new FriendDto
            {
                ProfileId = friendProfile.Id,
                UserName = friendProfile.ApplicationUser?.UserName ?? "Unknown",
                AvatarUrl = friendProfile.Equipment?.Avatar?.ImageURL ?? "/images/default.png",
                ChatId = f.ChatId,
                Level = friendProfile.Level
            });
        }

        return friends;
    }

    // 5. ОТРИМАННЯ ВХІДНИХ ЗАЯВОК
    public async Task<List<FriendDto>> GetIncomingInvitesAsync(Guid userId)
    {
        var invites = await _unitOfWork.FriendInviteRepository.GetIncomingInvitesAsync(userId);

        return invites.Select(i => new FriendDto
        {
            ProfileId = i.SenderProfileId,
            UserName = i.SenderProfile.ApplicationUser?.UserName ?? "Unknown",
            AvatarUrl = i.SenderProfile.Equipment?.Avatar?.ImageURL ?? "/images/default.png",
            SentAt = i.CreatedAt
        }).ToList();
    }
}