using Gymify.Application.DTOs.Image;

namespace Gymify.Application.Services.Interfaces;

public interface IImageService
{
    Task<ImageDto> CreateImageAsync(ImageUploadModel model);
    Task<ICollection<ImageDto>> GetAllImagesAsync();
    Task<ImageDto> GetImageByIdAsync(Guid id);
}
