using Gymify.Application.DTOs.Image;
using Gymify.Application.DTOs.Item;
using Gymify.Data.Enums;
using Microsoft.AspNetCore.Http;

namespace Gymify.Application.Services.Interfaces;

public interface IItemService
{
    Task<ICollection<ItemDto>> GetAllUserItemsAsync(Guid userProfileId, bool onlyOffical, bool ukranianVer);
    Task<ICollection<ItemDto>> GetUserItemsWithTypeAsync(Guid userProfileId, ItemType itemType, bool onlyUnique, bool ukranianVer);
    Task<ItemDto> GetByIdAsync(Guid itemId, bool ukranianVer);
    Task SetDefaultUserItemsAsync(Guid userProfileId);
    Task<ItemDto> CreateCustomAvatarAsync(Guid userId, ImageUploadModel imageUploadModel, bool ukranianVer);
}
