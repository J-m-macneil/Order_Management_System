using Application.Features.Customers.DTOs;
using Application.Interfaces;
using Application.Common.Exceptions;
using Application.Common.Validation;
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
        ValidateRequest(request);

        var accountNumber = request.AccountNumber.Trim();

        if (await _repo.AccountNumberExistsAsync(accountNumber, null, ct))
        {
            throw new ConflictException("A customer with this account number already exists.");
        }

        var customer = new Customer
        {
            AccountNumber = accountNumber,
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

    private static void ValidateRequest(CreateCustomerCommand request)
    {
        CommandValidation.RequiredText(request.AccountNumber, "Account number", 30);
        CommandValidation.RequiredText(request.CompanyName, "Company name", 160);
        CommandValidation.RequiredText(request.IndustryType, "Industry type", 80);
        CommandValidation.RequiredText(request.MainContactName, "Main contact name", 120);
        CommandValidation.Email(request.MainContactEmail, "Main contact email");
        CommandValidation.OptionalPhone(request.MainContactPhone, "Main contact phone");
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
