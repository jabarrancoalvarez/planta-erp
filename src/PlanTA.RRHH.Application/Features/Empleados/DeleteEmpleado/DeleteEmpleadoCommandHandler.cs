using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.RRHH.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Empleados.DeleteEmpleado;

public sealed class DeleteEmpleadoCommandHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteEmpleadoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteEmpleadoCommand request, CancellationToken ct)
    {
        var emp = await db.Empleados
            .FirstOrDefaultAsync(e => e.Id == new EmpleadoId(request.EmpleadoId) && e.EmpresaId == tenant.EmpresaId, ct);
        if (emp is null)
            return Result<bool>.Failure(EmpleadoErrors.NotFound(request.EmpleadoId));
        emp.SoftDelete();
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
