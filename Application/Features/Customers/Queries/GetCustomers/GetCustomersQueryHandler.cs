using Application.Features.Customers.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _repo;

    public GetCustomersQueryHandler(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken ct)
    {
        var customers = await _repo.GetAllAsync(ct);

        return customers.Select(x => new CustomerDto
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
        }).ToList();
    }
}