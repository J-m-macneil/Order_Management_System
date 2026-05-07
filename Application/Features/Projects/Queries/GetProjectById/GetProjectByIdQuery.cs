using Application.Features.Projects.DTOs;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQuery : IRequest<ProjectDto?>
{
    public int ProjectId { get; set; }
}