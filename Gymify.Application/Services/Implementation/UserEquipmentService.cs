using AutoMapper;
using Gymify.Application.DTOs.UserEquipment;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Application.ViewModels.UserProfile;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class UserEquipmentService(IUnitOfWork unitOfWork) : IUserEquipmentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;


    public async Task<UserEquipmentDto> GetUserEquipmentAsync(Guid userProfileId, bool ukranianVer)
    {
        var userEquipment = await _unitOfWork.UserEquipmentRepository.GetByUserIdAsync(userProfileId);

        var imageIds = new List<Guid>()
        {
            userEquipment.AvatarId,
            userEquipment.BackgroundId,
            userEquipment.FrameId,
        };

        var userItems = await _unitOfWork.ItemRepository.GetByListOfIdAsync(imageIds);

        var userTitle = await _unitOfWork.ItemRepository.GetByIdAsync(userEquipment.TitleId);

        var imagesDict = userItems.ToDictionary(itm => itm.Id, itm => itm.ImageURL);

        return new UserEquipmentDto
        {
            AvatarId = userEquipment.AvatarId,
            AvatarUrl = imagesDict.TryGetValue(userEquipment.AvatarId, out string? avatar) ? avatar : string.Empty,

            BackgroundId = userEquipment.BackgroundId,
            BackgroundUrl = imagesDict.TryGetValue(userEquipment.BackgroundId, out string? back) ? back : string.Empty,

            FrameId = userEquipment.FrameId,
            FrameUrl = imagesDict.TryGetValue(userEquipment.FrameId, out string? frame) ? frame : string.Empty,

            TitleId = userEquipment.TitleId,
            TitleText = ukranianVer ? userTitle.NameUk : userTitle.NameEn
        };
    }

    public async Task SetDefaultEquipment(Guid userProfileId)
    {
        var userEquipment = new UserEquipment
        {
            Id = Guid.NewGuid(),
            UserProfileId = userProfileId,
            AvatarId = Guid.Parse("f3c4d5e6-a7b8-4901-2345-cdef12345678"),
            BackgroundId = Guid.Parse("f2b3c4d5-e6f7-4890-1234-bcdef1234567"),
            FrameId = Guid.Parse("f1a2b3c4-d5e6-4789-9012-abcdef123456"),
            TitleId = Guid.Parse("f4d5e6f7-b8c9-5012-3456-def123456789")
        };

        await _unitOfWork.UserEquipmentRepository.CreateAsync(userEquipment);
        await _unitOfWork.SaveAsync();
    }

    public async Task UpdateUserEquipmentAsync(Guid userProfileId, UpdateUserEquipmentDto model)
    {
        var userEquipment = await _unitOfWork.UserEquipmentRepository.GetByUserIdAsync(userProfileId);

        async Task ValidateOwnership(Guid? itemId)
        {
            if (itemId == null) return;

            bool owns = await _unitOfWork.ItemRepository.IsOwnedByUserAsync(itemId.Value, userProfileId);

            if (!owns)
                throw new InvalidOperationException($"User does not own item with ID {itemId}");
        }

        await ValidateOwnership(model.AvatarId);
        await ValidateOwnership(model.BackgroundId);
        await ValidateOwnership(model.FrameId);
        await ValidateOwnership(model.TitleId);

        if (model.AvatarId != null)
            userEquipment.AvatarId = model.AvatarId.Value;

        if (model.BackgroundId != null)
            userEquipment.BackgroundId = model.BackgroundId.Value;

        if (model.FrameId != null)
            userEquipment.FrameId = model.FrameId.Value;

        if (model.TitleId != null)
            userEquipment.TitleId = model.TitleId.Value;

        await _unitOfWork.UserEquipmentRepository.UpdateAsync(userEquipment);
        await _unitOfWork.SaveAsync();

    }
}
