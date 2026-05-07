using Application.Features.Customers.DTOs;
using Application.Features.Customers.Queries.GetCustomerContacts;
using Domain.Repositories;
using MediatR;

public class GetCustomerContactsQueryHandler
    : IRequestHandler<GetCustomerContactsQuery, List<CustomerContactDto>>
{
    private readonly ICustomerContactRepository _repo;

    public GetCustomerContactsQueryHandler(ICustomerContactRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CustomerContactDto>> Handle(GetCustomerContactsQuery request, CancellationToken ct)
    {
        var contacts = await _repo.GetByCustomerAsync(request.CustomerId, ct);

        return contacts.Select(x => new CustomerContactDto
        {
            CustomerContactId = x.CustomerContactId,
            CustomerId = x.CustomerId,
            Name = x.Name,
            JobTitle = x.JobTitle,
            Email = x.Email,
            Phone = x.Phone,
            IsPrimary = x.IsPrimary
        }).ToList();
    }
}