using Application.Features.Customers.Commands.UpdateCustomerContact;
using Application.Common.Services;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Application.UnitTests.Features.Customers.Commands.UpdateCustomerContact;

public class UpdateCustomerContactCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingContact_UpdatesContactAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<ICustomerContactRepository>();
        var customers = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateCustomerContactCommandHandler(repo, customers, audit, new AuditChangeFormatter());
        var contact = CreateExistingContact();
        var customer = CreateExistingCustomer();
        var command = CreateValidCommand();

        repo.GetByIdAsync(
                command.CustomerId,
                command.CustomerContactId,
                Arg.Any<CancellationToken>())
            .Returns(contact);

        customers.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        contact.CustomerContactId.Should().Be(command.CustomerContactId);
        contact.CustomerId.Should().Be(command.CustomerId);
        contact.Name.Should().Be(command.Name);
        contact.JobTitle.Should().Be(command.JobTitle);
        contact.Email.Should().Be(command.Email);
        contact.Phone.Should().Be(command.Phone);
        contact.IsPrimary.Should().Be(command.IsPrimary);

        await repo.Received(1).GetByIdAsync(
            command.CustomerId,
            command.CustomerContactId,
            Arg.Any<CancellationToken>());

        await repo.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await repo.Received(1)
            .ClearPrimaryForCustomerAsync(
                command.CustomerId,
                command.CustomerContactId,
                Arg.Any<CancellationToken>());

        customer.MainContactName.Should().Be(command.Name);
        customer.MainContactEmail.Should().Be(command.Email);
        customer.MainContactPhone.Should().Be(command.Phone);

        await customers.Received(1)
            .UpdateAsync(customer, Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "CustomerContact"),
            Arg.Is<int>(value => value == contact.CustomerContactId),
            Arg.Is<string>(value => value == "Updated"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message =>
                message.Contains(command.CustomerId.ToString()) &&
                message.Contains(command.Name)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPrimaryContactIsChangedToNonPrimary_ClearsCustomerSummary()
    {
        // Arrange
        var repo = Substitute.For<ICustomerContactRepository>();
        var customers = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateCustomerContactCommandHandler(repo, customers, audit, new AuditChangeFormatter());
        var contact = CreateExistingContact();
        contact.IsPrimary = true;

        var customer = CreateExistingCustomer();
        var command = CreateValidCommand();
        command.IsPrimary = false;

        repo.GetByIdAsync(
                command.CustomerId,
                command.CustomerContactId,
                Arg.Any<CancellationToken>())
            .Returns(contact);

        customers.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        contact.Name.Should().Be(command.Name);
        contact.Email.Should().Be(command.Email);
        contact.Phone.Should().Be(command.Phone);
        contact.IsPrimary.Should().BeFalse();

        customer.MainContactName.Should().BeEmpty();
        customer.MainContactEmail.Should().BeEmpty();
        customer.MainContactPhone.Should().BeEmpty();

        await repo.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await repo.DidNotReceive()
            .ClearPrimaryForCustomerAsync(
                Arg.Any<int>(),
                Arg.Any<int?>(),
                Arg.Any<CancellationToken>());

        await customers.Received(1)
            .UpdateAsync(customer, Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "CustomerContact"),
            Arg.Is<int>(value => value == contact.CustomerContactId),
            Arg.Is<string>(value => value == "Updated"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message =>
                message.Contains(command.CustomerId.ToString()) &&
                message.Contains(command.Name)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonPrimaryContact_DoesNotClearOtherPrimaryContactsOrUpdateCustomerSummary()
    {
        // Arrange
        var repo = Substitute.For<ICustomerContactRepository>();
        var customers = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateCustomerContactCommandHandler(repo, customers, audit, new AuditChangeFormatter());
        var contact = CreateExistingContact();
        var command = CreateValidCommand();
        command.IsPrimary = false;

        repo.GetByIdAsync(
                command.CustomerId,
                command.CustomerContactId,
                Arg.Any<CancellationToken>())
            .Returns(contact);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        contact.Name.Should().Be(command.Name);
        contact.Email.Should().Be(command.Email);
        contact.Phone.Should().Be(command.Phone);
        contact.IsPrimary.Should().BeFalse();

        await repo.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await repo.DidNotReceive()
            .ClearPrimaryForCustomerAsync(
                Arg.Any<int>(),
                Arg.Any<int?>(),
                Arg.Any<CancellationToken>());

        await customers.DidNotReceive()
            .GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());

        await customers.DidNotReceive()
            .UpdateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "CustomerContact"),
            Arg.Is<int>(value => value == contact.CustomerContactId),
            Arg.Is<string>(value => value == "Updated"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message =>
                message.Contains(command.CustomerId.ToString()) &&
                message.Contains(command.Name)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenContactDoesNotExist_ThrowsExceptionAndDoesNotSaveChanges()
    {
        // Arrange
        var repo = Substitute.For<ICustomerContactRepository>();
        var customers = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateCustomerContactCommandHandler(repo, customers, audit, new AuditChangeFormatter());
        var command = CreateValidCommand();

        repo.GetByIdAsync(
                command.CustomerId,
                command.CustomerContactId,
                Arg.Any<CancellationToken>())
            .Returns((CustomerContact?)null);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Customer contact not found");

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
            Name = "Old Contact",
            JobTitle = "Old Job Title",
            Email = "old@acme.test",
            Phone = "00000000000",
            IsPrimary = false,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
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
            MainContactName = "Old Primary",
            MainContactEmail = "primary@old.test",
            MainContactPhone = "01111111111",
            PricingTierId = 1,
            PaymentTermsDays = 30,
            CreditLimit = 5000m,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }

    private static UpdateCustomerContactCommand CreateValidCommand()
    {
        return new UpdateCustomerContactCommand
        {
            CustomerId = 123,
            CustomerContactId = 456,
            Name = "Jane Smith",
            JobTitle = "Purchasing Manager",
            Email = "jane@acme.test",
            Phone = "01234567890",
            IsPrimary = true
        };
    }
}
