using Domain.Enums;

namespace Domain.Rules;

public static class OrderStatusTransitions
{
    private static readonly Dictionary<OrderStatusEnum, OrderStatusEnum[]> Allowed = new()
    {
        { OrderStatusEnum.Draft, new[] { OrderStatusEnum.Submitted } },
        { OrderStatusEnum.Submitted, new[] { OrderStatusEnum.PendingReview } },
        { OrderStatusEnum.PendingReview, new[] { OrderStatusEnum.Approved, OrderStatusEnum.Failed } },
        { OrderStatusEnum.Approved, Array.Empty<OrderStatusEnum>() },
        { OrderStatusEnum.InProcessing, Array.Empty<OrderStatusEnum>() },
        { OrderStatusEnum.AwaitingDispatch, new[] { OrderStatusEnum.Completed } }
    };

    public static bool IsTerminal(OrderStatusEnum status)
        => status is OrderStatusEnum.Completed
        or OrderStatusEnum.Failed
        or OrderStatusEnum.Cancelled;

    public static bool CanTransition(OrderStatusEnum from, OrderStatusEnum to)
    {
        if (IsTerminal(from))
            return false;

        if (to == OrderStatusEnum.Cancelled)
            return true;

        return Allowed.TryGetValue(from, out var next)
               && next.Contains(to);
    }

    public static IEnumerable<OrderStatusEnum> GetAllowed(OrderStatusEnum from)
    {
        if (IsTerminal(from))
            return Enumerable.Empty<OrderStatusEnum>();

        var allowed = Allowed.TryGetValue(from, out var next)
            ? next.ToList()
            : new List<OrderStatusEnum>();

        allowed.Add(OrderStatusEnum.Cancelled);

        return allowed.Distinct();
    }
}
