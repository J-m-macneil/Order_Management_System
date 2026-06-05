using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHanlder : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly ICustomerRepository _repo;

    public UpdateCustomerCommandHanlder(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var customer = await _repo.GetByIdAsync(request.CustomerId, ct);

        if (customer == null)
            throw new Exception("Customer not found");

        customer.AccountNumber = request.AccountNumber;
        customer.CompanyName = request.CompanyName;
        customer.IndustryType = request.IndustryType;
        customer.MainContactName = request.MainContactName;
        customer.MainContactEmail = request.MainContactEmail;
        customer.MainContactPhone = request.MainContactPhone;
        customer.BillingAddressId = request.BillingAddressId;
        customer.DefaultDeliveryAddressId = request.DefaultDeliveryAddressId;
        customer.PricingTierId = request.PricingTierId;
        customer.PaymentTermsDays = request.PaymentTermsDays;
        customer.CreditLimit = request.CreditLimit;
        customer.IsActive = request.IsActive;

        await _repo.UpdateAsync(customer, ct);

        return Unit.Value;
    }
}
