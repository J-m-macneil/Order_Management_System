namespace Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    List<string> Roles { get; }
}