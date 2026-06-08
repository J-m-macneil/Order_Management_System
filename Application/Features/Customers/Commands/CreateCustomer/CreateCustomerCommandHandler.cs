using Application.Features.Customers.DTOs;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler
    : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repo;
    private readonly IAuditService _audit;

    public CreateCustomerCommandHandler(
        ICustomerRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var customer = new Customer
        {
            AccountNumber = request.AccountNumber,
            CompanyName = request.CompanyName,
            IndustryType = request.IndustryType,
            MainContactName = request.MainContactName,
            MainContactEmail = request.MainContactEmail,
            MainContactPhone = request.MainContactPhone,
            BillingAddressId = request.BillingAddressId,
            DefaultDeliveryAddressId = request.DefaultDeliveryAddressId,
            PricingTierId = request.PricingTierId,
            PaymentTermsDays = request.PaymentTermsDays,
            CreditLimit = request.CreditLimit,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(customer, ct);

        await _audit.LogAsync(
            "Customer",
            customer.CustomerId,
            "Created",
            null,
            CreateSnapshot(customer),
            $"Customer created: {customer.CompanyName}.",
            ct);

        return new CustomerDto
        {
            CustomerId = customer.CustomerId,
            AccountNumber = customer.AccountNumber,
            CompanyName = customer.CompanyName,
            IndustryType = customer.IndustryType,
            MainContactName = customer.MainContactName,
            MainContactEmail = customer.MainContactEmail,
            MainContactPhone = customer.MainContactPhone,
            BillingAddressId = customer.BillingAddressId,
            DefaultDeliveryAddressId = customer.DefaultDeliveryAddressId,
            PricingTierId = customer.PricingTierId,
            PaymentTermsDays = customer.PaymentTermsDays,
            CreditLimit = customer.CreditLimit,
            IsActive = customer.IsActive
        };
    }

    private static object CreateSnapshot(Customer customer)
    {
        return new
        {
            customer.CustomerId,
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
