using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Almacenes.CreateAlmacen;

public record CreateAlmacenCommand(
    string Nombre,
    string? Direccion = null,
    string? Descripcion = null,
    bool EsPrincipal = false) : ICommand<Guid>;
