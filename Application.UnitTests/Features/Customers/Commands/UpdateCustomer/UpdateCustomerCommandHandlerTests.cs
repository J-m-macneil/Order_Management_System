using Application.Features.Customers.Commands.UpdateCustomer;
using Application.Common.Services;
using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Application.UnitTests.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCustomer_UpdatesCustomerAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateCustomerCommandHanlder(repo, audit, new AuditChangeFormatter());
        var existingCustomer = CreateExistingCustomer();
        var command = CreateValidCommand();

        repo.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(existingCustomer);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        existingCustomer.CustomerId.Should().Be(command.CustomerId);
        existingCustomer.AccountNumber.Should().Be(command.AccountNumber);
        existingCustomer.CompanyName.Should().Be(command.CompanyName);
        existingCustomer.IndustryType.Should().Be(command.IndustryType);
        existingCustomer.MainContactName.Should().Be("Old Contact");
        existingCustomer.MainContactEmail.Should().Be("old@example.test");
        existingCustomer.MainContactPhone.Should().Be("00000000000");
        existingCustomer.BillingAddressId.Should().Be(command.BillingAddressId);
        existingCustomer.DefaultDeliveryAddressId.Should().Be(command.DefaultDeliveryAddressId);
        existingCustomer.PricingTierId.Should().Be(command.PricingTierId);
        existingCustomer.PaymentTermsDays.Should().Be(command.PaymentTermsDays);
        existingCustomer.CreditLimit.Should().Be(command.CreditLimit);
        existingCustomer.IsActive.Should().Be(command.IsActive);

        await repo.Received(1)
            .GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>());

        await repo.Received(1)
            .UpdateAsync(existingCustomer, Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Customer"),
            Arg.Is<int>(value => value == existingCustomer.CustomerId),
            Arg.Is<string>(value => value == "Updated"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message => message.Contains(command.CompanyName)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_ThrowsExceptionAndDoesNotUpdateCustomer()
    {
        // Arrange
        var repo = Substitute.For<ICustomerRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateCustomerCommandHanlder(repo, audit, new AuditChangeFormatter());
        var command = CreateValidCommand();

        repo.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Customer not found");

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
            AccountNumber = "OLD-001",
            CompanyName = "Old Chemicals",
            IndustryType = "Distribution",
            MainContactName = "Old Contact",
            MainContactEmail = "old@example.test",
            MainContactPhone = "00000000000",
            BillingAddressId = 1,
            DefaultDeliveryAddressId = 2,
            PricingTierId = 1,
            PaymentTermsDays = 14,
            CreditLimit = 1000m,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }

    private static UpdateCustomerCommand CreateValidCommand()
    {
        return new UpdateCustomerCommand
        {
            CustomerId = 123,
            AccountNumber = "CUST-123",
            CompanyName = "Updated Chemicals",
            IndustryType = "Manufacturing",
            MainContactName = "Jane Smith",
            MainContactEmail = "jane@updated.test",
            MainContactPhone = "01234567890",
            BillingAddressId = 10,
            DefaultDeliveryAddressId = 11,
            PricingTierId = 2,
            PaymentTermsDays = 30,
            CreditLimit = 5000m,
            IsActive = true
        };
    }
}
