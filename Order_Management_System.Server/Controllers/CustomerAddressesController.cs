using Application.Features.Addresses.DTOs;
using Application.Features.Addresses.Queries.GetAddressById;
using Application.Features.Addresses.Queries.GetCustomerAddresses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/customers/{customerId}/addresses")]
[Authorize]
public class CustomerAddressesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerAddressesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll(int customerId)
    {
        var result = await _mediator.Send(new GetCustomerAddressesQuery
        {
            CustomerId = customerId
        });

        return Ok(result);
    }

    [HttpGet("{addressId}")]
    public async Task<ActionResult<AddressDto>> GetById(int customerId, int addressId)
    {
        var result = await _mediator.Send(new GetAddressByIdQuery
        {
            CustomerId = customerId,
            AddressId = addressId
        });

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}