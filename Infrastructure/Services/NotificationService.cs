using Application.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _dbContext;
    private readonly IAuditService _auditService;

    public NotificationService(
        AppDbContext dbContext,
        IAuditService auditService)
    {
        _dbContext = dbContext;
        _auditService = auditService;
    }

    public async Task CreateOrderSubmittedNotificationAsync(
        int orderId,
        CancellationToken cancellationToken)
    {
        await CreateNotificationAsync(
            orderId,
            "OrderSubmitted",
            "Order submitted",
            "Pending",
            sentAt: null,
            auditAction: "Created",
            auditNote: "Order submitted notification created.",
            cancellationToken);
    }

    private async Task CreateNotificationAsync(
        int orderId,
        string notificationType,
        string subjectPrefix,
        string status,
        DateTime? sentAt,
        string auditAction,
        string auditNote,
        CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Notifications.AnyAsync(n =>
            n.OrderId == orderId &&
            n.NotificationType == notificationType,
            cancellationToken);

        if (exists)
        {
            return;
        }

        var order = await _dbContext.Orders
            .Include(o => o.Customer)
            .FirstAsync(o => o.OrderId == orderId, cancellationToken);

        var recipientEmail = $"purchasing{order.CustomerId}@simulated-customer.co.uk";

        var notification = new Notification
        {
            OrderId = order.OrderId,
            RecipientEmail = recipientEmail,
            NotificationType = notificationType,
            Subject = $"{subjectPrefix}: {order.OrderNumber}",
            CreatedAt = DateTime.UtcNow,
            SentAt = sentAt,
            Status = status
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _auditService.AddSystemAction(
            "Notification",
            notification.NotificationId,
            auditAction,
            null,
            new
            {
                notification.NotificationId,
                order.OrderId,
                NotificationType = notification.NotificationType,
                RecipientEmail = recipientEmail
            },
            auditNote);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
