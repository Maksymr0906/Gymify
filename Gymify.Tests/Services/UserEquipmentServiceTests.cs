using Gymify.Application.DTOs.UserEquipment;
using Gymify.Application.Services.Implementation;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class UserEquipmentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IUserEquipmentRepository> _mockEquipmentRepo;
        private readonly Mock<IItemRepository> _mockItemRepo;
        private readonly UserEquipmentService _service;

        public UserEquipmentServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockEquipmentRepo = new Mock<IUserEquipmentRepository>();
            _mockItemRepo = new Mock<IItemRepository>();

            // Налаштування UoW
            _mockUow.Setup(u => u.UserEquipmentRepository).Returns(_mockEquipmentRepo.Object);
            _mockUow.Setup(u => u.ItemRepository).Returns(_mockItemRepo.Object);

            _service = new UserEquipmentService(_mockUow.Object);
        }

        [Fact]
        public async Task GetUserEquipmentAsync_ShouldReturnCorrectDto()
        {
            // ARRANGE
            var userId = Guid.NewGuid();

            var avatarId = Guid.NewGuid();
            var frameId = Guid.NewGuid();
            var bgId = Guid.NewGuid();
            var titleId = Guid.NewGuid();

            // 1. Імітуємо запис в БД про екіпірування юзера
            var equipmentEntity = new UserEquipment
            {
                UserProfileId = userId,
                AvatarId = avatarId,
                FrameId = frameId,
                BackgroundId = bgId,
                TitleId = titleId
            };

            _mockEquipmentRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(equipmentEntity);

            // 2. Імітуємо отримання предметів-картинок (Avatar, Frame, Bg)
            var imageItems = new List<Item>
            {
                new Item { Id = avatarId, ImageURL = "avatar.png" },
                new Item { Id = frameId, ImageURL = "frame.png" },
                new Item { Id = bgId, ImageURL = "bg.png" }
            };

            // Важливо: It.Is<List<Guid>> перевіряє, чи список містить потрібні ID
            _mockItemRepo.Setup(r => r.GetByListOfIdAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(imageItems);

            // 3. Імітуємо отримання титулу (окремий запит в твоєму сервісі)
            var titleItem = new Item { Id = titleId, Name = "Gym Boss" };
            _mockItemRepo.Setup(r => r.GetByIdAsync(titleId))
                .ReturnsAsync(titleItem);

            // ACT
            var result = await _service.GetUserEquipmentAsync(userId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(avatarId, result.AvatarId);
            Assert.Equal("avatar.png", result.AvatarUrl);

            Assert.Equal(bgId, result.BackgroundId);
            Assert.Equal("bg.png", result.BackgroundUrl);

            Assert.Equal(titleId, result.TitleId);
            Assert.Equal("Gym Boss", result.TitleText);
        }

        [Fact]
        public async Task SetDefaultEquipment_ShouldCreateEntry_WithHardcodedIds()
        {
            // ARRANGE
            var userId = Guid.NewGuid();

            // Налаштовуємо CreateAsync
            _mockEquipmentRepo.Setup(r => r.CreateAsync(It.IsAny<UserEquipment>()))
                .ReturnsAsync((UserEquipment ue) => ue);

            // ACT
            await _service.SetDefaultEquipment(userId);

            // ASSERT
            // Перевіряємо, що створився запис саме з тими GUID, які захардкоджені в сервісі
            _mockEquipmentRepo.Verify(r => r.CreateAsync(It.Is<UserEquipment>(ue =>
                ue.UserProfileId == userId &&
                ue.AvatarId == Guid.Parse("f3c4d5e6-a7b8-4901-2345-cdef12345678") &&
                ue.BackgroundId == Guid.Parse("f2b3c4d5-e6f7-4890-1234-bcdef1234567") &&
                ue.FrameId == Guid.Parse("f1a2b3c4-d5e6-4789-9012-abcdef123456") &&
                ue.TitleId == Guid.Parse("f4d5e6f7-b8c9-5012-3456-def123456789")
            )), Times.Once);

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserEquipmentAsync_ShouldUpdate_WhenUserOwnsItems()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var newAvatarId = Guid.NewGuid();

            var dto = new UpdateUserEquipmentDto
            {
                AvatarId = newAvatarId,
                BackgroundId = null // Не міняємо фон
            };

            var existingEquipment = new UserEquipment
            {
                UserProfileId = userId,
                AvatarId = Guid.NewGuid(), // Старий аватар
                BackgroundId = Guid.NewGuid()
            };

            _mockEquipmentRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(existingEquipment);

            // Імітуємо, що юзер володіє новим аватаром
            _mockItemRepo.Setup(r => r.IsOwnedByUserAsync(newAvatarId, userId))
                .ReturnsAsync(true);

            // ACT
            await _service.UpdateUserEquipmentAsync(userId, dto);

            // ASSERT
            // Аватар мав змінитись
            Assert.Equal(newAvatarId, existingEquipment.AvatarId);
            // Фон НЕ мав змінитись (бо в DTO null)
            Assert.NotEqual(Guid.Empty, existingEquipment.BackgroundId);

            _mockEquipmentRepo.Verify(r => r.UpdateAsync(existingEquipment), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserEquipmentAsync_ShouldThrow_WhenUserDoesNotOwnItem()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var stolenAvatarId = Guid.NewGuid();

            var dto = new UpdateUserEquipmentDto { AvatarId = stolenAvatarId };

            var existingEquipment = new UserEquipment { UserProfileId = userId };

            _mockEquipmentRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(existingEquipment);

            // Імітуємо, що юзер НЕ володіє предметом
            _mockItemRepo.Setup(r => r.IsOwnedByUserAsync(stolenAvatarId, userId))
                .ReturnsAsync(false);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateUserEquipmentAsync(userId, dto));

            Assert.Contains($"User does not own item with ID {stolenAvatarId}", ex.Message);

            // Перевіряємо, що збереження НЕ викликалось
            _mockEquipmentRepo.Verify(r => r.UpdateAsync(It.IsAny<UserEquipment>()), Times.Never);
            _mockUow.Verify(u => u.SaveAsync(), Times.Never);
        }
    }
}