using PlanTA.RRHH.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Empleados.ListEmpleados;

public record ListEmpleadosQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<EmpleadoListDto>>;
