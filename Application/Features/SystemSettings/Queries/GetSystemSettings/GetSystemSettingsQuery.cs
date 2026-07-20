using Application.Features.SystemSettings.DTOs;
using MediatR;

namespace Application.Features.SystemSettings.Queries.GetSystemSettings;

public class GetSystemSettingsQuery : IRequest<List<SystemSettingDto>>
{
}
