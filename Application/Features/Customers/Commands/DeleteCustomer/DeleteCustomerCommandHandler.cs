using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _repo;

    public DeleteCustomerCommandHandler(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await _repo.GetByIdAsync(request.CustomerId, ct);

        if (customer == null)
            return;

        // soft delete (your system standard)
        customer.IsActive = false;
        customer.DeletedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(customer, ct);
    }
}