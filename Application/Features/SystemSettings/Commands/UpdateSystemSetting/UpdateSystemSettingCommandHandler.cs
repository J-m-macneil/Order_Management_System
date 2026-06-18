using Application.Interfaces;
using Domain.Entities.SystemSettings;
using Domain.Repositories;
using MediatR;

namespace Application.Features.SystemSettings.Commands.UpdateSystemSetting;

public class UpdateSystemSettingCommandHandler : IRequestHandler<UpdateSystemSettingRequest, Unit>
{
    private readonly ISystemSettingRepository _repo;
    private readonly IAuditService _audit;
    private readonly IAuditChangeFormatter _changeFormatter;

    public UpdateSystemSettingCommandHandler(
        ISystemSettingRepository repo,
        IAuditService audit,
        IAuditChangeFormatter changeFormatter)
    {
        _repo = repo;
        _audit = audit;
        _changeFormatter = changeFormatter;
    }

    public async Task<Unit> Handle(UpdateSystemSettingRequest request, CancellationToken ct)
    {
        var setting = await _repo.GetByIdAsync(request.SystemSettingId, ct);

        if (setting is null)
        {
            throw new InvalidOperationException("System setting not found.");
        }

        var newValue = NormalizeAndValidate(request.Data.SettingValue, setting.DataType, setting.SettingKey);
        var oldValues = CreateSnapshot(setting);

        setting.SettingValue = newValue;
        setting.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);

        var newValues = CreateSnapshot(setting);
        var changes = _changeFormatter.GetChanges(oldValues, newValues);

        await _audit.LogAsync(
            "SystemSetting",
            setting.SystemSettingId,
            "Updated",
            oldValues,
            newValues,
            _changeFormatter.CreateUpdateNote("System setting", setting.SettingKey, changes),
            ct);

        return Unit.Value;
    }

    private static string NormalizeAndValidate(string? value, string dataType, string settingKey)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{settingKey} requires a value.");
        }

        var normalized = value.Trim();

        switch (dataType.Trim().ToLowerInvariant())
        {
            case "boolean":
                if (!bool.TryParse(normalized, out var boolValue))
                {
                    throw new InvalidOperationException($"{settingKey} must be true or false.");
                }

                return boolValue.ToString().ToLowerInvariant();

            case "integer":
                if (!int.TryParse(normalized, out var intValue))
                {
                    throw new InvalidOperationException($"{settingKey} must be a whole number.");
                }

                return intValue.ToString();

            case "decimal":
                if (!decimal.TryParse(normalized, out var decimalValue))
                {
                    throw new InvalidOperationException($"{settingKey} must be a decimal number.");
                }

                return decimalValue.ToString("0.##");

            case "string":
                return normalized;

            default:
                throw new InvalidOperationException($"{settingKey} has an unsupported data type.");
        }
    }

    private static object CreateSnapshot(SystemSetting setting)
    {
        return new
        {
            setting.SettingKey,
            setting.SettingValue,
            setting.DataType,
            setting.Description,
            setting.UpdatedAt
        };
    }
}
