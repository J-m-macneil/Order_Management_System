using Domain.Entities;
using Domain.Entities.Orders;

public class Document
{
    public int DocumentId { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string DocumentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
}