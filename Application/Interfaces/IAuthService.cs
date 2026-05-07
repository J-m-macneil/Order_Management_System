using Application.Features.Auth.Commands.LoginCommand;
using Application.Features.Auth.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginCommand request);
}