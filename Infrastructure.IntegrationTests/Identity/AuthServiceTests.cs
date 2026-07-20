using Application.Features.Auth.Commands.LoginCommand;
using Domain.Entities.Identity;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Persistence.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.IntegrationTests.Identity;

public class AuthServiceTests
{
    private const string Password = "SecurePassword123!";

    [Fact]
    public async Task LoginAsync_WithActiveUser_CreatesHashedRefreshToken()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var service = CreateService(context);
        var user = await AddUserAsync(context, isActive: true);

        // Act
        var result = await service.LoginAsync(CreateLogin(user), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();

        var storedToken = await context.RefreshTokens.SingleAsync();
        storedToken.TokenHash.Should().NotBe(result.RefreshToken);
        storedToken.TokenHash.Should().HaveLength(64);
        storedToken.RevokedAtUtc.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ReturnsNullAndCreatesNoToken()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var service = CreateService(context);
        var user = await AddUserAsync(context, isActive: false);

        // Act
        var result = await service.LoginAsync(CreateLogin(user), CancellationToken.None);

        // Assert
        result.Should().BeNull();
        (await context.RefreshTokens.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task LoginDemoAsync_CreatesSessionForReadOnlyDemoUser()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var service = CreateService(context);

        // Act
        var result = await service.LoginDemoAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.User.Role.Should().Be("Demo");
        result.User.Username.Should().Be("demo");
        (await context.RefreshTokens.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task LoginAsync_DoesNotAuthenticateDemoUserWithPassword()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var service = CreateService(context);

        // Act
        var result = await service.LoginAsync(new LoginCommand
        {
            UsernameOrEmail = "demo",
            Password = "Password123!"
        }, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WhenDemoReadOnlyModeIsEnabled_ReturnsNull()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var service = CreateService(context, demoReadOnlyMode: true);
        var user = await AddUserAsync(context, isActive: true);

        // Act
        var result = await service.LoginAsync(CreateLogin(user), CancellationToken.None);

        // Assert
        result.Should().BeNull();
        (await context.RefreshTokens.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task RefreshAsync_WithValidToken_RotatesTokenAndRejectsReuse()
    {
        // Arrange
        await using var context = await CreateContextAsync();
        var service = CreateService(context);
        var user = await AddUserAsync(context, isActive: true);
        var login = await service.LoginAsync(CreateLogin(user), CancellationToken.None);

        // Act
        var refreshed = await service.RefreshAsync(login!.RefreshToken, CancellationToken.None);
        var reused = await service.RefreshAsync(login.RefreshToken, CancellationToken.None);

        // Assert
        refreshed.Should().NotBeNull();
        refreshed!.RefreshToken.Should().NotBe(login.RefreshToken);
        reused.Should().BeNull();

        var storedTokens = await context.RefreshTokens
            .OrderBy(x => x.RefreshTokenId)
            .ToListAsync();

        storedTokens.Should().HaveCount(2);
        storedTokens[0].RevokedAtUtc.Should().NotBeNull();
        storedTokens[1].RevokedAtUtc.Should().BeNull();
    }

    private static AuthService CreateService(AppDbContext context, bool demoReadOnlyMode = false)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "Back.Tests",
                ["Jwt:Audience"] = "Back.Tests.Client",
                ["Jwt:SecretKey"] = new string('s', 64),
                ["Jwt:ExpiryMinutes"] = "15",
                ["Jwt:RefreshTokenExpiryDays"] = "7",
                ["Demo:ReadOnlyMode"] = demoReadOnlyMode.ToString()
            })
            .Build();

        var passwordService = new PasswordService();
        return new AuthService(
            context,
            passwordService,
            new JwtTokenService(configuration),
            configuration);
    }

    private static async Task<User> AddUserAsync(AppDbContext context, bool isActive)
    {
        var passwordService = new PasswordService();
        var identifier = Guid.NewGuid().ToString("N");
        var user = new User
        {
            FirstName = "Security",
            LastName = "Test",
            FullName = "Security Test",
            Email = $"security-{identifier}@back.test",
            Username = $"security-{identifier}",
            PasswordHash = passwordService.HashPassword(Password),
            RoleId = 1,
            DepartmentId = 1,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(CancellationToken.None);
        return user;
    }

    private static LoginCommand CreateLogin(User user)
    {
        return new LoginCommand
        {
            UsernameOrEmail = user.Username,
            Password = Password
        };
    }

    private static async Task<AppDbContext> CreateContextAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }
}
