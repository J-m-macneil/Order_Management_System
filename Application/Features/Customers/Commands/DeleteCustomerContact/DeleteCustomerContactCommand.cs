using MediatR;

namespace Application.Features.Customers.Commands.DeleteCustomerContact
{
    public class DeleteCustomerContactCommand : IRequest
    {
        public int CustomerId { get; set; }
        public int CustomerContactId { get; set; }
    }
}