namespace Gymify.Application.DTOs.Image;

public record ImageDto(
    Guid Id,
    string FileName,
    string FileExtension,
    string Title,
    string Url,
    DateTime DateCreated
);