using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Empleados.UpdateEmpleado;

public record UpdateEmpleadoCommand(
    Guid EmpleadoId,
    string Nombre,
    string Apellidos,
    string Puesto,
    string? Email,
    string? Telefono,
    string? Departamento,
    decimal CosteHoraEstandar,
    int DiasVacacionesAnuales) : ICommand<bool>;
