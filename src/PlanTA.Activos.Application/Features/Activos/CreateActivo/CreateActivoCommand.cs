using PlanTA.Activos.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Activos.Application.Features.Activos.CreateActivo;

public record CreateActivoCommand(
    string Codigo,
    string Nombre,
    TipoActivo Tipo,
    CriticidadActivo Criticidad,
    string? Descripcion = null,
    Guid? ActivoPadreId = null,
    string? Ubicacion = null,
    string? Fabricante = null,
    string? Modelo = null,
    string? NumeroSerie = null,
    DateTimeOffset? FechaAdquisicion = null,
    decimal CosteAdquisicion = 0) : ICommand<Guid>;
