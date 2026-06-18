using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetDepartments;

public class GetDepartmentsQuery : IRequest<List<DepartmentDto>>
{
}
