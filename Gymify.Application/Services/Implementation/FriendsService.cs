using Gymify.Application.DTOs.Friends;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Gymify.Application.Services.Implementation;

public class FriendsService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IFriendsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    // ОНОВЛЕНИЙ ПОШУК
    public async Task<List<FriendDto>> SearchPotentialFriendsAsync(string query, Guid currentUserId)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<FriendDto>();

        // 1. Шукаємо всіх схожих
        var users = await _unitOfWork.UserProfileRepository.SearchUsersAsync(query, currentUserId);

        // 2. Фільтруємо Адмінів (через UserManager це найнадійніше)
        var nonAdminUsers = new List<UserProfile>();
        foreach (var u in users)
        {
            var appUser = await _userManager.FindByIdAsync(u.ApplicationUserId.ToString());
            if (appUser != null && !await _userManager.IsInRoleAsync(appUser, "Admin"))
            {
                nonAdminUsers.Add(u);
            }
        }

        // 3. Визначаємо статуси для кожного знайденого
        var results = new List<FriendDto>();

        foreach (var user in nonAdminUsers)
        {
            var status = UserRelationshipStatus.None;

            // А. Чи друзі?
            if (await _unitOfWork.FriendshipRepository.AreFriendsAsync(currentUserId, user.Id))
            {
                status = UserRelationshipStatus.Friend;
            }
            else
            {
                // Б. Чи є активна заявка?
                var invite = await _unitOfWork.FriendInviteRepository.GetInviteAnyDirectionAsync(currentUserId, user.Id);
                if (invite != null)
                {
                    status = invite.SenderProfileId == currentUserId
                        ? UserRelationshipStatus.RequestSent
                        : UserRelationshipStatus.RequestReceived;
                }
            }

            results.Add(new FriendDto
            {
                ProfileId = user.Id,
                UserName = user.ApplicationUser?.UserName ?? "Unknown",
                AvatarUrl = user.Equipment?.Avatar?.ImageURL ?? "/images/default.png",
                Level = user.Level,
                Status = status // <--- Передаємо статус на фронт
            });
        }

        return results;
    }

    // ОТРИМАННЯ ВИХІДНИХ
    public async Task<List<FriendDto>> GetOutgoingInvitesAsync(Guid userId)
    {
        var invites = await _unitOfWork.FriendInviteRepository.GetOutgoingInvitesAsync(userId);
        return invites.Select(i => new FriendDto
        {
            ProfileId = i.ReceiverProfileId, // ID того, кому відправили
            UserName = i.ReceiverProfile.ApplicationUser?.UserName ?? "Unknown",
            AvatarUrl = i.ReceiverProfile.Equipment?.Avatar?.ImageURL ?? "/images/default.png",
            SentAt = i.CreatedAt,
            Status = UserRelationshipStatus.RequestSent
        }).ToList();
    }

    // СКАСУВАННЯ ЗАЯВКИ (те саме, що Decline, але семантично для Cancel)
    public async Task CancelFriendRequestAsync(Guid receiverId, Guid currentUserId)
    {
        // Шукаємо заявку, де Я = Sender, Він = Receiver
        var invite = await _unitOfWork.FriendInviteRepository.GetInviteAsync(currentUserId, receiverId);
        if (invite == null) throw new Exception("Invite not found");

        await _unitOfWork.FriendInviteRepository.DeleteAsync(invite);
        await _unitOfWork.SaveAsync();
    }
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
            ReceiverProfileId = receiverId
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
            Type = ChatType.Private
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
            ChatId = chat.Id
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


    public async Task RemoveFriendAsync(Guid currentUserId, Guid friendId)
    {
        var friendship = await _unitOfWork.FriendshipRepository.GetByUsersAsync(currentUserId, friendId);

        if (friendship == null)
            throw new KeyNotFoundException("Friendship not found.");

        var chatId = friendship.ChatId;

        await _unitOfWork.FriendshipRepository.DeleteAsync(friendship);

        if (chatId != Guid.Empty)
        {
            var chat = await _unitOfWork.ChatRepository.GetByIdAsync(chatId);
            if (chat != null)
            {
                await _unitOfWork.ChatRepository.DeleteByIdAsync(chat.Id);
            }
        }

        await _unitOfWork.SaveAsync();
    }
}