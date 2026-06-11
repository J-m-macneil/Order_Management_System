using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHanlder : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly ICustomerRepository _repo;
    private readonly IAuditService _audit;
    private readonly IAuditChangeFormatter _changeFormatter;

    public UpdateCustomerCommandHanlder(
        ICustomerRepository repo,
        IAuditService audit,
        IAuditChangeFormatter changeFormatter)
    {
        _repo = repo;
        _audit = audit;
        _changeFormatter = changeFormatter;
    }

    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var customer = await _repo.GetByIdAsync(request.CustomerId, ct);

        if (customer == null)
            throw new Exception("Customer not found");

        var oldValues = CreateSnapshot(customer);

        customer.AccountNumber = request.AccountNumber;
        customer.CompanyName = request.CompanyName;
        customer.IndustryType = request.IndustryType;
        customer.BillingAddressId = request.BillingAddressId;
        customer.DefaultDeliveryAddressId = request.DefaultDeliveryAddressId;
        customer.PricingTierId = request.PricingTierId;
        customer.PaymentTermsDays = request.PaymentTermsDays;
        customer.CreditLimit = request.CreditLimit;
        customer.IsActive = request.IsActive;

        await _repo.UpdateAsync(customer, ct);

        var newValues = CreateSnapshot(customer);
        var changes = _changeFormatter.GetChanges(oldValues, newValues);

        await _audit.LogAsync(
            "Customer",
            customer.CustomerId,
            "Updated",
            oldValues,
            newValues,
            _changeFormatter.CreateUpdateNote("Customer", customer.CompanyName, changes),
            ct);

        return Unit.Value;
    }

    private static object CreateSnapshot(Customer customer)
    {
        return new
        {
            customer.AccountNumber,
            customer.CompanyName,
            customer.IndustryType,
            customer.MainContactName,
            customer.MainContactEmail,
            customer.MainContactPhone,
            customer.BillingAddressId,
            customer.DefaultDeliveryAddressId,
            customer.PricingTierId,
            customer.PaymentTermsDays,
            customer.CreditLimit,
            customer.IsActive
        };
    }
}
