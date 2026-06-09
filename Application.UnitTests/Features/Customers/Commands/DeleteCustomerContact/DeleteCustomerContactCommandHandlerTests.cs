using Application.Features.Customers.Commands.DeleteCustomerContact;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Customers.Commands.DeleteCustomerContact;

public class DeleteCustomerContactCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingContact_SoftDeletesContactAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<ICustomerContactRepository>();
        var customers = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteCustomerContactCommandHandler(repo, customers, audit);
        var contact = CreateExistingContact();
        var customer = CreateExistingCustomer();
        var command = new DeleteCustomerContactCommand
        {
            CustomerId = contact.CustomerId,
            CustomerContactId = contact.CustomerContactId
        };

        repo.GetByIdAsync(
                command.CustomerId,
                command.CustomerContactId,
                Arg.Any<CancellationToken>())
            .Returns(contact);

        customers.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        // Act
        var before = DateTime.UtcNow;
        await handler.Handle(command, CancellationToken.None);
        var after = DateTime.UtcNow;

        // Assert
        contact.IsActive.Should().BeFalse();
        contact.IsPrimary.Should().BeFalse();
        contact.DeletedAt.Should().NotBeNull();
        contact.DeletedAt!.Value.Should().BeOnOrAfter(before);
        contact.DeletedAt.Value.Should().BeOnOrBefore(after);

        customer.MainContactName.Should().BeEmpty();
        customer.MainContactEmail.Should().BeEmpty();
        customer.MainContactPhone.Should().BeEmpty();

        await repo.Received(1).GetByIdAsync(
            command.CustomerId,
            command.CustomerContactId,
            Arg.Any<CancellationToken>());

        await repo.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await customers.Received(1)
            .UpdateAsync(customer, Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "CustomerContact"),
            Arg.Is<int>(value => value == contact.CustomerContactId),
            Arg.Is<string>(value => value == "Deleted"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message =>
                message.Contains(contact.CustomerId.ToString()) &&
                message.Contains(contact.Name)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonPrimaryContact_DoesNotClearCustomerSummary()
    {
        // Arrange
        var repo = Substitute.For<ICustomerContactRepository>();
        var customers = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteCustomerContactCommandHandler(repo, customers, audit);
        var contact = CreateExistingContact();
        contact.IsPrimary = false;

        var command = new DeleteCustomerContactCommand
        {
            CustomerId = contact.CustomerId,
            CustomerContactId = contact.CustomerContactId
        };

        repo.GetByIdAsync(
                command.CustomerId,
                command.CustomerContactId,
                Arg.Any<CancellationToken>())
            .Returns(contact);

        // Act
        var before = DateTime.UtcNow;
        await handler.Handle(command, CancellationToken.None);
        var after = DateTime.UtcNow;

        // Assert
        contact.IsActive.Should().BeFalse();
        contact.IsPrimary.Should().BeFalse();
        contact.DeletedAt.Should().NotBeNull();
        contact.DeletedAt!.Value.Should().BeOnOrAfter(before);
        contact.DeletedAt.Value.Should().BeOnOrBefore(after);

        await repo.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await customers.DidNotReceive()
            .GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());

        await customers.DidNotReceive()
            .UpdateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "CustomerContact"),
            Arg.Is<int>(value => value == contact.CustomerContactId),
            Arg.Is<string>(value => value == "Deleted"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message =>
                message.Contains(contact.CustomerId.ToString()) &&
                message.Contains(contact.Name)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenContactDoesNotExist_DoesNotSaveChangesOrWriteAuditLog()
    {
        // Arrange
        var repo = Substitute.For<ICustomerContactRepository>();
        var customers = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteCustomerContactCommandHandler(repo, customers, audit);
        var command = new DeleteCustomerContactCommand
        {
            CustomerId = 123,
            CustomerContactId = 456
        };

        repo.GetByIdAsync(
                command.CustomerId,
                command.CustomerContactId,
                Arg.Any<CancellationToken>())
            .Returns((CustomerContact?)null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await repo.Received(1).GetByIdAsync(
            command.CustomerId,
            command.CustomerContactId,
            Arg.Any<CancellationToken>());

        await repo.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await customers.DidNotReceive()
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

    private static CustomerContact CreateExistingContact()
    {
        return new CustomerContact
        {
            CustomerContactId = 456,
            CustomerId = 123,
            Name = "Jane Smith",
            JobTitle = "Purchasing Manager",
            Email = "jane@acme.test",
            Phone = "01234567890",
            IsPrimary = true,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1),
            DeletedAt = null
        };
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
            PricingTierId = 1,
            PaymentTermsDays = 30,
            CreditLimit = 5000m,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }
}
