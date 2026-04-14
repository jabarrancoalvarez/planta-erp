using MediatR;
using PlanTA.Incidencias.Application.Interfaces;
using PlanTA.Incidencias.Domain.Entities;
using PlanTA.Incidencias.Domain.Enums;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Incidencias.Application.Features.Incidencias.CreateIncidencia;

public record CreateIncidenciaCommand(
    string Codigo,
    string Titulo,
    string Descripcion,
    TipoIncidencia Tipo,
    SeveridadIncidencia Severidad,
    Guid ReportadoPorUserId,
    Guid? ActivoId = null,
    string? UbicacionTexto = null,
    bool ParadaLinea = false,
    List<string>? FotosUrl = null) : ICommand<Guid>;

public sealed class CreateIncidenciaCommandHandler(
    IIncidenciasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateIncidenciaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateIncidenciaCommand request, CancellationToken cancellationToken)
    {
        var inc = Incidencia.Crear(
            request.Codigo, request.Titulo, request.Descripcion, request.Tipo,
            request.Severidad, request.ReportadoPorUserId, tenant.EmpresaId,
            request.ActivoId, request.UbicacionTexto, request.ParadaLinea);

        if (request.FotosUrl is { Count: > 0 })
        {
            foreach (var url in request.FotosUrl)
                inc.AgregarFoto(FotoIncidencia.Crear(inc.Id, url));
        }

        db.Incidencias.Add(inc);
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(inc.Id.Value);
    }
}
