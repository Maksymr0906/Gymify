using AutoMapper;
using Gymify.Application.DTOs.Image;
using Gymify.Application.DTOs.Item;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Application.Services.Implementation;

public class ItemService(IUnitOfWork unitOfWork, IImageService imageService) : IItemService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IImageService _imageService = imageService;

    public async Task<ICollection<ItemDto>> GetAllUserItemsAsync(Guid userProfileId, bool onlyOffical, bool ukranianVer)
    {
        var userItems = await _unitOfWork.ItemRepository.GetAllItemsByUserIdAsync(userProfileId, onlyOffical);

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
    
    public async Task<ICollection<ItemDto>> GetUserItemsWithTypeAsync(Guid userProfileId, ItemType itemType, bool onlyUnique, bool ukranianVer)
    {
        var userItems = await _unitOfWork.ItemRepository.GetItemsWithTypeByUserIdAsync(userProfileId, itemType);

        if (onlyUnique) userItems = userItems.DistinctBy(u => u.Id).ToList();

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
                    UserProfileId = userProfileId,
                    ItemId = Guid.Parse("f1a2b3c4-d5e6-4789-9012-abcdef123456")
                },
                new UserItem()
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = userProfileId,
                    ItemId = Guid.Parse("f2b3c4d5-e6f7-4890-1234-bcdef1234567")
                },
                new UserItem()
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = userProfileId,
                    ItemId = Guid.Parse("f3c4d5e6-a7b8-4901-2345-cdef12345678")
                },
                new UserItem()
                {
                    Id = Guid.NewGuid(),
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

    public async Task<ItemDto> CreateCustomAvatarAsync(Guid userId, ImageUploadModel imageUploadModel, bool ukranianVer)
    {

        var imageModel = await _imageService.CreateImageAsync(imageUploadModel);

        var newItem = new Item
        {
            Id = Guid.NewGuid(),
            NameEn = $"Custom item {imageModel.FileName}", 
            NameUk = $"Особистий предмет {imageModel.FileName}",
            ImageURL = imageModel.Url,
            Type = ItemType.Avatar,
            IsCustom = true,
            CreatorUserId = userId 
        };

        await _unitOfWork.ItemRepository.CreateAsync(newItem);
        var userItem = new UserItem
        {
            UserProfileId = userId,
            ItemId = newItem.Id
        };

        await _unitOfWork.UserItemRepository.CreateAsync(userItem);

        await _unitOfWork.SaveAsync();

        return new ItemDto
        {
            Id = newItem.Id,
            Name = ukranianVer ? newItem.NameUk : newItem.NameEn,
            ImageURL = newItem.ImageURL,
            Type = (int)newItem.Type
        };
    }
}
