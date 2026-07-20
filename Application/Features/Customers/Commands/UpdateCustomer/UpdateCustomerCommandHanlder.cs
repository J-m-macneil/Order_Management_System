using Application.Interfaces;
using Application.Common.Exceptions;
using Application.Common.Validation;
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
        ValidateRequest(request);

        var customer = await _repo.GetByIdAsync(request.CustomerId, ct);

        if (customer == null)
            throw new Exception("Customer not found");

        var accountNumber = request.AccountNumber.Trim();

        if (await _repo.AccountNumberExistsAsync(accountNumber, customer.CustomerId, ct))
        {
            throw new ConflictException("A customer with this account number already exists.");
        }

        var oldValues = CreateSnapshot(customer);

        customer.AccountNumber = accountNumber;
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

    private static void ValidateRequest(UpdateCustomerCommand request)
    {
        CommandValidation.PositiveId(request.CustomerId, "Customer");
        CommandValidation.RequiredText(request.AccountNumber, "Account number", 30);
        CommandValidation.RequiredText(request.CompanyName, "Company name", 160);
        CommandValidation.RequiredText(request.IndustryType, "Industry type", 80);
        CommandValidation.PositiveId(request.PricingTierId, "Pricing tier");

        if (request.PaymentTermsDays < 0)
        {
            throw new BadRequestException("Payment terms cannot be negative.");
        }

        CommandValidation.NonNegative(request.CreditLimit, "Credit limit");
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
