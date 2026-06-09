using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.DeleteCustomerContact
{
    public class DeleteCustomerContactCommandHandler : IRequestHandler<DeleteCustomerContactCommand>
    {
        private readonly ICustomerContactRepository _repo;
        private readonly ICustomerRepository _customers;
        private readonly IAuditService _audit;

        public DeleteCustomerContactCommandHandler(
            ICustomerContactRepository repo,
            ICustomerRepository customers,
            IAuditService audit)
        {
            _repo = repo;
            _customers = customers;
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
            var wasPrimary = contact.IsPrimary;

            contact.IsActive = false;
            contact.IsPrimary = false;
            contact.DeletedAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync(ct);

            if (wasPrimary)
            {
                await ClearCustomerMainContactAsync(contact.CustomerId, ct);
            }

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

        private async Task ClearCustomerMainContactAsync(int customerId, CancellationToken ct)
        {
            var customer = await _customers.GetByIdAsync(customerId, ct);

            if (customer == null)
            {
                return;
            }

            customer.MainContactName = string.Empty;
            customer.MainContactEmail = string.Empty;
            customer.MainContactPhone = string.Empty;

            await _customers.UpdateAsync(customer, ct);
        }
    }
}
