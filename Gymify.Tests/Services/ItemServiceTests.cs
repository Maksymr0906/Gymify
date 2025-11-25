using Gymify.Application.DTOs.Item;
using Gymify.Application.Services.Implementation;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class ItemServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IItemRepository> _mockItemRepo;
        private readonly Mock<IUserItemRepository> _mockUserItemRepo;

        private readonly ItemService _service;

        public ItemServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockItemRepo = new Mock<IItemRepository>();
            _mockUserItemRepo = new Mock<IUserItemRepository>();

            // Налаштування UoW
            _mockUow.Setup(u => u.ItemRepository).Returns(_mockItemRepo.Object);
            _mockUow.Setup(u => u.UserItemRepository).Returns(_mockUserItemRepo.Object);

            _service = new ItemService(_mockUow.Object);
        }

        [Fact]
        public async Task GetAllUserItemsAsync_ShouldReturnMappedDtos()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var items = new List<Item>
            {
                new Item { Id = Guid.NewGuid(), Name = "Sword", Type = ItemType.Avatar, Rarity = ItemRarity.Common },
                new Item { Id = Guid.NewGuid(), Name = "Shield", Type = ItemType.Frame, Rarity = ItemRarity.Rare }
            };

            _mockItemRepo.Setup(r => r.GetAllItemsByUserIdAsync(userId))
                .ReturnsAsync(items);

            // ACT
            var result = await _service.GetAllUserItemsAsync(userId);

            // ASSERT
            Assert.Equal(2, result.Count);
            Assert.Contains(result, i => i.Name == "Sword" && i.Type == (int)ItemType.Avatar);
            Assert.Contains(result, i => i.Name == "Shield" && i.Type == (int)ItemType.Frame);
        }

        [Fact]
        public async Task GetUserItemsWithTypeAsync_ShouldReturnFilteredItems()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var type = ItemType.Avatar;

            var items = new List<Item>
            {
                new Item { Id = Guid.NewGuid(), Name = "Avatar 1", Type = ItemType.Avatar }
            };

            // Мокаємо саме метод з фільтрацією по типу
            _mockItemRepo.Setup(r => r.GetItemsWithTypeByUserIdAsync(userId, type))
                .ReturnsAsync(items);

            // ACT
            var result = await _service.GetUserItemsWithTypeAsync(userId, type);

            // ASSERT
            Assert.Single(result);
            Assert.Equal("Avatar 1", result.First().Name);

            // Перевіряємо, що викликався правильний метод репозиторію
            _mockItemRepo.Verify(r => r.GetItemsWithTypeByUserIdAsync(userId, type), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenItemExists()
        {
            // ARRANGE
            var itemId = Guid.NewGuid();
            var item = new Item
            {
                Id = itemId,
                Name = "Unique Item",
                Description = "Desc",
                ImageURL = "url.png",
                Type = ItemType.Background,
                Rarity = ItemRarity.Legendary
            };

            _mockItemRepo.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(item);

            // ACT
            var result = await _service.GetByIdAsync(itemId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(item.Id, result.Id);
            Assert.Equal(item.Name, result.Name);
            Assert.Equal((int)item.Type, result.Type);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenItemNotFound()
        {
            // ARRANGE
            var itemId = Guid.NewGuid();
            _mockItemRepo.Setup(r => r.GetByIdAsync(itemId))
                .ReturnsAsync((Item)null); // Репозиторій повернув null

            // ACT & ASSERT
            // ❗ Ми змінили тип очікуваної помилки на KeyNotFoundException
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetByIdAsync(itemId));

            // (Опціонально) Можна навіть перевірити текст помилки
            Assert.Contains(itemId.ToString(), exception.Message);
        }

        [Fact]
        public async Task SetDefaultUserItemsAsync_ShouldAddFixedAmountOfItems()
        {
            // ARRANGE
            var userId = Guid.NewGuid();

            // Налаштовуємо CreateAsync, щоб він просто повертав Task (успіх)
            _mockUserItemRepo.Setup(r => r.CreateAsync(It.IsAny<UserItem>()))
                .ReturnsAsync((UserItem ui) => ui);

            // ACT
            await _service.SetDefaultUserItemsAsync(userId);

            // ASSERT
            // У твоєму коді хардкодом прописано 4 предмети. Перевіряємо, що ми додали саме 4.
            _mockUserItemRepo.Verify(r => r.CreateAsync(It.Is<UserItem>(ui =>
                ui.UserProfileId == userId &&
                ui.ItemId != Guid.Empty // Перевіряємо, що ID предмета валідний
            )), Times.Exactly(4));

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}