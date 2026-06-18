using Application.Common.Interfaces;
using Application.Features.Orders.Commands.ChangeOrderStatus;
using Application.Interfaces;
using Domain.Entities.Orders;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Orders.Commands.ChangeOrderStatus;

public class ChangeOrderStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenSalesSubmitsDraft_ChangesStatusQueuesJobsAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var currentUser = Substitute.For<ICurrentUserService>();
        var jobQueue = Substitute.For<IProcessingJobQueueService>();
        var audit = Substitute.For<IAuditService>();
        var handler = new ChangeOrderStatusCommandHandler(repo, currentUser, jobQueue, audit);
        var order = CreateOrder(OrderStatusEnum.Draft);

        currentUser.UserId.Returns(7);
        currentUser.Roles.Returns(new List<string> { "Sales" });
        repo.GetByIdAsync(order.OrderId, Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        var result = await handler.Handle(
            new ChangeOrderStatusCommand
            {
                OrderId = order.OrderId,
                StatusId = (int)OrderStatusEnum.Submitted
            },
            CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        order.OrderStatusId.Should().Be((int)OrderStatusEnum.Submitted);
        order.OrderStatusHistory.Should().ContainSingle();
        order.OrderStatusHistory.Single().ChangedByUserId.Should().Be(7);

        await repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await jobQueue.Received(1).QueueSubmissionJobsAsync(order.OrderId);
        await jobQueue.DidNotReceive().QueueApprovalJobsAsync(Arg.Any<int>());
        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Order"),
            Arg.Is<int>(value => value == order.OrderId),
            Arg.Is<string>(value => value == "StatusChanged:Submitted"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(value => value.Contains("Draft") && value.Contains("Submitted")),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenReturningSubmittedOrderToDraftWithoutReason_ThrowsAndDoesNotSave()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var currentUser = Substitute.For<ICurrentUserService>();
        var jobQueue = Substitute.For<IProcessingJobQueueService>();
        var audit = Substitute.For<IAuditService>();
        var handler = new ChangeOrderStatusCommandHandler(repo, currentUser, jobQueue, audit);
        var order = CreateOrder(OrderStatusEnum.Submitted);

        currentUser.UserId.Returns(7);
        currentUser.Roles.Returns(new List<string> { "Sales" });
        repo.GetByIdAsync(order.OrderId, Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        var act = () => handler.Handle(
            new ChangeOrderStatusCommand
            {
                OrderId = order.OrderId,
                StatusId = (int)OrderStatusEnum.Draft
            },
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("A reason is required to move an order from Submitted to Draft.");

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
    public async Task Handle_WhenOperationsApprovesPendingReview_QueuesApprovalJobs()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var currentUser = Substitute.For<ICurrentUserService>();
        var jobQueue = Substitute.For<IProcessingJobQueueService>();
        var audit = Substitute.For<IAuditService>();
        var handler = new ChangeOrderStatusCommandHandler(repo, currentUser, jobQueue, audit);
        var order = CreateOrder(OrderStatusEnum.PendingReview);

        currentUser.UserId.Returns(8);
        currentUser.Roles.Returns(new List<string> { "Operations" });
        repo.GetByIdAsync(order.OrderId, Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        await handler.Handle(
            new ChangeOrderStatusCommand
            {
                OrderId = order.OrderId,
                StatusId = (int)OrderStatusEnum.Approved
            },
            CancellationToken.None);

        // Assert
        order.OrderStatusId.Should().Be((int)OrderStatusEnum.Approved);
        await jobQueue.Received(1).QueueApprovalJobsAsync(order.OrderId);
        await audit.Received(1).LogAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Is<string>(value => value == "StatusChanged:Approved"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOperationsManuallyMovesPendingReviewToFailed_Throws()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var currentUser = Substitute.For<ICurrentUserService>();
        var jobQueue = Substitute.For<IProcessingJobQueueService>();
        var audit = Substitute.For<IAuditService>();
        var handler = new ChangeOrderStatusCommandHandler(repo, currentUser, jobQueue, audit);
        var order = CreateOrder(OrderStatusEnum.PendingReview);

        currentUser.UserId.Returns(8);
        currentUser.Roles.Returns(new List<string> { "Operations" });
        repo.GetByIdAsync(order.OrderId, Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        var act = () => handler.Handle(
            new ChangeOrderStatusCommand
            {
                OrderId = order.OrderId,
                StatusId = (int)OrderStatusEnum.Failed,
                Reason = "Review failed"
            },
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid transition from PendingReview to Failed");

        await repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static Order CreateOrder(OrderStatusEnum status)
    {
        return new Order
        {
            OrderId = 123,
            OrderNumber = "ORD-TEST-001",
            OrderStatusId = (int)status
        };
    }
}
