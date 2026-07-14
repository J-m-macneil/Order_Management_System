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
    private const string RefreshCookieName = "__Host-back_refresh";

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (result is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Invalid username/email or password."
            });
        }

        AppendSessionCookies(result);

        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken))
        {
            return UnauthorizedProblem();
        }

        var result = await _authService.RefreshAsync(refreshToken, cancellationToken);

        if (result is null)
        {
            DeleteSessionCookies();
            return UnauthorizedProblem();
        }

        AppendSessionCookies(result);
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
    [AllowAnonymous]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        DeleteSessionCookies();

        if (Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken))
        {
            await _authService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        }

        return NoContent();
    }

    private void AppendSessionCookies(LoginResponseDto session)
    {
        Response.Cookies.Append(AuthCookieName, session.Token, CreateCookieOptions(session.ExpiresAtUtc));
        Response.Cookies.Append(
            RefreshCookieName,
            session.RefreshToken,
            CreateCookieOptions(session.RefreshTokenExpiresAtUtc));
    }

    private void DeleteSessionCookies()
    {
        var options = CreateCookieOptions(null);
        Response.Cookies.Delete(AuthCookieName, options);
        Response.Cookies.Delete(RefreshCookieName, options);
    }

    private static CookieOptions CreateCookieOptions(DateTime? expiresAtUtc)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAtUtc,
            Path = "/"
        };
    }

    private UnauthorizedObjectResult UnauthorizedProblem()
    {
        return Unauthorized(new ProblemDetails
        {
            Title = "Unauthorized",
            Status = StatusCodes.Status401Unauthorized,
            Detail = "Your session is invalid or has expired."
        });
    }
}
