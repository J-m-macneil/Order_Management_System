using Application.Common.Models;
using Application.Features.Customers.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ICustomerRepository _repo;

    public GetCustomersQueryHandler(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken ct)
    {
        var totalCount = await _repo.CountActiveAsync(
            request.SearchTerm,
            request.IndustryType,
            request.PaymentTermsDays,
            request.IsActive,
            ct);

        var customers = await _repo.GetPagedAsync(
            request.Skip,
            request.PageSize,
            request.SearchTerm,
            request.IndustryType,
            request.PaymentTermsDays,
            request.IsActive,
            ct);

        var items = customers.Select(x => new CustomerDto
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

        return new PagedResult<CustomerDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
