using MediatR;

namespace Application.Features.SystemSettings.Commands.UpdateSystemSetting;

public class UpdateSystemSettingCommand : IRequest<Unit>
{
    public string SettingValue { get; set; } = string.Empty;
}
