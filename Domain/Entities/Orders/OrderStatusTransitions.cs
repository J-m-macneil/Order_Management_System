using Domain.Entities.Orders;
using Domain.Enums;

public static class OrderStatusTransitions
{
    // 🔹 1. RULES (data)
    private static readonly List<StatusTransitionRule> _rules = new()
    {
        new StatusTransitionRule
        {
            From = OrderStatusEnum.Draft,
            To = OrderStatusEnum.Submitted,
            AllowedRoles = new() { "Sales", "Admin" }
        },
        new StatusTransitionRule
        {
            From = OrderStatusEnum.Submitted,
            To = OrderStatusEnum.PendingReview,
            AllowedRoles = new() { "Operations", "Admin" }
        },
        new StatusTransitionRule
        {
            From = OrderStatusEnum.PendingReview,
            To = OrderStatusEnum.Approved,
            AllowedRoles = new() { "Operations", "Admin" }
        },
        new StatusTransitionRule
        {
            From = OrderStatusEnum.PendingReview,
            To = OrderStatusEnum.Failed,
            AllowedRoles = new() { "Operations", "Admin" }
        },
        new StatusTransitionRule
        {
            From = OrderStatusEnum.Approved,
            To = OrderStatusEnum.InProcessing,
            AllowedRoles = new() { "Operations", "Admin" }
        },
        new StatusTransitionRule
        {
            From = OrderStatusEnum.InProcessing,
            To = OrderStatusEnum.AwaitingDispatch,
            AllowedRoles = new() { "Operations", "Admin" }
        },
        new StatusTransitionRule
        {
            From = OrderStatusEnum.AwaitingDispatch,
            To = OrderStatusEnum.Completed,
            AllowedRoles = new() { "Operations", "Admin" }
        }
    };

    // 🔹 2. CORE METHODS (logic lives HERE)

    public static bool IsTerminal(OrderStatusEnum status)
    {
        return status is OrderStatusEnum.Completed
            or OrderStatusEnum.Failed
            or OrderStatusEnum.Cancelled;
    }

    public static bool CanTransition(
        OrderStatusEnum from,
        OrderStatusEnum to,
        string role)
    {
        if (IsTerminal(from))
            return false;

        // Global rule: cancel allowed from any non-terminal
        if (to == OrderStatusEnum.Cancelled)
            return true;

        return _rules.Any(r =>
            r.From == from &&
            r.To == to &&
            r.AllowedRoles.Contains(role));
    }

    public static List<OrderStatusEnum> GetAllowedTransitions(
        OrderStatusEnum from,
        string role)
    {
        if (IsTerminal(from))
            return new();

        var allowed = _rules
            .Where(r => r.From == from && r.AllowedRoles.Contains(role))
            .Select(r => r.To)
            .ToList();

        // Global cancel rule
        allowed.Add(OrderStatusEnum.Cancelled);

        return allowed.Distinct().ToList();
    }
}