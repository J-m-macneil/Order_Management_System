using Application.Features.Customers.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _repo;

    public GetCustomerByIdQueryHandler(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var x = await _repo.GetByIdAsync(request.CustomerId, ct);

        if (x == null)
            return null;

        return new CustomerDto
        {
            CustomerId = x.CustomerId,
            AccountNumber = x.AccountNumber,
            CompanyName = x.CompanyName,
            IndustryType = x.IndustryType,
            MainContactName = x.MainContactName,
            MainContactEmail = x.MainContactEmail,
            MainContactPhone = x.MainContactPhone,
            BillingAddressId = x.BillingAddressId,
            DefaultDeliveryAddressId = x.DefaultDeliveryAddressId,
            PricingTierId = x.PricingTierId,
            PaymentTermsDays = x.PaymentTermsDays,
            CreditLimit = x.CreditLimit,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        };
    }
}