using Application.Features.Projects.DTOs;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQuery : IRequest<List<ProjectDto>>
{
}