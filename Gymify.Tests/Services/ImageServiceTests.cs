using Gymify.Application.DTOs.Image;
using Gymify.Application.Services.Implementation;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class ImageServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IImageRepository> _mockImageRepo;
        private readonly ImageService _service;

        public ImageServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockImageRepo = new Mock<IImageRepository>();

            // Налаштовуємо UoW, щоб він повертав мок репозиторія картинок
            _mockUow.Setup(u => u.ImageRepository).Returns(_mockImageRepo.Object);

            _service = new ImageService(_mockUow.Object);
        }

        [Fact]
        public async Task CreateImageAsync_ShouldCreateImage_WhenDataIsValid()
        {
            // ARRANGE
            var uploadModel = new ImageUploadModel(
                "test.jpg",          // FileName
                ".jpg",              // FileExtension
                "Test Image",        // Title
                "some/local/path",   // LocalPath (додайте фейковий шлях)
                "/images/test.jpg",  // UrlPath
                new byte[] { 1, 2, 3 } // FileContent
            );

            // Мокаємо CreateAsync (просто повертає Task)
            _mockImageRepo.Setup(r => r.CreateAsync(It.IsAny<Image>()))
                .ReturnsAsync((Image i) => i);

            // ACT
            var result = await _service.CreateImageAsync(uploadModel);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(uploadModel.FileName, result.FileName);
            Assert.Equal(uploadModel.UrlPath, result.Url);

            // Перевіряємо, що ID згенерувався (не пустий)
            Assert.NotEqual(Guid.Empty, result.Id);

            // Перевіряємо виклик репозиторія
            _mockImageRepo.Verify(r => r.CreateAsync(It.Is<Image>(img =>
                img.FileName == uploadModel.FileName &&
                img.Url == uploadModel.UrlPath
            )), Times.Once);

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateImageAsync_ShouldThrowArgumentException_WhenFileContentIsMissing()
        {
            // ARRANGE
            var invalidModel = new ImageUploadModel(
                "empty.jpg",      // FileName
                ".jpg",           // FileExtension
                "Test Image",     // Title
                "some/path",      // LocalPath
                "/url/path",      // UrlPath
                null              // FileContent (те, що ми тестуємо - null)
            );

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CreateImageAsync(invalidModel));

            Assert.Equal("Invalid image file content.", ex.Message);

            // Переконуємось, що нічого не зберігалось
            _mockImageRepo.Verify(r => r.CreateAsync(It.IsAny<Image>()), Times.Never);
            _mockUow.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllImagesAsync_ShouldReturnListOfImageDtos()
        {
            // ARRANGE
            var images = new List<Image>
            {
                new Image { Id = Guid.NewGuid(), FileName = "img1.jpg", Url = "/url1" },
                new Image { Id = Guid.NewGuid(), FileName = "img2.jpg", Url = "/url2" }
            };

            _mockImageRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(images);

            // ACT
            var result = await _service.GetAllImagesAsync();

            // ASSERT
            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.Url == "/url1");
            Assert.Contains(result, dto => dto.Url == "/url2");
        }

        [Fact]
        public async Task GetImageByIdAsync_ShouldReturnDto_WhenImageExists()
        {
            // ARRANGE
            var imageId = Guid.NewGuid();
            var image = new Image { Id = imageId, FileName = "found.jpg" };

            _mockImageRepo.Setup(r => r.GetByIdAsync(imageId)).ReturnsAsync(image);

            // ACT
            var result = await _service.GetImageByIdAsync(imageId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(imageId, result.Id);
            Assert.Equal("found.jpg", result.FileName);
        }

        [Fact]
        public async Task GetImageByIdAsync_ShouldThrowKeyNotFoundException_WhenImageDoesNotExist()
        {
            // ARRANGE
            var imageId = Guid.NewGuid();

            _mockImageRepo.Setup(r => r.GetByIdAsync(imageId))
                .ReturnsAsync((Image)null); // Імітуємо, що нічого не знайдено

            // ACT & ASSERT
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetImageByIdAsync(imageId));
        }
    }
}