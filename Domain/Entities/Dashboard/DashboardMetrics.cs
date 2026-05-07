namespace Domain.Models;

public class DashboardMetrics
{
    public Metrics Metrics { get; set; } = new();
    public List<StatusCount> OrdersByStatus { get; set; } = new();
    public List<TopCustomer> TopCustomers { get; set; } = new();
    public List<RecentFailure> RecentFailures { get; set; } = new();
    public List<PriorityOrder> PriorityOrders { get; set; } = new();
}

public class Metrics
{
    public int TotalOrders { get; set; }
    public int ActiveOrders { get; set; }
    public int FailedOrders { get; set; }
    public decimal TotalValue { get; set; }
}

public class StatusCount
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopCustomer
{
    public string Name { get; set; } = string.Empty;
    public int Orders { get; set; }
}

public class RecentFailure
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class PriorityOrder
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
}