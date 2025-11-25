using Gymify.Application.DTOs.Comment;
using Gymify.Application.Services.Implementation;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ICommentRepository> _mockCommentRepo;
        private readonly Mock<IUserProfileRepository> _mockUserProfileRepo;

        private readonly CommentService _service;

        public CommentServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockCommentRepo = new Mock<ICommentRepository>();
            _mockUserProfileRepo = new Mock<IUserProfileRepository>();

            _mockUow.Setup(u => u.CommentRepository).Returns(_mockCommentRepo.Object);
            _mockUow.Setup(u => u.UserProfileRepository).Returns(_mockUserProfileRepo.Object);

            _service = new CommentService(_mockUow.Object);
        }

        [Fact]
        public async Task GetCommentDtos_ShouldReturnMappedComments_AndSetCanDeleteCorrectly()
        {
            // ARRANGE
            var currentUserId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var targetId = Guid.NewGuid();

            // Імітуємо дані з бази (з вкладеними об'єктами, ніби спрацював Include)
            var comments = new List<Comment>
            {
                new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = "My comment",
                    AuthorId = currentUserId, // Автор - поточний юзер
                    Author = new UserProfile
                    {
                        ApplicationUser = new ApplicationUser { UserName = "Me" },
                        Equipment = new UserEquipment { Avatar = new Item { ImageURL = "avatar1.png" } }
                    },
                    CreatedAt = DateTime.UtcNow
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = "Other comment",
                    AuthorId = otherUserId, // Автор - хтось інший
                    Author = new UserProfile
                    {
                        ApplicationUser = new ApplicationUser { UserName = "Other" },
                        Equipment = null // Перевіримо null-safety
                    },
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10)
                }
            };

            _mockCommentRepo.Setup(r => r.GetCommentsByTargetIdAndTypeAsync(targetId, CommentTargetType.Workout))
                .ReturnsAsync(comments);

            // ACT
            var result = await _service.GetCommentDtos(currentUserId, targetId, CommentTargetType.Workout);

            // ASSERT
            Assert.Equal(2, result.Count);

            // Перевірка першого коментаря (свого)
            var myComment = result.First(c => c.AuthorId == currentUserId);
            Assert.True(myComment.CanDelete); // Має бути true
            Assert.Equal("avatar1.png", myComment.AuthorAvatarUrl);

            // Перевірка другого коментаря (чужого)
            var otherComment = result.First(c => c.AuthorId == otherUserId);
            Assert.False(otherComment.CanDelete); // Має бути false
            Assert.Equal("/images/default-avatar.png", otherComment.AuthorAvatarUrl); // Дефолтна картинка
        }

        [Fact]
        public async Task UploadComment_ShouldCreateComment_AndReturnDto()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            string content = "Nice workout!";

            var userProfile = new UserProfile
            {
                Id = userId,
                ApplicationUser = new ApplicationUser { UserName = "TestUser" },
                Equipment = new UserEquipment { Avatar = new Item { ImageURL = "img.png" } }
            };

            _mockUserProfileRepo.Setup(r => r.GetAllCredentialsAboutUserByIdAsync(userId))
                .ReturnsAsync(userProfile);

            _mockCommentRepo.Setup(r => r.CreateAsync(It.IsAny<Comment>()))
                .ReturnsAsync((Comment c) => c);

            // ACT
            var result = await _service.UploadComment(userId, targetId, CommentTargetType.Workout, content);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(content, result.Content);
            Assert.Equal("TestUser", result.AuthorName);

            _mockCommentRepo.Verify(r => r.CreateAsync(It.Is<Comment>(c =>
                c.Content == content &&
                c.AuthorId == userId &&
                c.TargetId == targetId
            )), Times.Once);

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}