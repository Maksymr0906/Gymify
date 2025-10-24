using Gymify.Application.DTOs.Item;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class ItemService(IUnitOfWork unitOfWork) : IItemService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ICollection<ItemDto>> GetAllUserItemsAsync(Guid userProfileId)
    {
        var userItems = await _unitOfWork.ItemRepository.GetAllItemsByUserIdAsync(userProfileId);

        var itemDtos = userItems.Select(item => new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            ImageURL = item.ImageURL,
            Type = (int)item.Type,
            Rarity = (int)item.Rarity
        }).ToList();

        return itemDtos;
    }

    public async Task<ItemDto> GetByIdAsync(Guid itemId)
    {
        var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);

        var itemDto = new ItemDto
        {
            Id = itemId,
            Name = item.Name,
            Description = item.Description,
            ImageURL = item.ImageURL,
            Type = (int)item.Type,
            Rarity = (int)item.Rarity
        };

        return itemDto;
    }
}
