using Domain.Enums;
using Domain.Rules;

namespace Domain.UnitTests.Rules;

public class OrderStatusTransitionsTests
{
    [Fact]
    public void CanTransition_WhenDraftAndSalesSubmits_ReturnsTrue()
    {
        var result = OrderStatusTransitions.CanTransition(
            OrderStatusEnum.Draft,
            OrderStatusEnum.Submitted,
            new[] { "Sales" });

        Assert.True(result);
    }

    [Fact]
    public void CanTransition_WhenApprovedMovesManually_ReturnsFalse()
    {
        var result = OrderStatusTransitions.CanTransition(
            OrderStatusEnum.Approved,
            OrderStatusEnum.AwaitingDispatch,
            new[] { "Operations" });

        Assert.False(result);
    }

    [Fact]
    public void CanTransition_WhenPendingReviewMovesToFailedManually_ReturnsFalse()
    {
        var result = OrderStatusTransitions.CanTransition(
            OrderStatusEnum.PendingReview,
            OrderStatusEnum.Failed,
            new[] { "Operations" });

        Assert.False(result);
    }

    [Fact]
    public void GetAllowed_WhenFailedAndOperations_ReturnsCancelOnly()
    {
        var result = OrderStatusTransitions
            .GetAllowed(OrderStatusEnum.Failed, new[] { "Operations" })
            .ToList();

        Assert.Equal(new[] { OrderStatusEnum.Cancelled }, result);
    }

    [Fact]
    public void GetAllowed_WhenPendingReviewAndOperations_ReturnsReviewActions()
    {
        var result = OrderStatusTransitions
            .GetAllowed(OrderStatusEnum.PendingReview, new[] { "Operations" })
            .ToList();

        Assert.Equal(
            new[]
            {
                OrderStatusEnum.Draft,
                OrderStatusEnum.Approved,
                OrderStatusEnum.Cancelled
            },
            result);
    }
}
