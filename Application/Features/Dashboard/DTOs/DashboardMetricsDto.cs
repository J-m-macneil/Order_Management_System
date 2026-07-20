namespace Application.Features.Dashboard.Queries.GetDashboardMetrics;

public class DashboardMetricsDto
{
    public MetricsDto Metrics { get; set; } = new();
    public List<StatusCountDto> OrdersByStatus { get; set; } = new();
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
    public List<RecentFailureDto> RecentFailures { get; set; } = new();
    public List<PriorityOrderDto> PriorityOrders { get; set; } = new();
}

public class MetricsDto
{
    public int TotalOrders { get; set; }
    public int ActiveOrders { get; set; }
    public int FailedOrders { get; set; }
    public decimal TotalValue { get; set; }
}

public class StatusCountDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public int Orders { get; set; }
}

public class RecentFailureDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool RequiresAction { get; set; }
}

public class PriorityOrderDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
}
