using Domain.Enums;

namespace Domain.Rules;

public static class OrderStatusTransitions
{
    private static readonly Dictionary<OrderStatusEnum, OrderStatusEnum[]> Allowed = new()
    {
        { OrderStatusEnum.Draft, new[] { OrderStatusEnum.Submitted, OrderStatusEnum.Cancelled } },
        { OrderStatusEnum.Submitted, new[] { OrderStatusEnum.Draft, OrderStatusEnum.PendingReview, OrderStatusEnum.Cancelled } },
        { OrderStatusEnum.PendingReview, new[] { OrderStatusEnum.Draft, OrderStatusEnum.Approved, OrderStatusEnum.Cancelled } },
        { OrderStatusEnum.Approved, Array.Empty<OrderStatusEnum>() },
        { OrderStatusEnum.InProcessing, Array.Empty<OrderStatusEnum>() },
        { OrderStatusEnum.AwaitingDispatch, new[] { OrderStatusEnum.Completed, OrderStatusEnum.Cancelled } },
        { OrderStatusEnum.Failed, new[] { OrderStatusEnum.Cancelled } }
    };

    public static bool IsTerminal(OrderStatusEnum status)
        => status is OrderStatusEnum.Completed
        or OrderStatusEnum.Cancelled;

    public static bool CanTransition(OrderStatusEnum from, OrderStatusEnum to)
    {
        if (IsTerminal(from))
            return false;

        return Allowed.TryGetValue(from, out var next)
               && next.Contains(to);
    }

    public static bool CanTransition(OrderStatusEnum from, OrderStatusEnum to, IEnumerable<string> roles)
    {
        return CanTransition(from, to) && HasRolePermission(from, to, roles);
    }

    public static IEnumerable<OrderStatusEnum> GetAllowed(OrderStatusEnum from)
    {
        if (IsTerminal(from))
            return Enumerable.Empty<OrderStatusEnum>();

        return Allowed.TryGetValue(from, out var next)
            ? next.Distinct()
            : Enumerable.Empty<OrderStatusEnum>();
    }

    public static IEnumerable<OrderStatusEnum> GetAllowed(OrderStatusEnum from, IEnumerable<string> roles)
    {
        return GetAllowed(from)
            .Where(to => HasRolePermission(from, to, roles));
    }

    private static bool HasRolePermission(OrderStatusEnum from, OrderStatusEnum to, IEnumerable<string> roles)
    {
        var roleSet = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (roleSet.Contains("Admin"))
            return true;

        if (roleSet.Contains("Sales"))
        {
            return (from, to) is
                (OrderStatusEnum.Draft, OrderStatusEnum.Submitted) or
                (OrderStatusEnum.Submitted, OrderStatusEnum.Draft) or
                (OrderStatusEnum.Draft, OrderStatusEnum.Cancelled) or
                (OrderStatusEnum.Submitted, OrderStatusEnum.Cancelled);
        }

        if (roleSet.Contains("Operations"))
        {
            return (from, to) is
                (OrderStatusEnum.Submitted, OrderStatusEnum.PendingReview) or
                (OrderStatusEnum.PendingReview, OrderStatusEnum.Draft) or
                (OrderStatusEnum.PendingReview, OrderStatusEnum.Approved) or
                (OrderStatusEnum.PendingReview, OrderStatusEnum.Cancelled) or
                (OrderStatusEnum.AwaitingDispatch, OrderStatusEnum.Completed) or
                (OrderStatusEnum.AwaitingDispatch, OrderStatusEnum.Cancelled) or
                (OrderStatusEnum.Failed, OrderStatusEnum.Cancelled);
        }

        return false;
    }
}
