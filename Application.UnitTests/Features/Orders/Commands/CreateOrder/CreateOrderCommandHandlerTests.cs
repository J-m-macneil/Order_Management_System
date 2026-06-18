using Application.Features.Orders.Commands.CreateOrder;
using Application.Interfaces;
using Domain.Entities.Orders;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_AddsDraftOrderCalculatesTotalsAndWritesAuditLog()
    {
        // Arrange
        Order? savedOrder = null;
        var repo = Substitute.For<IOrderRepository>();
        var audit = Substitute.For<IAuditService>();
        var handler = new CreateOrderCommandHandler(repo, audit);
        var command = CreateValidCommand();

        repo.AddAsync(Arg.Do<Order>(order =>
            {
                order.OrderId = 123;
                savedOrder = order;
            }), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(123);
        savedOrder.Should().NotBeNull();
        savedOrder!.OrderStatusId.Should().Be(1);
        savedOrder.Currency.Should().Be("GBP");
        savedOrder.OrderItems.Should().HaveCount(2);
        savedOrder.Subtotal.Should().Be(200m);
        savedOrder.DiscountAmount.Should().Be(10m);
        savedOrder.TaxAmount.Should().Be(38m);
        savedOrder.TotalAmount.Should().Be(228m);

        await repo.Received(1).AddAsync(savedOrder, Arg.Any<CancellationToken>());
        await repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Order"),
            Arg.Is<int>(value => value == savedOrder.OrderId),
            Arg.Is<string>(value => value == "Created"),
            Arg.Is<object?>(value => value == null),
            Arg.Any<object>(),
            Arg.Is<string>(value => value.Contains(savedOrder.OrderNumber)),
            Arg.Any<CancellationToken>());
    }

    private static CreateOrderCommand CreateValidCommand()
    {
        return new CreateOrderCommand
        {
            CustomerId = 1,
            DeliveryAddressId = 10,
            BillingAddressId = 11,
            WarehouseId = 2,
            CarrierId = 3,
            CreatedByUserId = 4,
            RequestedDeliveryDate = new DateTime(2026, 7, 1),
            PurchaseOrderReference = "PO-001",
            SpecialInstructions = "Handle with care",
            IsPriorityOrder = true,
            Items =
            {
                new CreateOrderItemCommand
                {
                    ProductId = 100,
                    Quantity = 2,
                    UnitPrice = 50m,
                    DiscountPercent = 10m
                },
                new CreateOrderItemCommand
                {
                    ProductId = 101,
                    Quantity = 1,
                    UnitPrice = 100m,
                    DiscountPercent = 0m
                }
            }
        };
    }
}
