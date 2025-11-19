using Gymify.Application.DTOs.UserEquipment;
using Gymify.Application.ViewModels.UserProfile;

namespace Gymify.Application.Services.Interfaces;

public interface IUserEquipmentService
{
    Task<UserEquipmentDto> GetUserEquipmentAsync(Guid userProfileId);
    Task UpdateUserEquipmentAsync(Guid userProfileId, UpdateUserEquipmentDto model);
    Task SetDefaultEquipment(Guid userProfileId);
}
