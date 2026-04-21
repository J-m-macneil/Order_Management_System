namespace Domain.Entities;

public class Project
{
    public int ProjectId { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public string ProjectCode { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string Status { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}