using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlanTA.Activos.Domain.Entities;
using PlanTA.Activos.Domain.Enums;
using PlanTA.Activos.Infrastructure.Data;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Enums;
using PlanTA.Calidad.Infrastructure.Data;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.ValueObjects;
using PlanTA.Compras.Infrastructure.Data;
using PlanTA.Costes.Domain.Entities;
using PlanTA.Costes.Domain.Enums;
using PlanTA.Costes.Infrastructure.Data;
using PlanTA.CRM.Domain.Entities;
using PlanTA.CRM.Domain.Enums;
using PlanTA.CRM.Infrastructure.Data;
using PlanTA.Facturacion.Domain.Entities;
using PlanTA.Facturacion.Domain.Enums;
using PlanTA.Facturacion.Infrastructure.Data;
using PlanTA.IA.Domain.Entities;
using PlanTA.IA.Domain.Enums;
using PlanTA.IA.Infrastructure.Data;
using PlanTA.Importador.Domain.Entities;
using PlanTA.Importador.Domain.Enums;
using PlanTA.Importador.Infrastructure.Data;
using PlanTA.Incidencias.Domain.Entities;
using PlanTA.Incidencias.Domain.Enums;
using PlanTA.Incidencias.Infrastructure.Data;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.Inventario.Domain.Enums;
using PlanTA.Inventario.Infrastructure.Data;
using PlanTA.Mantenimiento.Domain.Entities;
using PlanTA.Mantenimiento.Domain.Enums;
using PlanTA.Mantenimiento.Infrastructure.Data;
using PlanTA.OEE.Domain.Entities;
using PlanTA.OEE.Infrastructure.Data;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Infrastructure.Data;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.RRHH.Infrastructure.Data;
using PlanTA.Seguridad.Domain.Entities;
using PlanTA.Seguridad.Infrastructure.Data;
using PlanTA.Seguridad.Infrastructure.Identity;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Infrastructure.Data;

namespace PlanTA.API;

