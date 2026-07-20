using Application.Interfaces;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _repo;
    private readonly IAuditService _audit;

    public DeleteCustomerCommandHandler(
        ICustomerRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await _repo.GetByIdAsync(request.CustomerId, ct);

        if (customer == null)
            return;

        var oldValues = new
        {
            customer.CustomerId,
            customer.CompanyName,
            customer.IsActive,
            customer.DeletedAt
        };

        customer.IsActive = false;
        customer.DeletedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(customer, ct);

        await _audit.LogAsync(
            "Customer",
            customer.CustomerId,
            "Deleted",
            oldValues,
            new
            {
                customer.CustomerId,
                customer.CompanyName,
                customer.IsActive,
                customer.DeletedAt
            },
            $"Customer deleted: {customer.CompanyName}.",
            ct);
    }
}
