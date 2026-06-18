using MediatR;

namespace Application.Features.SystemSettings.Commands.UpdateSystemSetting;

public class UpdateSystemSettingRequest : IRequest<Unit>
{
    public int SystemSettingId { get; set; }
    public UpdateSystemSettingCommand Data { get; set; } = new();
}