public static class DemoDataSeeder
{
    public static async Task RunAsync(IServiceProvider services, ILogger logger)
    {
        // Sufijo único por ejecución para evitar conflictos con runs parciales previos
        var sufijo = DateTime.UtcNow.ToString("yyyyMMddHHmm");
        var sfx = sufijo[^6..];
        var empresaNombre = $"Empresa Demo Seed {sufijo}";
        var empresaCif = $"B{sufijo[..8]}";
        var adminEmail = $"demoseed{sufijo}@planta-erp.com";
        var adminPassword = "DemoSeed2026!!";

        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var segDb = sp.GetRequiredService<SeguridadDbContext>();

        logger.LogInformation("== Iniciando DemoDataSeeder ({Nombre}) ==", empresaNombre);

        // --- 1. Empresa + admin ---
        var empresa = Empresa.Crear(empresaNombre, empresaCif, adminEmail);
        segDb.Empresas.Add(empresa);
        await segDb.SaveChangesAsync();
        var empresaId = empresa.Id.Value;
        logger.LogInformation("Empresa creada: {Id}", empresaId);

        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var rol in new[] { "Administrador", "GerentePlanta", "JefeAlmacen", "JefeProduccion", "Compras", "Ventas", "Calidad", "Operario" })
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole<Guid> { Name = rol });
        }

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            Nombre = "Admin Demo Seed",
            EmpresaId = empresaId,
            EmailConfirmed = true
        };
        var createUserRes = await userManager.CreateAsync(admin, adminPassword);
        if (!createUserRes.Succeeded)
            throw new Exception("No se pudo crear usuario admin: " + string.Join("; ", createUserRes.Errors.Select(e => e.Description)));
        await userManager.AddToRoleAsync(admin, "Administrador");
        var adminUserId = admin.Id;
        logger.LogInformation("Admin creado: {Email} / {Password}", adminEmail, adminPassword);

        // --- 2. Inventario ---
        var invDb = sp.GetRequiredService<InventarioDbContext>();
        var almacen1 = Almacen.Crear("Almacén Central", empresaId, "Polígono Industrial 1", "Almacén principal", esPrincipal: true);
        var almacen2 = Almacen.Crear("Almacén Secundario", empresaId, "Nave 7", "Materias primas");
        invDb.Almacenes.AddRange(almacen1, almacen2);

        var prodMP = Producto.Crear($"MP-001-{sfx}", "Acero Inoxidable 304 (kg)", TipoProducto.MateriaPrima, UnidadMedida.Kilogramo, empresaId,
            "Acero inox grado 304, plancha 2mm", precioCompra: 4.50m, precioVenta: 0m);
        var prodComp = Producto.Crear($"CMP-001-{sfx}", "Tornillo M8x40 inox", TipoProducto.Componente, UnidadMedida.Unidad, empresaId,
            precioCompra: 0.12m, precioVenta: 0.30m);
        var prodPT = Producto.Crear($"PT-001-{sfx}", "Carcasa Mecanizada A1", TipoProducto.ProductoTerminado, UnidadMedida.Unidad, empresaId,
            "Carcasa de aluminio mecanizada", precioCompra: 0m, precioVenta: 89.50m);
        var prodPT2 = Producto.Crear($"PT-002-{sfx}", "Carcasa Mecanizada A2", TipoProducto.ProductoTerminado, UnidadMedida.Unidad, empresaId,
            precioCompra: 0m, precioVenta: 119.00m);
        var prodMP2 = Producto.Crear($"MP-002-{sfx}", "Aluminio 6061 (kg)", TipoProducto.MateriaPrima, UnidadMedida.Kilogramo, empresaId,
            precioCompra: 6.20m, precioVenta: 0m);
        invDb.Productos.AddRange(prodMP, prodComp, prodPT, prodPT2, prodMP2);

        var lote1 = Lote.Crear(prodMP.Id, 500m, empresaId, $"L-MP001-{sfx}", DateTimeOffset.UtcNow.AddYears(2), "Proveedor A");
        var lote2 = Lote.Crear(prodMP2.Id, 300m, empresaId, $"L-MP002-{sfx}", DateTimeOffset.UtcNow.AddYears(2), "Proveedor B");
        invDb.Lotes.AddRange(lote1, lote2);

        var alerta = AlertaStock.Crear(prodComp.Id, stockMinimo: 200m, stockMaximo: 2000m, empresaId, almacenId: almacen1.Id, puntoReorden: 300m, cantidadReorden: 1500m);
        invDb.Alertas.Add(alerta);
        await invDb.SaveChangesAsync();
        logger.LogInformation("Inventario sembrado");

        // --- 3. Producción ---
        var prodDb = sp.GetRequiredService<ProduccionDbContext>();
        var bom = ListaMateriales.Crear(prodPT.Id.Value, "BOM Carcasa A1 v1", empresaId, "Lista de materiales para PT-001");
        bom.AgregarLinea(prodMP.Id.Value, 0.8m, "kg", merma: 0.05m, orden: 1);
        bom.AgregarLinea(prodComp.Id.Value, 8m, "u", merma: 0m, orden: 2);
        prodDb.ListasMateriales.Add(bom);

        var ruta = RutaProduccion.Crear(prodPT.Id.Value, "Ruta Mecanizado A1", empresaId);
        ruta.AgregarOperacion(1, "Corte plancha", PlanTA.Produccion.Domain.Enums.TipoOperacion.Corte, new PlanTA.Produccion.Domain.ValueObjects.TiempoEstimado(15), "CT-CORTE-01");
        ruta.AgregarOperacion(2, "Ensamblaje componentes", PlanTA.Produccion.Domain.Enums.TipoOperacion.Ensamblaje, new PlanTA.Produccion.Domain.ValueObjects.TiempoEstimado(45), "CT-CNC-01");
        ruta.AgregarOperacion(3, "Inspección final", PlanTA.Produccion.Domain.Enums.TipoOperacion.Inspeccion, new PlanTA.Produccion.Domain.ValueObjects.TiempoEstimado(20), "CT-CTRL-01");
        prodDb.RutasProduccion.Add(ruta);

        var of = OrdenFabricacion.Crear($"OF-{sfx}-1", prodPT.Id.Value, bom.Id, 50m, "u", empresaId, ruta.Id, prioridad: 1, observaciones: "Pedido cliente urgente");
        prodDb.OrdenesFabricacion.Add(of);

        var of2 = OrdenFabricacion.Crear($"OF-{sfx}-2", prodPT2.Id.Value, bom.Id, 25m, "u", empresaId, ruta.Id, prioridad: 2);
        prodDb.OrdenesFabricacion.Add(of2);
        await prodDb.SaveChangesAsync();
        logger.LogInformation("Producción sembrada");

        // --- 4. Compras ---
        var compDb = sp.GetRequiredService<ComprasDbContext>();
        var condPago30 = new CondicionesPago(30, 0m, "Transferencia");
        var condPago60 = new CondicionesPago(60, 1.5m, "Transferencia");

        var prov1 = Proveedor.Crear("Aceros del Norte SL", $"B{sfx}01", "ventas@acerosnorte.es", condPago30, empresaId,
            "Pol. Ind. Las Hayas, 5", "Bilbao", "48015", "España", "+34 944 123 456", "https://acerosnorte.es");
        prov1.AgregarContacto("Luis Echeverría", "lecheverria@acerosnorte.es", "Comercial", "+34 600 111 222", esPrincipal: true);
        var prov2 = Proveedor.Crear("Tornillería Industrial Levante", $"B{sfx}02", "info@tornilleria-levante.es", condPago60, empresaId,
            "Av. Industria 12", "Valencia", "46014", "España", "+34 963 456 789");
        compDb.Proveedores.AddRange(prov1, prov2);

        var oc = OrdenCompra.Crear($"OC-{sfx}-1", prov1.Id, empresaId, DateTimeOffset.UtcNow.AddDays(7), "Reposición materia prima Q1");
        oc.AgregarLinea(prodMP.Id.Value, "Acero Inox 304", 200m, 4.50m);
        oc.AgregarLinea(prodMP2.Id.Value, "Aluminio 6061", 100m, 6.20m);
        compDb.OrdenesCompra.Add(oc);

        var oc2 = OrdenCompra.Crear($"OC-{sfx}-2", prov2.Id, empresaId, DateTimeOffset.UtcNow.AddDays(14));
        oc2.AgregarLinea(prodComp.Id.Value, "Tornillo M8x40 inox", 5000m, 0.12m);
        oc2.Enviar();
        compDb.OrdenesCompra.Add(oc2);
        await compDb.SaveChangesAsync();
        logger.LogInformation("Compras sembrado");

        // --- 5. Ventas ---
        var venDb = sp.GetRequiredService<VentasDbContext>();
        var cli1 = Cliente.Crear("Mecanizados García SL", $"B{sfx}11", "compras@mecanizadosgarcia.es", empresaId,
            "C/ Industria 45", "C/ Industria 45", "Madrid", "28042", "España", "+34 911 222 333");
        cli1.AgregarContacto("Pedro García", "pgarcia@mecanizadosgarcia.es", "Director Compras", "+34 600 333 444", esPrincipal: true);
        var cli2 = Cliente.Crear("Industrias Soler SA", $"A{sfx}22", "pedidos@soler.com", empresaId,
            "Pol. Ind. Sur, Nave 14", "Pol. Ind. Sur, Nave 14", "Barcelona", "08040", "España", "+34 933 444 555");
        venDb.Clientes.AddRange(cli1, cli2);

        var ped = Pedido.Crear($"PED-{sfx}-1", cli1.Id, empresaId, DateTimeOffset.UtcNow.AddDays(10), "C/ Industria 45, Madrid", "Entrega antes de las 10:00");
        ped.AgregarLinea(prodPT.Id.Value, "Carcasa Mecanizada A1", 30m, 89.50m);
        ped.AgregarLinea(prodPT2.Id.Value, "Carcasa Mecanizada A2", 10m, 119.00m);
        venDb.Pedidos.Add(ped);

        var ped2 = Pedido.Crear($"PED-{sfx}-2", cli2.Id, empresaId, DateTimeOffset.UtcNow.AddDays(21));
        ped2.AgregarLinea(prodPT.Id.Value, "Carcasa Mecanizada A1", 50m, 87.00m, descuento: 5m);
        ped2.Confirmar();
        venDb.Pedidos.Add(ped2);
        await venDb.SaveChangesAsync();
        logger.LogInformation("Ventas sembrado");

        // --- 6. Calidad ---
        var calDb = sp.GetRequiredService<CalidadDbContext>();
        var plantilla = PlantillaInspeccion.Crear("Inspección Recepción Acero", OrigenInspeccion.Recepcion, empresaId, "Control de calidad para acero entrante");
        plantilla.AgregarCriterio("Espesor", "Numerico", esObligatorio: true, descripcion: "Espesor en mm", valorMinimo: 1.95m, valorMaximo: 2.05m, unidadMedida: "mm");
        plantilla.AgregarCriterio("Inspección visual", "Visual", esObligatorio: true, descripcion: "Sin defectos visibles");
        plantilla.AgregarCriterio("Certificado material", "Documental", esObligatorio: true, descripcion: "Certificado 3.1 incluido");
        calDb.PlantillasInspeccion.Add(plantilla);

        var plantilla2 = PlantillaInspeccion.Crear("Inspección Final OF", OrigenInspeccion.Produccion, empresaId);
        plantilla2.AgregarCriterio("Dimensiones", "Numerico", esObligatorio: true, valorMinimo: 99.5m, valorMaximo: 100.5m, unidadMedida: "mm");
        plantilla2.AgregarCriterio("Acabado superficial", "Visual", esObligatorio: true);
        calDb.PlantillasInspeccion.Add(plantilla2);
        await calDb.SaveChangesAsync();
        logger.LogInformation("Calidad sembrado");

        // --- 7. Activos ---
        var actDb = sp.GetRequiredService<ActivosDbContext>();
        var act1 = Activo.Crear($"MAQ-CNC-{sfx}", "CNC Mazak Variaxis i-700", TipoActivo.Maquina, CriticidadActivo.Critica, empresaId,
            "Centro mecanizado 5 ejes", null, "Nave A - Zona CNC", "Mazak", "Variaxis i-700", $"MZ-{sfx}", DateTimeOffset.UtcNow.AddYears(-3), 285000m);
        var act2 = Activo.Crear($"MAQ-CORTE-{sfx}", "Sierra de cinta Behringer HBP310", TipoActivo.Maquina, CriticidadActivo.Alta, empresaId,
            null, null, "Nave A - Zona Corte", "Behringer", "HBP310", $"BHR-{sfx}", DateTimeOffset.UtcNow.AddYears(-5), 42000m);
        var act3 = Activo.Crear($"VEH-CARR-{sfx}", "Carretilla Linde H30", TipoActivo.Vehiculo, CriticidadActivo.Media, empresaId,
            null, null, "Almacén Central", "Linde", "H30", $"LND-{sfx}", DateTimeOffset.UtcNow.AddYears(-2), 28000m);
        actDb.Activos.AddRange(act1, act2, act3);
        await actDb.SaveChangesAsync();
        logger.LogInformation("Activos sembrado");

        // --- 8. Mantenimiento ---
        var mtoDb = sp.GetRequiredService<MantenimientoDbContext>();
        var ot1 = OrdenTrabajo.Crear($"OT-{sfx}-1", "Cambio aceite husillo CNC", TipoMantenimiento.Preventivo, act1.Id.Value, empresaId,
            "Cambio programado de aceite hidráulico del husillo", PrioridadOT.Media, DateTimeOffset.UtcNow.AddDays(3), 4m);
        var ot2 = OrdenTrabajo.Crear($"OT-{sfx}-2", "Reparación sistema refrigeración", TipoMantenimiento.Correctivo, act1.Id.Value, empresaId,
            "Fuga detectada en circuito", PrioridadOT.Alta, DateTimeOffset.UtcNow.AddDays(1), 6m);
        var ot3 = OrdenTrabajo.Crear($"OT-{sfx}-3", "Revisión anual carretilla", TipoMantenimiento.Preventivo, act3.Id.Value, empresaId,
            null, PrioridadOT.Baja, DateTimeOffset.UtcNow.AddDays(15), 2m);
        mtoDb.OrdenesTrabajo.AddRange(ot1, ot2, ot3);
        await mtoDb.SaveChangesAsync();
        logger.LogInformation("Mantenimiento sembrado");

        // --- 9. Incidencias ---
        var incDb = sp.GetRequiredService<IncidenciasDbContext>();
        var inc1 = Incidencia.Crear($"INC-{sfx}-1", "Ruido anormal husillo CNC", "Se escucha vibración en arranque", TipoIncidencia.Averia,
            SeveridadIncidencia.Alta, adminUserId, empresaId, act1.Id.Value, "Nave A - CNC", paradaLinea: false);
        var inc2 = Incidencia.Crear($"INC-{sfx}-2", "Carretilla no arranca", "La carretilla H30 no enciende esta mañana", TipoIncidencia.Averia,
            SeveridadIncidencia.Media, adminUserId, empresaId, act3.Id.Value, "Almacén Central", paradaLinea: false);
        incDb.Incidencias.AddRange(inc1, inc2);
        await incDb.SaveChangesAsync();
        logger.LogInformation("Incidencias sembrado");

        // --- 10. CRM ---
        var crmDb = sp.GetRequiredService<CrmDbContext>();
        var lead1 = Lead.Crear("Construcciones Pérez", OrigenLead.Web, empresaId, "Construcciones Pérez SL",
            "info@construccionesperez.es", "+34 911 555 666", "Interesado en mecanizado a medida");
        var lead2 = Lead.Crear("Talleres Hermanos Rodríguez", OrigenLead.Evento, empresaId, "Talleres HR",
            "contacto@thr.es", "+34 933 777 888", "Conocidos en feria BIEMH 2026");
        var lead3 = Lead.Crear("Industrial Aragón", OrigenLead.Recomendacion, empresaId, "Industrial Aragón SA",
            "ventas@industarag.com", null, "Referido por cliente actual");
        crmDb.Leads.AddRange(lead1, lead2, lead3);
        await crmDb.SaveChangesAsync();
        logger.LogInformation("CRM sembrado");

        // --- 11. Costes ---
        var cosDb = sp.GetRequiredService<CostesDbContext>();
        var imp1 = ImputacionCoste.Crear(TipoCoste.ManoObra, OrigenImputacion.Manual, 8m, 22.50m, empresaId,
            of.Id.Value, null, null, "Mecanizado OF-2026-0001 - Operario A").Value!;
        var imp2 = ImputacionCoste.Crear(TipoCoste.Material, OrigenImputacion.ConsumoStock, 40m, 4.50m, empresaId,
            of.Id.Value, null, prodMP.Id.Value, "Consumo acero OF-2026-0001").Value!;
        var imp3 = ImputacionCoste.Crear(TipoCoste.Maquina, OrigenImputacion.OrdenTrabajo, 6m, 35m, empresaId,
            null, ot2.Id.Value, null, "Horas máquina OT correctiva").Value!;
        cosDb.Imputaciones.AddRange(imp1, imp2, imp3);
        await cosDb.SaveChangesAsync();
        logger.LogInformation("Costes sembrado");

        // --- 12. Facturación ---
        var facDb = sp.GetRequiredService<FacturacionDbContext>();
        var facNumBase = int.Parse(sfx);
        var fac1 = Factura.Crear($"A-{sfx}", facNumBase, 2026, cli1.Id.Value, "Mecanizados García SL", empresaId, TipoFactura.Ordinaria,
            $"B{sfx}11", "C/ Industria 45, 28042 Madrid", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(30), $"Pedido PED-{sfx}-1");
        fac1.AgregarLinea("Carcasa Mecanizada A1", 30m, 89.50m, 21m, 0m, prodPT.Id.Value);
        fac1.AgregarLinea("Carcasa Mecanizada A2", 10m, 119.00m, 21m, 0m, prodPT2.Id.Value);

        var fac2 = Factura.Crear($"A-{sfx}", facNumBase + 1, 2026, cli2.Id.Value, "Industrias Soler SA", empresaId, TipoFactura.Ordinaria,
            $"A{sfx}22", "Pol. Ind. Sur, Nave 14, 08040 Barcelona");
        fac2.AgregarLinea("Carcasa Mecanizada A1", 50m, 87.00m, 21m, 5m);
        facDb.Facturas.AddRange(fac1, fac2);
        await facDb.SaveChangesAsync();
        logger.LogInformation("Facturación sembrado");

        // --- 13. OEE ---
        var oeeDb = sp.GetRequiredService<OEEDbContext>();
        var oee1 = RegistroOEE.Crear(act1.Id.Value, new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero), 480, 420, 250, 240, 90m, empresaId,
            null, of.Id.Value, "Turno mañana - producción normal").Value!;
        var oee2 = RegistroOEE.Crear(act1.Id.Value, new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero).AddDays(-1), 480, 380, 230, 215, 90m, empresaId,
            null, of.Id.Value, "Parada por avería refrigeración").Value!;
        var oee3 = RegistroOEE.Crear(act2.Id.Value, new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero), 480, 460, 180, 178, 150m, empresaId,
            null, null, "Sierra - normal").Value!;
        oeeDb.Registros.AddRange(oee1, oee2, oee3);
        await oeeDb.SaveChangesAsync();
        logger.LogInformation("OEE sembrado");

        // --- 14. RRHH ---
        var rrhhDb = sp.GetRequiredService<RRHHDbContext>();
        var emp1 = Empleado.Crear($"E-{sfx}-1", "Carlos", "Martínez Ruiz", $"{sfx}1A", "Operario CNC", empresaId,
            DateTimeOffset.UtcNow.AddYears(-4), 18.50m, $"cmartinez{sfx}@demoseed.com", "+34 600 100 200", "Producción");
        var emp2 = Empleado.Crear($"E-{sfx}-2", "Laura", "Sánchez Gómez", $"{sfx}2B", "Jefa de Calidad", empresaId,
            DateTimeOffset.UtcNow.AddYears(-2), 26m, $"lsanchez{sfx}@demoseed.com", "+34 600 300 400", "Calidad");
        var emp3 = Empleado.Crear($"E-{sfx}-3", "Miguel", "López Torres", $"{sfx}3C", "Técnico Mantenimiento", empresaId,
            DateTimeOffset.UtcNow.AddYears(-6), 22m, $"mlopez{sfx}@demoseed.com", "+34 600 500 600", "Mantenimiento");
        rrhhDb.Empleados.AddRange(emp1, emp2, emp3);
        await rrhhDb.SaveChangesAsync();
        logger.LogInformation("RRHH sembrado");

        // --- 15. IA ---
        var iaDb = sp.GetRequiredService<IADbContext>();
        var conv = ConversacionIA.Crear("Análisis paradas CNC marzo", ContextoIA.Mantenimiento, adminUserId, empresaId);
        conv.AgregarMensaje(RolMensaje.User, "¿Cuáles son las principales causas de parada del CNC este mes?");
        conv.AgregarMensaje(RolMensaje.Assistant, "Según los registros OEE, el 60% de las paradas corresponden a fallos del sistema de refrigeración. Recomiendo programar una OT preventiva.", 18, 32, "claude-opus-4-6");
        iaDb.Conversaciones.Add(conv);
        await iaDb.SaveChangesAsync();
        logger.LogInformation("IA sembrado");

        // --- 16. Importador ---
        var impDb = sp.GetRequiredService<ImportadorDbContext>();
        var job = ImportJob.Crear(TipoImportacion.Productos, FormatoArchivo.Csv, "productos_q1_2026.csv", adminUserId, empresaId);
        impDb.Jobs.Add(job);
        await impDb.SaveChangesAsync();
        logger.LogInformation("Importador sembrado");

        logger.LogInformation("== DemoDataSeeder COMPLETADO ==");
        logger.LogInformation("Login: {Email} / {Password}", adminEmail, adminPassword);
    }
}
