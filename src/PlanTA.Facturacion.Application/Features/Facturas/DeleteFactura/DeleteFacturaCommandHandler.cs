using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Facturacion.Application.Interfaces;
using PlanTA.Facturacion.Domain.Entities;
using PlanTA.Facturacion.Domain.Enums;
using PlanTA.Facturacion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Application.Features.Facturas.DeleteFactura;

public sealed class DeleteFacturaCommandHandler(
    IFacturacionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteFacturaCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteFacturaCommand request, CancellationToken ct)
    {
        var id = new FacturaId(request.Id);
        var factura = await db.Facturas
            .FirstOrDefaultAsync(f => f.Id == id && f.EmpresaId == tenant.EmpresaId, ct);

        if (factura is null)
            return Result<bool>.Failure(FacturaErrors.NotFound(request.Id));

        if (factura.Estado != EstadoFactura.Borrador)
            return Result<bool>.Failure(FacturaErrors.EstadoInvalido(factura.Estado.ToString()));

        factura.SoftDelete();
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
