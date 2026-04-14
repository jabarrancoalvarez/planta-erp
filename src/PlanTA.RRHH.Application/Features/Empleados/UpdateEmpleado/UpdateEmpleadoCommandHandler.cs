using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.RRHH.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Empleados.UpdateEmpleado;

public sealed class UpdateEmpleadoCommandHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateEmpleadoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateEmpleadoCommand request, CancellationToken ct)
    {
        var emp = await db.Empleados
            .FirstOrDefaultAsync(e => e.Id == new EmpleadoId(request.EmpleadoId) && e.EmpresaId == tenant.EmpresaId, ct);
        if (emp is null)
            return Result<bool>.Failure(EmpleadoErrors.NotFound(request.EmpleadoId));
        emp.Editar(
            request.Nombre, request.Apellidos, request.Puesto,
            request.Email, request.Telefono, request.Departamento,
            request.CosteHoraEstandar, request.DiasVacacionesAnuales);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
