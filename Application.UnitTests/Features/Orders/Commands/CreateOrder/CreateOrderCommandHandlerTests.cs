using Application.Common.Exceptions;
using Application.Common.Interfaces;
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
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(4);
        var handler = new CreateOrderCommandHandler(repo, audit, currentUser);
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
        result.OrderId.Should().Be(123);
        savedOrder.Should().NotBeNull();
        var order = savedOrder!;
        order.CreatedByUserId.Should().Be(4);
        order.OrderStatusId.Should().Be(1);
        order.Currency.Should().Be("GBP");
        order.OrderItems.Should().HaveCount(2);
        order.Subtotal.Should().Be(200m);
        order.DiscountAmount.Should().Be(10m);
        order.TaxAmount.Should().Be(38m);
        order.TotalAmount.Should().Be(228m);

        await repo.Received(1).AddAsync(order, Arg.Any<CancellationToken>());
        await repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Order"),
            Arg.Is<int>(value => value == order.OrderId),
            Arg.Is<string>(value => value == "Created"),
            Arg.Is<object?>(value => value == null),
            Arg.Any<object>(),
            Arg.Is<string>(value => value.Contains(order.OrderNumber)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutAuthenticatedUser_ThrowsUnauthorizedException()
    {
        var repo = Substitute.For<IOrderRepository>();
        var audit = Substitute.For<IAuditService>();
        var currentUser = Substitute.For<ICurrentUserService>();
        var handler = new CreateOrderCommandHandler(repo, audit, currentUser);

        var act = () => handler.Handle(CreateValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await repo.DidNotReceive().AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
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
