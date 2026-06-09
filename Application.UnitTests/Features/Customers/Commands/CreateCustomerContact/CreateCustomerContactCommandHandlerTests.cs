using Application.Features.Customers.Commands.CreateCustomerContact;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Customers.Commands.CreateCustomerContact;

public class CreateCustomerContactCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_AddsContactAndReturnsDto()
    {
        // Arrange
        CustomerContact? savedContact = null;
        var customer = CreateExistingCustomer();

        var repo = Substitute.For<ICustomerContactRepository>();
        var customers = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new CreateCustomerContactCommandHandler(repo, customers, audit);
        var command = CreateValidCommand();

        customers.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        repo.AddAsync(Arg.Do<CustomerContact>(contact =>
        {
            contact.CustomerContactId = 456;
            savedContact = contact;
        }), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

        // Act
        var before = DateTime.UtcNow;
        var result = await handler.Handle(command, CancellationToken.None);
        var after = DateTime.UtcNow;

        // Assert
        savedContact.Should().NotBeNull();

        savedContact!.CustomerContactId.Should().Be(456);
        savedContact.CustomerId.Should().Be(command.CustomerId);
        savedContact.Name.Should().Be(command.Name);
        savedContact.JobTitle.Should().Be(command.JobTitle);
        savedContact.Email.Should().Be(command.Email);
        savedContact.Phone.Should().Be(command.Phone);
        savedContact.IsPrimary.Should().Be(command.IsPrimary);
        savedContact.IsActive.Should().BeTrue();
        savedContact.CreatedAt.Should().BeOnOrAfter(before);
        savedContact.CreatedAt.Should().BeOnOrBefore(after);

        await repo.Received(1)
            .AddAsync(savedContact, Arg.Any<CancellationToken>());

        await repo.Received(1)
            .ClearPrimaryForCustomerAsync(
                command.CustomerId,
                null,
                Arg.Any<CancellationToken>());

        customer.MainContactName.Should().Be(command.Name);
        customer.MainContactEmail.Should().Be(command.Email);
        customer.MainContactPhone.Should().Be(command.Phone);

        await customers.Received(1)
            .UpdateAsync(customer, Arg.Any<CancellationToken>());

        result.CustomerContactId.Should().Be(savedContact.CustomerContactId);
        result.CustomerId.Should().Be(command.CustomerId);
        result.Name.Should().Be(command.Name);
        result.JobTitle.Should().Be(command.JobTitle);
        result.Email.Should().Be(command.Email);
        result.Phone.Should().Be(command.Phone);
        result.IsPrimary.Should().Be(command.IsPrimary);

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "CustomerContact"),
            Arg.Is<int>(value => value == savedContact.CustomerContactId),
            Arg.Is<string>(value => value == "Created"),
            Arg.Is<object?>(value => value == null),
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
        CustomerContact? savedContact = null;

        var repo = Substitute.For<ICustomerContactRepository>();
        var customers = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new CreateCustomerContactCommandHandler(repo, customers, audit);
        var command = CreateValidCommand();
        command.IsPrimary = false;

        repo.AddAsync(Arg.Do<CustomerContact>(contact =>
        {
            contact.CustomerContactId = 789;
            savedContact = contact;
        }), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        savedContact.Should().NotBeNull();
        savedContact!.IsPrimary.Should().BeFalse();

        await repo.Received(1)
            .AddAsync(savedContact, Arg.Any<CancellationToken>());

        await repo.DidNotReceive()
            .ClearPrimaryForCustomerAsync(
                Arg.Any<int>(),
                Arg.Any<int?>(),
                Arg.Any<CancellationToken>());

        await customers.DidNotReceive()
            .GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());

        await customers.DidNotReceive()
            .UpdateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());

        result.IsPrimary.Should().BeFalse();

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "CustomerContact"),
            Arg.Is<int>(value => value == savedContact.CustomerContactId),
            Arg.Is<string>(value => value == "Created"),
            Arg.Is<object?>(value => value == null),
            Arg.Any<object>(),
            Arg.Is<string>(message =>
                message.Contains(command.CustomerId.ToString()) &&
                message.Contains(command.Name)),
            Arg.Any<CancellationToken>());
    }

    private static CreateCustomerContactCommand CreateValidCommand()
    {
        return new CreateCustomerContactCommand
        {
            CustomerId = 123,
            Name = "Jane Smith",
            JobTitle = "Purchasing Manager",
            Email = "jane@acme.test",
            Phone = "01234567890",
            IsPrimary = true
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
            MainContactName = string.Empty,
            MainContactEmail = string.Empty,
            MainContactPhone = string.Empty,
            PricingTierId = 1,
            PaymentTermsDays = 30,
            CreditLimit = 5000m,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }
}
