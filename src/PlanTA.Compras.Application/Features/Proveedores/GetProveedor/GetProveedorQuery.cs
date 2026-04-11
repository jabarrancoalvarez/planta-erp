using PlanTA.Compras.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.Proveedores.GetProveedor;

public record GetProveedorQuery(Guid ProveedorId) : IQuery<ProveedorDetailDto>;
