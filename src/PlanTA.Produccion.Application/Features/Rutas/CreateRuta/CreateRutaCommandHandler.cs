using MediatR;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.ValueObjects;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.Rutas.CreateRuta;

public sealed class CreateRutaCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateRutaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRutaCommand request, CancellationToken cancellationToken)
    {
        var ruta = RutaProduccion.Crear(
            request.ProductoId,
            request.Nombre,
            tenant.EmpresaId,
            request.Descripcion);

        if (request.Operaciones is not null)
        {
            foreach (var op in request.Operaciones)
            {
                ruta.AgregarOperacion(
                    op.Numero,
                    op.Nombre,
                    op.TipoOperacion,
                    new TiempoEstimado(op.TiempoEstimadoMinutos),
                    op.CentroTrabajo,
                    op.Instrucciones);
            }
        }

        db.RutasProduccion.Add(ruta);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(ruta.Id.Value);
    }
}
