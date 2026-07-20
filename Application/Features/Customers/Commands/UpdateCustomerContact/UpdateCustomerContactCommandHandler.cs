using Application.Interfaces;
using Application.Common.Validation;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.UpdateCustomerContact;

public class UpdateCustomerContactCommandHandler
    : IRequestHandler<UpdateCustomerContactCommand, Unit>
{
    private readonly ICustomerContactRepository _repo;
    private readonly ICustomerRepository _customers;
    private readonly IAuditService _audit;

    public UpdateCustomerContactCommandHandler(
        ICustomerContactRepository repo,
        ICustomerRepository customers,
        IAuditService audit)
    {
        _repo = repo;
        _customers = customers;
        _audit = audit;
    }

    public async Task<Unit> Handle(UpdateCustomerContactCommand request, CancellationToken ct)
    {
        ValidateRequest(request);

        var contact = await _repo.GetByIdAsync(
            request.CustomerId,
            request.CustomerContactId,
            ct);

        if (contact == null)
            throw new Exception("Customer contact not found");

        var oldValues = CreateSnapshot(contact);
        var wasPrimary = contact.IsPrimary;

        contact.Name = request.Name;
        contact.JobTitle = request.JobTitle;
        contact.Email = request.Email;
        contact.Phone = request.Phone;
        contact.IsPrimary = request.IsPrimary;

        if (contact.IsPrimary)
        {
            await _repo.ClearPrimaryForCustomerAsync(
                contact.CustomerId,
                contact.CustomerContactId,
                ct);
        }

        await _repo.SaveChangesAsync(ct);

        if (contact.IsPrimary)
        {
            await SyncCustomerMainContactAsync(contact, ct);
        }
        else if (wasPrimary)
        {
            await ClearCustomerMainContactAsync(contact.CustomerId, ct);
        }

        var newValues = CreateSnapshot(contact);

        await _audit.LogAsync(
            "CustomerContact",
            contact.CustomerContactId,
            "Updated",
            oldValues,
            newValues,
            $"Customer contact updated: {contact.Name} for customer #{contact.CustomerId}.",
            ct);

        return Unit.Value;
    }

    private static void ValidateRequest(UpdateCustomerContactCommand request)
    {
        CommandValidation.PositiveId(request.CustomerId, "Customer");
        CommandValidation.PositiveId(request.CustomerContactId, "Customer contact");
        CommandValidation.RequiredText(request.Name, "Contact name", 120);
        CommandValidation.OptionalText(request.JobTitle, "Job title", 120);
        CommandValidation.Email(request.Email);
        CommandValidation.OptionalPhone(request.Phone);
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

    private async Task ClearCustomerMainContactAsync(int customerId, CancellationToken ct)
    {
        var customer = await _customers.GetByIdAsync(customerId, ct);

        if (customer == null)
        {
            return;
        }

        customer.MainContactName = string.Empty;
        customer.MainContactEmail = string.Empty;
        customer.MainContactPhone = string.Empty;

        await _customers.UpdateAsync(customer, ct);
    }
}
