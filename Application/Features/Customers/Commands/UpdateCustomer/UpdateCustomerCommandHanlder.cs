using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHanlder : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly ICustomerRepository _repo;
    private readonly IAuditService _audit;

    public UpdateCustomerCommandHanlder(
        ICustomerRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
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

        await _audit.LogAsync(
            "Customer",
            customer.CustomerId,
            "Updated",
            oldValues,
            newValues,
            $"Customer updated: {customer.CompanyName}.",
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
