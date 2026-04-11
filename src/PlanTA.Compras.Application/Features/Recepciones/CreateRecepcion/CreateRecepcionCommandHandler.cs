using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Enums;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.Recepciones.CreateRecepcion;

public sealed class CreateRecepcionCommandHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateRecepcionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRecepcionCommand request, CancellationToken cancellationToken)
    {
        var oc = await db.OrdenesCompra
            .Include(o => o.Lineas)
            .Where(o => o.Id == new OrdenCompraId(request.OrdenCompraId) && o.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (oc is null)
            return Result<Guid>.Failure(OrdenCompraErrors.NotFound(request.OrdenCompraId));

        if (oc.EstadoOC is not (EstadoOC.Enviada or EstadoOC.ParcialmenteRecibida))
            return Result<Guid>.Failure(
                OrdenCompraErrors.TransicionInvalida(oc.EstadoOC.ToString(), "Recepcion"));

        var recepcion = Recepcion.Crear(
            new OrdenCompraId(request.OrdenCompraId),
            tenant.EmpresaId,
            request.NumeroAlbaran,
            request.Observaciones);

        if (request.Lineas is not null)
        {
            foreach (var lineaReq in request.Lineas)
            {
                var lineaOC = oc.Lineas.FirstOrDefault(l => l.Id == new LineaOrdenCompraId(lineaReq.LineaOrdenCompraId));
                if (lineaOC is null)
                    continue;

                var pendiente = lineaOC.CantidadPendiente;
                if (lineaReq.CantidadRecibida > pendiente)
                    return Result<Guid>.Failure(
                        RecepcionErrors.CantidadExcedida(lineaReq.LineaOrdenCompraId, pendiente, lineaReq.CantidadRecibida));

                recepcion.AgregarLinea(
                    new LineaOrdenCompraId(lineaReq.LineaOrdenCompraId),
                    lineaReq.ProductoId,
                    lineaReq.CantidadRecibida,
                    lineaReq.LoteId,
                    lineaReq.UbicacionDestinoId);

                lineaOC.RegistrarRecepcion(lineaReq.CantidadRecibida);
            }
        }

        // Update OC state based on lines
        if (oc.TodasLineasRecibidas())
            oc.RecibirCompleta();
        else if (oc.AlgunaLineaParcialmenteRecibida() || oc.Lineas.Any(l => l.CantidadRecibida > 0))
            oc.RecibirParcialmente();

        db.Recepciones.Add(recepcion);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(recepcion.Id.Value);
    }
}
