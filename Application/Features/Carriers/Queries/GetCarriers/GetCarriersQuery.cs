using Application.Features.Carriers.DTOs;
using MediatR;

namespace Application.Features.Carriers.Queries.GetCarriers;

public class GetCarriersQuery : IRequest<List<CarrierDto>>
{
}