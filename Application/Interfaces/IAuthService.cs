using Application.Features.Auth.Commands.LoginCommand;
using Application.Features.Auth.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginCommand request, CancellationToken cancellationToken);
    Task<LoginResponseDto?> LoginDemoAsync(CancellationToken cancellationToken);
    Task<LoginResponseDto?> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
