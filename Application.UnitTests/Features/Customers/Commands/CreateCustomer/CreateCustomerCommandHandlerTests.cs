using Application.Features.Customers.Commands.CreateCustomer;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_AddsCustomerAndReturnsDto()
    {
        // Arrange
        Customer? savedCustomer = null;

        var repo = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new CreateCustomerCommandHandler(repo, audit);
        var command = CreateValidCommand();

        repo.AddAsync(Arg.Do<Customer>(customer =>
        {
            customer.CustomerId = 123;
            savedCustomer = customer;
        }), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

        // Act
        var before = DateTime.UtcNow;
        var result = await handler.Handle(command, CancellationToken.None);
        var after = DateTime.UtcNow;

        // Assert
        savedCustomer.Should().NotBeNull();

        savedCustomer!.CustomerId.Should().Be(123);
        savedCustomer.AccountNumber.Should().Be(command.AccountNumber);
        savedCustomer.CompanyName.Should().Be(command.CompanyName);
        savedCustomer.IndustryType.Should().Be(command.IndustryType);
        savedCustomer.MainContactName.Should().Be(command.MainContactName);
        savedCustomer.MainContactEmail.Should().Be(command.MainContactEmail);
        savedCustomer.MainContactPhone.Should().Be(command.MainContactPhone);
        savedCustomer.BillingAddressId.Should().Be(command.BillingAddressId);
        savedCustomer.DefaultDeliveryAddressId.Should().Be(command.DefaultDeliveryAddressId);
        savedCustomer.PricingTierId.Should().Be(command.PricingTierId);
        savedCustomer.PaymentTermsDays.Should().Be(command.PaymentTermsDays);
        savedCustomer.CreditLimit.Should().Be(command.CreditLimit);
        savedCustomer.IsActive.Should().Be(command.IsActive);
        savedCustomer.CreatedAt.Should().BeOnOrAfter(before);
        savedCustomer.CreatedAt.Should().BeOnOrBefore(after);

        await repo.Received(1)
            .AddAsync(savedCustomer, Arg.Any<CancellationToken>());

        result.CustomerId.Should().Be(savedCustomer.CustomerId);
        result.AccountNumber.Should().Be(command.AccountNumber);
        result.CompanyName.Should().Be(command.CompanyName);
        result.IndustryType.Should().Be(command.IndustryType);
        result.MainContactName.Should().Be(command.MainContactName);
        result.MainContactEmail.Should().Be(command.MainContactEmail);
        result.MainContactPhone.Should().Be(command.MainContactPhone);
        result.BillingAddressId.Should().Be(command.BillingAddressId);
        result.DefaultDeliveryAddressId.Should().Be(command.DefaultDeliveryAddressId);
        result.PricingTierId.Should().Be(command.PricingTierId);
        result.PaymentTermsDays.Should().Be(command.PaymentTermsDays);
        result.CreditLimit.Should().Be(command.CreditLimit);
        result.IsActive.Should().Be(command.IsActive);

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Customer"),
            Arg.Is<int>(value => value == savedCustomer.CustomerId),
            Arg.Is<string>(value => value == "Created"),
            Arg.Is<object?>(value => value == null),
            Arg.Any<object>(),
            Arg.Is<string>(message => message.Contains(command.CompanyName)),
            Arg.Any<CancellationToken>());
    }

    private static CreateCustomerCommand CreateValidCommand()
    {
        return new CreateCustomerCommand
        {
            AccountNumber = "CUST-001",
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
            IsActive = true
        };
    }
}
