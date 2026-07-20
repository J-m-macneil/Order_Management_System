using MediatR;
using Application.Features.Carriers.DTOs;

namespace Application.Features.Carriers.Queries.GetCarrierById;

public class GetCarrierByIdQuery : IRequest<CarrierDto?>
{
    public int CarrierId { get; set; }
}