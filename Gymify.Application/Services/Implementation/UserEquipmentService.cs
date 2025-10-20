using Gymify.Application.DTOs.UserEquipment;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class UserEquipmentService(IUnitOfWork unitOfWork): IUserEquipmentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<UserEquipmentDto> GetUserEquipmentAsync(Guid userProfileId)
    {
        var userEquipment = await _unitOfWork.UserEquipmentRepository.GetByUserIdAsync(userProfileId);

        return new UserEquipmentDto
        {
            AvatarId = userEquipment.AvatarId,
            BackgroundId = userEquipment.BackgroundId,
            FrameId = userEquipment.FrameId,
            TitleId = userEquipment.TitleId,
        };
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
