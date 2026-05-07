using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.DeleteCustomerContact
{
    public class DeleteCustomerContactCommandHandler : IRequestHandler<DeleteCustomerContactCommand>
    {
        private readonly ICustomerContactRepository _repo;

        public DeleteCustomerContactCommandHandler(ICustomerContactRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteCustomerContactCommand request, CancellationToken ct)
        {
            var contact = await _repo.GetByIdAsync(
                request.CustomerId,
                request.CustomerContactId,
                ct);

            if (contact == null)
                return;

            contact.IsActive = false;
            contact.DeletedAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync(ct);
        }
    }
}