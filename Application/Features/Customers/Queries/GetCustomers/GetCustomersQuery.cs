using Application.Common.Models;
using Application.Features.Customers.DTOs;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQuery : PaginationQuery, IRequest<PagedResult<CustomerDto>>
{
    public string? SearchTerm { get; set; }
    public string? IndustryType { get; set; }
    public int? PaymentTermsDays { get; set; }
    public bool? IsActive { get; set; }
}
