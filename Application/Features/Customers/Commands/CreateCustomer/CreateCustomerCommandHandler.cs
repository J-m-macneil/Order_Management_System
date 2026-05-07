using Application.Features.Customers.DTOs;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler
    : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repo;

    public CreateCustomerCommandHandler(ICustomerRepository repo)
    {
        _repo = repo;
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
}