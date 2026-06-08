using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.DeleteCustomerContact
{
    public class DeleteCustomerContactCommandHandler : IRequestHandler<DeleteCustomerContactCommand>
    {
        private readonly ICustomerContactRepository _repo;
        private readonly IAuditService _audit;

        public DeleteCustomerContactCommandHandler(
            ICustomerContactRepository repo,
            IAuditService audit)
        {
            _repo = repo;
            _audit = audit;
        }

        public async Task Handle(DeleteCustomerContactCommand request, CancellationToken ct)
        {
            var contact = await _repo.GetByIdAsync(
                request.CustomerId,
                request.CustomerContactId,
                ct);

            if (contact == null)
                return;

            var oldValues = CreateSnapshot(contact);

            contact.IsActive = false;
            contact.DeletedAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync(ct);

            await _audit.LogAsync(
                "CustomerContact",
                contact.CustomerContactId,
                "Deleted",
                oldValues,
                CreateSnapshot(contact),
                $"Customer contact deleted for customer #{contact.CustomerId}: {contact.Name}.",
                ct);
        }

        private static object CreateSnapshot(CustomerContact contact)
        {
            return new
            {
                contact.CustomerContactId,
                contact.CustomerId,
                contact.Name,
                contact.JobTitle,
                contact.Email,
                contact.Phone,
                contact.IsPrimary,
                contact.IsActive,
                contact.DeletedAt
            };
        }
    }
}
