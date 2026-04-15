using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        AppDbContext dbContext,
        IPasswordService passwordService,
        IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var normalizedInput = request.UsernameOrEmail.Trim();

        var user = await _dbContext.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x =>
                x.Username == normalizedInput ||
                x.Email == normalizedInput);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var passwordValid = _passwordService.VerifyPassword(
            user.PasswordHash,
            request.Password);

        if (!passwordValid)
        {
            return null;
        }

        var roleName = user.Role?.Name ?? string.Empty;

        var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user, roleName);

        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            User = new AuthUserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Role = roleName
            }
        };
    }
}