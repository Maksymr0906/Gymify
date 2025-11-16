using Gymify.Application.DTOs.Auth;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Gymify.Application.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
    }

    // Створювати UserEquipment, Створювати звязки зі всіма Achievements з прогресом 0, додати поле юзернейма
    public async Task<IdentityResult> RegisterAsync(RegisterRequestDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            var profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                CurrentXP = 0,
                Level = 1
            };

            await _unitOfWork.UserProfileRepository.CreateAsync(profile);
            await _unitOfWork.SaveAsync();

            user.UserProfileId = profile.Id;
            await _userManager.UpdateAsync(user);

            // add default avatar, background, frame to useritems relation
            var userItems = new List<UserItem> 
            {
                new UserItem()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserProfileId = profile.Id,
                    ItemId = Guid.Parse("f1a2b3c4-d5e6-4789-9012-abcdef123456")
                },
                new UserItem()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserProfileId = profile.Id,
                    ItemId = Guid.Parse("f2b3c4d5-e6f7-4890-1234-bcdef1234567")
                },
                new UserItem()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserProfileId = profile.Id,
                    ItemId = Guid.Parse("f3c4d5e6-a7b8-4901-2345-cdef12345678")
                },
                new UserItem()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserProfileId = profile.Id,
                    ItemId = Guid.Parse("f4d5e6f7-b8c9-5012-3456-def123456789")
                }
            };

            foreach (var userItem in userItems) {
                await _unitOfWork.UserItemRepository.CreateAsync(userItem);
            }

            await _unitOfWork.SaveAsync();

            // add userequioment reltaion with base 
            
            var userEquipment = new UserEquipment
            {
                Id = Guid.NewGuid(),
                UserProfileId = profile.Id,
                CreatedAt = DateTime.UtcNow,
                AvatarId = Guid.Parse("f3c4d5e6-a7b8-4901-2345-cdef12345678"),
                BackgroundId = Guid.Parse("f2b3c4d5-e6f7-4890-1234-bcdef1234567"),
                FrameId = Guid.Parse("f1a2b3c4-d5e6-4789-9012-abcdef123456"),
                TitleId = Guid.Parse("f4d5e6f7-b8c9-5012-3456-def123456789"),
            };

            await _unitOfWork.UserEquipmentRepository.CreateAsync(userEquipment);
            await _unitOfWork.SaveAsync();


            // add all achievements relation
            var achievements = await _unitOfWork.AchievementRepository.GetAllAsync();

            var userAchievements = achievements.Select(a => new UserAchievement
            {
                UserProfileId = profile.Id,
                AchievementId = a.Id,
                Progress = 0,
                IsCompleted = false,
                UnlockedAt = DateTime.UtcNow,
            }).ToList();

            foreach (var ua in userAchievements)
            {
                await _unitOfWork.UserAchievementRepository.CreateAsync(ua);
            }

            await _unitOfWork.SaveAsync();

            await _userManager.AddToRoleAsync(user, "User");

            var claims = new List<Claim>
            {
                new Claim("UserProfileId", profile.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            await _userManager.AddClaimsAsync(user, claims);
            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        return result;
    }

    public async Task<SignInResult> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return SignInResult.Failed;

        var result = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false);

        return result;
    }


    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
