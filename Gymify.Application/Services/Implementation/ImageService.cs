using Gymify.Application.DTOs.Image;
using Gymify.Application.Services.Interfaces;

namespace Gymify.Application.Services.Implementation;

public class ImageService : IImageService
{
    public Task<ImageDto> CreateImageAsync(ImageUploadModel model)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<ImageDto>> GetAllImagesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ImageDto> GetImageByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
