using Application.Features.Customers.Commands.DeleteCustomer;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCustomer_SoftDeletesCustomerAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteCustomerCommandHandler(repo, audit);
        var customer = CreateExistingCustomer();
        var command = new DeleteCustomerCommand { CustomerId = customer.CustomerId };

        repo.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        // Act
        var before = DateTime.UtcNow;
        await handler.Handle(command, CancellationToken.None);
        var after = DateTime.UtcNow;

        // Assert
        customer.IsActive.Should().BeFalse();
        customer.DeletedAt.Should().NotBeNull();
        customer.DeletedAt!.Value.Should().BeOnOrAfter(before);
        customer.DeletedAt.Value.Should().BeOnOrBefore(after);

        await repo.Received(1)
            .GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>());

        await repo.Received(1)
            .UpdateAsync(customer, Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Customer"),
            Arg.Is<int>(value => value == customer.CustomerId),
            Arg.Is<string>(value => value == "Deleted"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message => message.Contains(customer.CompanyName)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_DoesNotUpdateCustomerOrWriteAuditLog()
    {
        // Arrange
        var repo = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteCustomerCommandHandler(repo, audit);
        var command = new DeleteCustomerCommand { CustomerId = 123 };

        repo.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await repo.Received(1)
            .GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>());

        await repo.DidNotReceive()
            .UpdateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());

        await audit.DidNotReceive().LogAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<object?>(),
            Arg.Any<object?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    private static Customer CreateExistingCustomer()
    {
        return new Customer
        {
            CustomerId = 123,
            AccountNumber = "CUST-123",
            CompanyName = "Acme Chemicals",
            IndustryType = "Manufacturing",
            MainContactName = "Jane Smith",
            MainContactEmail = "jane@acme.test",
            MainContactPhone = "01234567890",
            BillingAddressId = 10,
            DefaultDeliveryAddressId = 11,
            PricingTierId = 1,
            PaymentTermsDays = 30,
            CreditLimit = 5000m,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1),
            DeletedAt = null
        };
    }
}
