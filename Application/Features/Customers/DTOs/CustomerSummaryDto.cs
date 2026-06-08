namespace Application.Features.Customers.DTOs;

public class CustomerSummaryDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int InactiveCustomers { get; set; }
}
