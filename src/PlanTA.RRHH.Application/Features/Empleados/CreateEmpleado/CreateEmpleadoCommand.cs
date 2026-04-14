using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Empleados.CreateEmpleado;

public record CreateEmpleadoCommand(
    string Codigo, string Nombre, string Apellidos, string DNI, string Puesto,
    decimal CosteHoraEstandar = 0,
    string? Email = null, string? Telefono = null, string? Departamento = null,
    Guid? UserId = null) : ICommand<Guid>;
