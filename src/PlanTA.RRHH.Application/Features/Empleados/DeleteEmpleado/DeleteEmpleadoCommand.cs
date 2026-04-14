using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Empleados.DeleteEmpleado;

public record DeleteEmpleadoCommand(Guid EmpleadoId) : ICommand<bool>;
