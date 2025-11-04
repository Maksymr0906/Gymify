using Gymify.Application.DTOs.Auth;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _authService.RegisterAsync(dto);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(dto);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _authService.LoginAsync(dto);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ModelState.AddModelError("", "Invalid login attempt.");
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Start");
    }
}
