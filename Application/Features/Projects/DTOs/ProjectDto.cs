namespace Application.Features.Projects.DTOs;

public class ProjectDto
{
    public int ProjectId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}