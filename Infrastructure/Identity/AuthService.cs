using Application.Features.Auth.Commands.LoginCommand;
using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Entities.Identity;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly int _refreshTokenExpiryDays;

    public AuthService(
        AppDbContext dbContext,
        IPasswordService passwordService,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenExpiryDays = GetRefreshTokenExpiryDays(configuration);
    }

    public async Task<LoginResponseDto?> LoginAsync(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedInput = request.UsernameOrEmail.Trim();

        var user = await _dbContext.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x =>
                x.Username == normalizedInput ||
                x.Email == normalizedInput,
                cancellationToken);

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

        user.LastLoginAt = DateTime.UtcNow;
        var result = CreateSession(user);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }

    public async Task<LoginResponseDto?> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var tokenHash = HashToken(refreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .Include(x => x.User)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null ||
            storedToken.RevokedAtUtc != null ||
            storedToken.ExpiresAtUtc <= now ||
            !storedToken.User.IsActive)
        {
            return null;
        }

        storedToken.RevokedAtUtc = now;
        var result = CreateSession(storedToken.User);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }

    public async Task RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.TokenHash == tokenHash &&
                x.RevokedAtUtc == null,
                cancellationToken);

        if (storedToken is null)
        {
            return;
        }

        storedToken.RevokedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private LoginResponseDto CreateSession(User user)
    {
        var now = DateTime.UtcNow;
        var roleName = user.Role?.Name ?? string.Empty;
        var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user, roleName);
        var refreshToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        var refreshTokenExpiresAtUtc = now.AddDays(_refreshTokenExpiryDays);

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = HashToken(refreshToken),
            CreatedAtUtc = now,
            ExpiresAtUtc = refreshTokenExpiresAtUtc
        });

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
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

    private static int GetRefreshTokenExpiryDays(IConfiguration configuration)
    {
        return int.TryParse(configuration["Jwt:RefreshTokenExpiryDays"], out var days)
            && days > 0
                ? days
                : 7;
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
