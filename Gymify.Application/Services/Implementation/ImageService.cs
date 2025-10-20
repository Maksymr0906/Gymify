using Gymify.Application.DTOs.Image;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class ImageService(IUnitOfWork unitOfWork) : IImageService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ImageDto> CreateImageAsync(ImageUploadModel model)
    {
        if (model.FileContent == null || model.FileContent.Length == 0)
            throw new ArgumentException("Invalid image file content.");

        var imageEntity = new Image
        {
            Id = Guid.NewGuid(),
            FileName = model.FileName,
            FileExtension = model.FileExtension,
            Title = model.Title,
            Url = model.UrlPath,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ImageRepository.CreateAsync(imageEntity);
        await _unitOfWork.SaveAsync();

        return new ImageDto(
            imageEntity.Id,
            imageEntity.FileName,
            imageEntity.FileExtension,
            imageEntity.Title,
            imageEntity.Url,
            imageEntity.CreatedAt
        );
    }

    public async Task<ICollection<ImageDto>> GetAllImagesAsync()
    {
        var images = await _unitOfWork.ImageRepository.GetAllAsync();

        return images.Select(image => new ImageDto(
            image.Id,
            image.FileName,
            image.FileExtension,
            image.Title,
            image.Url,
            image.CreatedAt
        )).ToList();
    }

    public async Task<ImageDto> GetImageByIdAsync(Guid id)
    {
        var image = await _unitOfWork.ImageRepository.GetByIdAsync(id);

        if (image == null)
            throw new KeyNotFoundException($"Image with ID '{id}' was not found.");

        return new ImageDto(
            image.Id,
            image.FileName,
            image.FileExtension,
            image.Title,
            image.Url,
            image.CreatedAt
        );
    }
}
