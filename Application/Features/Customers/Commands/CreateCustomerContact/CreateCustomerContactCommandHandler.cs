using Application.Features.Customers.DTOs;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomerContact;

public class CreateCustomerContactCommandHandler
    : IRequestHandler<CreateCustomerContactCommand, CustomerContactDto>
{
    private readonly ICustomerContactRepository _repo;
    private readonly ICustomerRepository _customers;
    private readonly IAuditService _audit;

    public CreateCustomerContactCommandHandler(
        ICustomerContactRepository repo,
        ICustomerRepository customers,
        IAuditService audit)
    {
        _repo = repo;
        _customers = customers;
        _audit = audit;
    }

    public async Task<CustomerContactDto> Handle(
        CreateCustomerContactCommand request,
        CancellationToken ct)
    {
        var contact = new CustomerContact
        {
            CustomerId = request.CustomerId,
            Name = request.Name,
            JobTitle = request.JobTitle,
            Email = request.Email,
            Phone = request.Phone,
            IsPrimary = request.IsPrimary,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        if (contact.IsPrimary)
        {
            await _repo.ClearPrimaryForCustomerAsync(contact.CustomerId, null, ct);
        }

        await _repo.AddAsync(contact, ct);

        if (contact.IsPrimary)
        {
            await SyncCustomerMainContactAsync(contact, ct);
        }

        await _audit.LogAsync(
            "CustomerContact",
            contact.CustomerContactId,
            "Created",
            null,
            CreateSnapshot(contact),
            $"Customer contact created for customer #{contact.CustomerId}: {contact.Name}.",
            ct);

        return new CustomerContactDto
        {
            CustomerContactId = contact.CustomerContactId,
            CustomerId = contact.CustomerId,
            Name = contact.Name,
            JobTitle = contact.JobTitle,
            Email = contact.Email,
            Phone = contact.Phone,
            IsPrimary = contact.IsPrimary
        };
    }

    private static object CreateSnapshot(CustomerContact contact)
    {
        return new
        {
            contact.CustomerContactId,
            contact.CustomerId,
            contact.Name,
            contact.JobTitle,
            contact.Email,
            contact.Phone,
            contact.IsPrimary,
            contact.IsActive,
            contact.DeletedAt
        };
    }

    private async Task SyncCustomerMainContactAsync(CustomerContact contact, CancellationToken ct)
    {
        var customer = await _customers.GetByIdAsync(contact.CustomerId, ct);

        if (customer == null)
        {
            return;
        }

        customer.MainContactName = contact.Name;
        customer.MainContactEmail = contact.Email;
        customer.MainContactPhone = contact.Phone ?? string.Empty;

        await _customers.UpdateAsync(customer, ct);
    }
}
