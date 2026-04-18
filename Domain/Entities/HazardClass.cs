namespace Domain.Entities;

public class HazardClass
{
    public int HazardClassId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}