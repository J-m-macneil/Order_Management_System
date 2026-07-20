using Application.Features.SystemSettings.DTOs;
using Domain.Entities.SystemSettings;
using Domain.Repositories;
using MediatR;

namespace Application.Features.SystemSettings.Queries.GetSystemSettings;

public class GetSystemSettingsQueryHandler : IRequestHandler<GetSystemSettingsQuery, List<SystemSettingDto>>
{
    private readonly ISystemSettingRepository _repo;

    public GetSystemSettingsQueryHandler(ISystemSettingRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SystemSettingDto>> Handle(GetSystemSettingsQuery request, CancellationToken ct)
    {
        var settings = await _repo.GetAllAsync(ct);
        return settings.Select(Map).ToList();
    }

    private static SystemSettingDto Map(SystemSetting setting)
    {
        return new SystemSettingDto
        {
            SystemSettingId = setting.SystemSettingId,
            SettingKey = setting.SettingKey,
            SettingValue = setting.SettingValue,
            DataType = setting.DataType,
            Description = setting.Description,
            CreatedAt = setting.CreatedAt,
            UpdatedAt = setting.UpdatedAt
        };
    }
}
