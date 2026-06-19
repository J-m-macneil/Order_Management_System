using Application.Features.Auth.Commands.LoginCommand;
using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string AuthCookieName = "__Host-back_auth";

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(new { message = "Invalid username/email or password." });
        }

        Response.Cookies.Append(AuthCookieName, result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = result.ExpiresAtUtc,
            Path = "/"
        });

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userIdValue = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        var username = User.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
            ?? User.Identity?.Name
            ?? string.Empty;

        var fullName = User.FindFirstValue("fullName") ?? username;
        var role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        return Ok(new AuthUserDto
        {
            UserId = int.TryParse(userIdValue, out var userId) ? userId : 0,
            Username = username,
            FullName = fullName,
            Role = role
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AuthCookieName, new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });

        return NoContent();
    }
}
