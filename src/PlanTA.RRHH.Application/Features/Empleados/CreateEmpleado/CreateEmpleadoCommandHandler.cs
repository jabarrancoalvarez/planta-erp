using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.RRHH.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Empleados.CreateEmpleado;

public sealed class CreateEmpleadoCommandHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateEmpleadoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateEmpleadoCommand request, CancellationToken ct)
    {
        var dniNorm = request.DNI.Trim().ToUpperInvariant();
        var dup = await db.Empleados
            .AnyAsync(e => e.DNI == dniNorm && e.EmpresaId == tenant.EmpresaId, ct);

        if (dup) return Result<Guid>.Failure(EmpleadoErrors.DNIDuplicado(request.DNI));

        var emp = Empleado.Crear(
            request.Codigo, request.Nombre, request.Apellidos, request.DNI, request.Puesto,
            tenant.EmpresaId, fechaAlta: null, costeHoraEstandar: request.CosteHoraEstandar,
            email: request.Email, telefono: request.Telefono, departamento: request.Departamento,
            userId: request.UserId);

        db.Empleados.Add(emp);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(emp.Id.Value);
    }
}
