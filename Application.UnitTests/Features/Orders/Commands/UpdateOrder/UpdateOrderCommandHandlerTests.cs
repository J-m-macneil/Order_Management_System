using Application.Common.Interfaces;
using Application.Features.Orders.Commands.UpdateOrder;
using Application.Interfaces;
using Domain.Entities.Orders;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithDraftOrderAndSalesUser_UpdatesOrderLinesTotalsAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var audit = Substitute.For<IAuditService>();
        var currentUser = Substitute.For<ICurrentUserService>();
        var handler = new UpdateOrderCommandHandler(repo, audit, currentUser);
        var order = CreateDraftOrder();
        var command = CreateValidCommand(order.OrderId);

        currentUser.Roles.Returns(new List<string> { "Sales" });
        repo.GetByIdAsync(order.OrderId, Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        order.CustomerId.Should().Be(command.CustomerId);
        order.OrderItems.Should().HaveCount(1);
        order.OrderItems.Single().ProductId.Should().Be(55);
        order.Subtotal.Should().Be(200m);
        order.DiscountAmount.Should().Be(20m);
        order.TaxAmount.Should().Be(36m);
        order.TotalAmount.Should().Be(216m);

        repo.Received(1).RemoveItems(Arg.Is<IEnumerable<OrderItem>>(items => items.Count() == 1));
        await repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Order"),
            Arg.Is<int>(value => value == order.OrderId),
            Arg.Is<string>(value => value == "Updated"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(value => value.Contains(order.OrderNumber)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOrderIsNotDraft_ThrowsAndDoesNotSave()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var audit = Substitute.For<IAuditService>();
        var currentUser = Substitute.For<ICurrentUserService>();
        var handler = new UpdateOrderCommandHandler(repo, audit, currentUser);
        var order = CreateDraftOrder();
        order.OrderStatusId = 4;

        currentUser.Roles.Returns(new List<string> { "Sales" });
        repo.GetByIdAsync(order.OrderId, Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        var act = () => handler.Handle(CreateValidCommand(order.OrderId), CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Only draft orders can be edited.");

        await repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await audit.DidNotReceive().LogAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<object?>(),
            Arg.Any<object?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserCannotEditDraft_ThrowsAndDoesNotSave()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var audit = Substitute.For<IAuditService>();
        var currentUser = Substitute.For<ICurrentUserService>();
        var handler = new UpdateOrderCommandHandler(repo, audit, currentUser);
        var order = CreateDraftOrder();

        currentUser.Roles.Returns(new List<string> { "Operations" });
        repo.GetByIdAsync(order.OrderId, Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        var act = () => handler.Handle(CreateValidCommand(order.OrderId), CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Only Sales or Admin users can edit draft orders.");

        await repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static Order CreateDraftOrder()
    {
        return new Order
        {
            OrderId = 123,
            OrderNumber = "ORD-TEST-001",
            OrderStatusId = 1,
            CustomerId = 1,
            DeliveryAddressId = 10,
            BillingAddressId = 11,
            WarehouseId = 1,
            RequestedDeliveryDate = new DateTime(2026, 7, 1),
            OrderItems =
            {
                new OrderItem
                {
                    OrderItemId = 1,
                    ProductId = 10,
                    Quantity = 1,
                    UnitPrice = 50m,
                    DiscountPercent = 0m,
                    LineTotal = 50m
                }
            }
        };
    }

    private static UpdateOrderCommand CreateValidCommand(int orderId)
    {
        return new UpdateOrderCommand
        {
            OrderId = orderId,
            CustomerId = 2,
            DeliveryAddressId = 20,
            BillingAddressId = 21,
            WarehouseId = 3,
            RequestedDeliveryDate = new DateTime(2026, 8, 1),
            IsPriorityOrder = true,
            Items =
            {
                new Application.Features.Orders.Commands.CreateOrder.CreateOrderItemCommand
                {
                    ProductId = 55,
                    Quantity = 2,
                    UnitPrice = 100m,
                    DiscountPercent = 10m
                }
            }
        };
    }
}
