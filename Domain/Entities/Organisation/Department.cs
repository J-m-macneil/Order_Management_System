using Domain.Entities.Identity;

namespace Domain.Entities.Organisation;

public class Department
{
    public int DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
}
