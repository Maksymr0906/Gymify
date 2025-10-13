using Gymify.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace Gymify.Application.Services.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterRequestDto dto);
    Task<SignInResult> LoginAsync(LoginRequestDto dto);
    Task LogoutAsync();
}
