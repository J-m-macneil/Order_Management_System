using Application.Features.Customers.Commands.CreateCustomerContact;
using Application.Features.Customers.Commands.DeleteCustomerContact;
using Application.Features.Customers.Commands.UpdateCustomerContact;
using Application.Features.Customers.DTOs;
using Application.Features.Customers.Queries.GetCustomerContacts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/customers/{customerId}/contacts")]
[Authorize]
public class CustomerContactsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET ALL
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerContactDto>>> Get(int customerId)
    {
        var result = await _mediator.Send(new GetCustomerContactsQuery
        {
            CustomerId = customerId
        });

        return Ok(result);
    }

    // CREATE
    [HttpPost]
    public async Task<ActionResult<CustomerContactDto>> Create(
        int customerId,
        [FromBody] CreateCustomerContactDto dto)
    {
        var result = await _mediator.Send(new CreateCustomerContactCommand
        {
            CustomerId = customerId,
            Name = dto.Name,
            JobTitle = dto.JobTitle,
            Email = dto.Email,
            Phone = dto.Phone,
            IsPrimary = dto.IsPrimary
        });

        return Ok(result);
    }

    // UPDATE
    [HttpPut("{contactId}")]
    public async Task<IActionResult> Update(
        int customerId,
        int contactId,
        [FromBody] CreateCustomerContactDto dto)
    {
        await _mediator.Send(new UpdateCustomerContactCommand
        {
            CustomerId = customerId,
            Name = dto.Name,
            JobTitle = dto.JobTitle,
            Email = dto.Email,
            Phone = dto.Phone,
            IsPrimary = dto.IsPrimary
        });

        return NoContent();
    }

    // DELETE
    [HttpDelete("{contactId}")]
    public async Task<IActionResult> Delete(int customerId, int contactId)
    {
        await _mediator.Send(new DeleteCustomerContactCommand
        {
            CustomerId = customerId,
            CustomerContactId = contactId
        });

        return NoContent();
    }
}