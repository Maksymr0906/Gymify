using AutoMapper;
using Gymify.Application.DTOs.Item;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class ItemService(IUnitOfWork unitOfWork) : IItemService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ICollection<ItemDto>> GetAllUserItemsAsync(Guid userProfileId, bool ukranianVer)
    {
        var userItems = await _unitOfWork.ItemRepository.GetAllItemsByUserIdAsync(userProfileId);

        var itemDtos = userItems.Select(item => new ItemDto
        {
            Id = item.Id,
            Name = ukranianVer ? item.NameUk : item.NameEn,
            Description = ukranianVer ? item.DescriptionUk : item.DescriptionEn,
            ImageURL = item.ImageURL,
            Type = (int)item.Type,
            Rarity = (int)item.Rarity
        }).ToList();

        return itemDtos;
    }
    
    public async Task<ICollection<ItemDto>> GetUserItemsWithTypeAsync(Guid userProfileId, ItemType itemType, bool ukranianVer)
    {
        var userItems = await _unitOfWork.ItemRepository.GetItemsWithTypeByUserIdAsync(userProfileId, itemType);

        var itemDtos = userItems.Select(item => new ItemDto
        {
            Id = item.Id,
            Name = ukranianVer ? item.NameUk : item.NameEn,
            Description = ukranianVer ? item.DescriptionUk : item.DescriptionEn,
            ImageURL = item.ImageURL,
            Type = (int)item.Type,
            Rarity = (int)item.Rarity
        }).ToList();

        return itemDtos;
    }

    public async Task<ItemDto> GetByIdAsync(Guid itemId, bool ukranianVer)
    {
        var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);

        if (item == null)
        {
            throw new KeyNotFoundException($"Item with ID {itemId} not found.");
        }

        var itemDto = new ItemDto
        {
            Id = itemId,
            Name = ukranianVer ? item.NameUk : item.NameEn,
            Description = ukranianVer ? item.DescriptionUk : item.DescriptionEn,
            ImageURL = item.ImageURL,
            Type = (int)item.Type,
            Rarity = (int)item.Rarity
        };

        return itemDto;
    }

    public async Task SetDefaultUserItemsAsync(Guid userProfileId)
    {
        var userItems = new List<UserItem>
            {
                new UserItem()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserProfileId = userProfileId,
                    ItemId = Guid.Parse("f1a2b3c4-d5e6-4789-9012-abcdef123456")
                },
                new UserItem()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserProfileId = userProfileId,
                    ItemId = Guid.Parse("f2b3c4d5-e6f7-4890-1234-bcdef1234567")
                },
                new UserItem()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserProfileId = userProfileId,
                    ItemId = Guid.Parse("f3c4d5e6-a7b8-4901-2345-cdef12345678")
                },
                new UserItem()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserProfileId = userProfileId,
                    ItemId = Guid.Parse("f4d5e6f7-b8c9-5012-3456-def123456789")
                }
            };

        foreach (var userItem in userItems)
        {
            await _unitOfWork.UserItemRepository.CreateAsync(userItem);
        }

        await _unitOfWork.SaveAsync();
    }
}
