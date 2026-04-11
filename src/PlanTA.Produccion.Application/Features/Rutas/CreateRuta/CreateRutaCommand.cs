using PlanTA.Produccion.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.Rutas.CreateRuta;

public record CreateRutaCommand(
    Guid ProductoId,
    string Nombre,
    string? Descripcion = null,
    List<CreateOperacionRequest>? Operaciones = null) : ICommand<Guid>;

public record CreateOperacionRequest(
    int Numero,
    string Nombre,
    TipoOperacion TipoOperacion,
    decimal TiempoEstimadoMinutos,
    string CentroTrabajo,
    string? Instrucciones = null);
