using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.ValueObjects;
using PlanTA.Compras.Infrastructure.Data;
using PlanTA.CRM.Domain.Entities;
using PlanTA.CRM.Domain.Enums;
using PlanTA.CRM.Infrastructure.Data;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.Inventario.Domain.Enums;
using PlanTA.Inventario.Infrastructure.Data;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.RRHH.Infrastructure.Data;
using PlanTA.SharedKernel;

namespace PlanTA.API.Services;

public sealed class EmpresaDemoSeeder(
    CrmDbContext crm,
    InventarioDbContext inv,
    ComprasDbContext com,
    RRHHDbContext rrhh)
{
    public async Task<Result<int>> SeedAsync(Guid empresaId, CancellationToken ct = default)
    {
        var yaTiene = await crm.Leads.AnyAsync(l => l.EmpresaId == empresaId, ct);
        if (yaTiene)
            return Result<int>.Failure(Error.Conflict("Demo.YaInicializada", "La empresa ya tiene datos"));

        var total = 0;

        // --- CRM: 5 leads ---
        crm.Leads.AddRange(
            Lead.Crear("Juan García", OrigenLead.Web, empresaId, "Talleres García SL", "juan@talleresgarcia.es", "912345678"),
            Lead.Crear("María López", OrigenLead.Recomendacion, empresaId, "Mecanizados López", "maria@mecalopez.es", "913456789"),
            Lead.Crear("Carlos Ruiz", OrigenLead.Evento, empresaId, "Industrias Ruiz SA", "carlos@indruiz.com", "914567890"),
            Lead.Crear("Ana Martínez", OrigenLead.LlamadaFria, empresaId, "Ferretería Martínez", "ana@fmartinez.es", "915678901"),
            Lead.Crear("Pedro Sánchez", OrigenLead.Marketing, empresaId, "Componentes Sánchez", "pedro@compsanchez.es", "916789012")
        );
        total += 5;

        // --- Inventario: 8 productos ---
        inv.Productos.AddRange(
            Producto.Crear("SKU-001", "Tornillo M6 x 20", TipoProducto.Componente, UnidadMedida.Unidad, empresaId, "Tornillo hexagonal acero inox", precioCompra: 0.08m, precioVenta: 0.15m),
            Producto.Crear("SKU-002", "Arandela plana M6", TipoProducto.Componente, UnidadMedida.Unidad, empresaId, null, precioCompra: 0.02m, precioVenta: 0.05m),
            Producto.Crear("SKU-003", "Plancha aluminio 2mm", TipoProducto.MateriaPrima, UnidadMedida.MetroCuadrado, empresaId, "Plancha aluminio 1050 H14", precioCompra: 12.50m, precioVenta: 18.00m),
            Producto.Crear("SKU-004", "Barra acero 10mm", TipoProducto.MateriaPrima, UnidadMedida.Metro, empresaId, "Acero F-114 calibrado", precioCompra: 4.20m, precioVenta: 6.80m),
            Producto.Crear("SKU-005", "Conjunto soporte A-100", TipoProducto.ProductoTerminado, UnidadMedida.Unidad, empresaId, "Soporte industrial mecanizado", precioCompra: 0, precioVenta: 45.00m),
            Producto.Crear("SKU-006", "Kit racord hidráulico", TipoProducto.ProductoTerminado, UnidadMedida.Caja, empresaId, "Pack de 10 racords 1/2\"", precioCompra: 0, precioVenta: 89.00m),
            Producto.Crear("SKU-007", "Aceite corte soluble", TipoProducto.Consumible, UnidadMedida.Litro, empresaId, "Taladrina sintética", precioCompra: 3.50m, precioVenta: 6.00m),
            Producto.Crear("SKU-008", "Subconjunto motor B-200", TipoProducto.Semielaborado, UnidadMedida.Unidad, empresaId, null, precioCompra: 22.00m, precioVenta: 38.00m)
        );
        total += 8;

        // --- Compras: 3 proveedores ---
        // Instancia nueva de CondicionesPago por proveedor: EF owned types no pueden compartirse.
        static CondicionesPago Pago30() => new(30, 0m, "Transferencia");
        static CondicionesPago Pago60() => new(60, 2m, "Pagare");
        com.Proveedores.AddRange(
            Proveedor.Crear("Aceros del Norte SL", "B12345678", "ventas@acerosdelnorte.es", Pago30(), empresaId, "Pol. Ind. Norte 12", "Bilbao", "48001", "España", "944112233"),
            Proveedor.Crear("Suministros Industriales García", "B87654321", "pedidos@suminigarcia.es", Pago60(), empresaId, "Calle Mayor 45", "Zaragoza", "50001", "España", "976223344"),
            Proveedor.Crear("Ferretería Industrial Levante", "B11223344", "info@ferrileverante.es", Pago30(), empresaId, "Av. Puerto 89", "Valencia", "46001", "España", "963334455")
        );
        total += 3;

        // --- RRHH: 5 empleados ---
        var hoy = DateTimeOffset.UtcNow;
        rrhh.Empleados.AddRange(
            Empleado.Crear("EMP001", "Lucía", "Fernández Ruiz", "12345678A", "Responsable de planta", empresaId, hoy.AddYears(-5), 28m, "lucia.fernandez@demo.planta-erp.com", "611222333", "Producción"),
            Empleado.Crear("EMP002", "Roberto", "Gómez Díaz", "23456789B", "Operario CNC", empresaId, hoy.AddYears(-3), 18m, "roberto.gomez@demo.planta-erp.com", "622333444", "Producción"),
            Empleado.Crear("EMP003", "Elena", "Torres Vega", "34567890C", "Jefa de calidad", empresaId, hoy.AddYears(-4), 26m, "elena.torres@demo.planta-erp.com", "633444555", "Calidad"),
            Empleado.Crear("EMP004", "Miguel", "Castro Moreno", "45678901D", "Operario almacén", empresaId, hoy.AddYears(-2), 16m, "miguel.castro@demo.planta-erp.com", "644555666", "Almacén"),
            Empleado.Crear("EMP005", "Sara", "Jiménez Pardo", "56789012E", "Administrativa", empresaId, hoy.AddYears(-6), 20m, "sara.jimenez@demo.planta-erp.com", "655666777", "Administración")
        );
        total += 5;

        await crm.SaveChangesAsync(ct);
        await inv.SaveChangesAsync(ct);
        await com.SaveChangesAsync(ct);
        await rrhh.SaveChangesAsync(ct);

        return Result<int>.Success(total);
    }
}
