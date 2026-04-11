# ARCHITECTURE.md — PlanTA

> ERP/MES ligero para PYMEs y pequeñas fábricas.
> Sustituye Excel con una plataforma modular de gestión de inventario, producción, compras, ventas y trazabilidad.

---

## 1. Visión del Producto

**PlanTA** (juego de palabras: planta de producción + planificación) es un sistema de gestión integral para pequeñas y medianas empresas industriales. Cubre desde el control de stock hasta la planificación de producción, pasando por compras, ventas, almacenes y trazabilidad de lotes.

### Propuesta de valor
- Reemplazar Excel y procesos manuales con un sistema modular y accesible
- Interfaz intuitiva que no requiere formación SAP
- Módulos activables por empresa (multi-tenant preparado)
- Coste accesible para PYMEs (SaaS o self-hosted)

### Usuarios objetivo
| Rol | Responsabilidades |
|---|---|
| **Administrador** | Configuración global, usuarios, empresa |
| **Gerente de planta** | Visión 360, KPIs, aprobaciones |
| **Jefe de almacén** | Stock, recepciones, expediciones, ubicaciones |
| **Jefe de producción** | Órdenes de fabricación, BOM, planificación |
| **Compras** | Proveedores, órdenes de compra, recepciones |
| **Ventas** | Clientes, pedidos, expediciones |
| **Operario** | Escaneo de lotes, movimientos de stock, fichaje en OF |
| **Calidad** | Inspecciones, no conformidades, trazabilidad |

---

## 2. Stack Tecnológico

| Capa | Tecnología | Versión | Justificación |
|---|---|---|---|
| Backend | ASP.NET Core | .NET 9 / C# 13 | Stack probado en producción en otros proyectos |
| Arquitectura | Clean Architecture + CQRS | — | Separación de concerns, testabilidad |
| CQRS | MediatR | 12.x | Desacoplamiento commands/queries |
| Validación | FluentValidation | 11.x | Validación declarativa en pipeline |
| ORM | Entity Framework Core + Npgsql | 9.x | PostgreSQL nativo |
| Base de datos | PostgreSQL | 16+ | Un schema por módulo, robusto para PYME |
| Auth | ASP.NET Core Identity + JWT | — | Refresh tokens, roles, multi-tenant |
| Jobs | Hangfire + PostgreSQL | latest | Outbox processor, tareas programadas |
| Logs | Serilog → Seq | 4.x | Logging estructurado |
| Tests | xUnit + FluentAssertions + NSubstitute | latest | Unitarios + integración |
| Frontend | Angular | 21 | Standalone components, signals, última versión |
| CSS | SCSS + Variables de diseño | — | Sistema de diseño propio industrial |
| Despliegue | Render (API) + Vercel (frontend) + Neon (DB) | — | Mismo stack de infra probado |
| Contenedores | Docker + Docker Compose | — | Desarrollo local |

---

## 3. Arquitectura Backend — Clean Architecture Multi-Módulo

### 3.1 Estructura de solución

```
PlanTA.sln
│
src/
├── PlanTA.SharedKernel/                # Primitivas comunes a todos los módulos
├── PlanTA.API/                         # Entry point, Minimal API endpoints, Program.cs
│
│   ── Módulos de negocio ──
│
├── PlanTA.Inventario.Domain/           # Stock, Almacenes, Ubicaciones, Lotes
├── PlanTA.Inventario.Application/
├── PlanTA.Inventario.Infrastructure/
│
├── PlanTA.Produccion.Domain/           # Órdenes de Fabricación, BOM, Rutas
├── PlanTA.Produccion.Application/
├── PlanTA.Produccion.Infrastructure/
│
├── PlanTA.Compras.Domain/              # Proveedores, Órdenes de Compra, Recepciones
├── PlanTA.Compras.Application/
├── PlanTA.Compras.Infrastructure/
│
├── PlanTA.Ventas.Domain/               # Clientes, Pedidos, Expediciones
├── PlanTA.Ventas.Application/
├── PlanTA.Ventas.Infrastructure/
│
├── PlanTA.Calidad.Domain/              # Inspecciones, No Conformidades, Criterios
├── PlanTA.Calidad.Application/
├── PlanTA.Calidad.Infrastructure/
│
├── PlanTA.Seguridad.Domain/            # Usuarios, Roles, Permisos, Auditoría
├── PlanTA.Seguridad.Application/
├── PlanTA.Seguridad.Infrastructure/
│
tests/
├── PlanTA.SharedKernel.Tests/
├── PlanTA.Inventario.Domain.Tests/
├── PlanTA.Inventario.Application.Tests/
├── PlanTA.Produccion.Domain.Tests/
├── PlanTA.Produccion.Application.Tests/
├── PlanTA.Compras.Domain.Tests/
├── PlanTA.Ventas.Domain.Tests/
├── PlanTA.Calidad.Domain.Tests/
├── PlanTA.Seguridad.Domain.Tests/
└── PlanTA.API.IntegrationTests/
```

### 3.2 Regla de dependencias (NUNCA invertir)

```
API → Application → Domain → SharedKernel
Infrastructure → Application → Domain → SharedKernel
```

- `SharedKernel` no referencia ningún módulo
- `Domain` no referencia `Application`, `Infrastructure` ni otros módulos
- `Application` no referencia `Infrastructure` ni otros módulos de dominio
- **Ningún módulo referencia a otro módulo** — la comunicación es solo por DomainEvents

### 3.3 SharedKernel — Primitivas comunes

```csharp
// ── Strongly Typed IDs ──
public abstract record EntityId(Guid Value)
{
    public override string ToString() => Value.ToString();
}

// Ejemplo por módulo:
public record ProductoId(Guid Value) : EntityId(Value);
public record AlmacenId(Guid Value) : EntityId(Value);
public record OrdenFabricacionId(Guid Value) : EntityId(Value);

// ── Entidades base ──
public abstract class BaseEntity<TId> where TId : EntityId
{
    public TId Id { get; protected set; } = default!;
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; protected set; }
    public string? CreatedByUserId { get; protected set; }
}

public abstract class AggregateRoot<TId> : BaseEntity<TId> where TId : EntityId
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    // Concurrencia optimista
    public uint Version { get; protected set; }
}

// ── Value Objects ──
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetAtomicValues();

    public override bool Equals(object? obj) { ... }
    public override int GetHashCode() { ... }
}

// ── Soft Delete ──
public interface IHasSoftDelete
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    void SoftDelete();
    void Restore();
}

// ── Result<T> ──
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public Error? Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(Error error) => new(false, default, error);
    public static Result<T> Failure(string code, string message) =>
        new(false, default, Error.Validation(code, message));
}

public record Error(string Code, string Message, ErrorType Type)
{
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);
}

public enum ErrorType { Validation, NotFound, Conflict, Forbidden, Internal }

// ── CQRS interfaces ──
public interface ICommand<T> : IRequest<Result<T>>;        // Transaccional
public interface IQuery<T> : IRequest<Result<T>>;           // Solo lectura
public interface IDomainEvent : INotification;

// ── MediatR Behaviors (orden de ejecución) ──
// 1. LoggingBehavior       → log entrada/salida
// 2. ValidationBehavior    → FluentValidation automático
// 3. TransactionBehavior   → solo para ICommand<T>
// 4. PerformanceBehavior   → warning si > 500ms
```

---

## 4. Módulos de Negocio — Diseño de Dominio

### 4.1 Módulo Inventario (`schema: inventario`)

> Core del sistema. Todo el flujo de stock pasa por aquí.

#### Entidades

| Entidad | Tipo | Descripción |
|---|---|---|
| `Producto` | AggregateRoot | Artículo con SKU, nombre, categoría, unidad de medida |
| `CategoriaProducto` | Entity | Jerarquía de categorías (Materia Prima, Producto Terminado, Componente) |
| `Almacen` | AggregateRoot | Almacén físico con ubicaciones |
| `Ubicacion` | Entity | Estantería/zona dentro de un almacén (pasillo-estante-nivel) |
| `StockUbicacion` | Entity | Cantidad de un producto en una ubicación concreta |
| `Lote` | AggregateRoot | Lote de fabricación/recepción con trazabilidad |
| `NumeroSerie` | Entity | Número de serie individual dentro de un lote |
| `MovimientoStock` | AggregateRoot | Registro de cada movimiento (entrada, salida, ajuste, transferencia) |
| `AlertaStock` | Entity | Configuración de stock mínimo/máximo por producto-almacén |

#### Value Objects

```csharp
public record SKU(string Value) : ValueObject;             // "MAT-ACERO-001"
public record UnidadMedida(string Codigo, string Nombre);  // "KG", "UND", "L", "M"
public record CodigoUbicacion(string Pasillo, string Estante, string Nivel); // "A-03-2"
public record CodigoLote(string Value);                     // "LOT-2026-04-001"
```

#### Enums

```csharp
public enum TipoProducto { MateriaPrima, Componente, Semielaborado, ProductoTerminado, Consumible }
public enum TipoMovimiento { Entrada, Salida, Ajuste, TransferenciaInterna, Merma, Devolucion }
public enum EstadoLote { Activo, EnCuarentena, Bloqueado, Agotado, Caducado }
```

#### Domain Events

```csharp
StockBajoMinimoEvent(ProductoId, AlmacenId, decimal CantidadActual, decimal Minimo)
StockActualizadoEvent(ProductoId, AlmacenId, UbicacionId, decimal CantidadAnterior, decimal CantidadNueva)
LoteCreadoEvent(LoteId, ProductoId, decimal Cantidad)
LoteBloqueadoEvent(LoteId, string Motivo)
ProductoCreadoEvent(ProductoId, string SKU)
```

#### Tablas PostgreSQL (`schema: inventario`)

```sql
inventario.productos
inventario.categorias_producto
inventario.almacenes
inventario.ubicaciones
inventario.stock_ubicacion          -- cantidad por producto-ubicación
inventario.lotes
inventario.numeros_serie
inventario.movimientos_stock        -- log inmutable de cada movimiento
inventario.alertas_stock            -- config de min/max por producto-almacén
```

---

### 4.2 Módulo Producción (`schema: produccion`)

> Órdenes de fabricación, listas de materiales (BOM) y rutas de producción.

#### Entidades

| Entidad | Tipo | Descripción |
|---|---|---|
| `ListaMateriales` (BOM) | AggregateRoot | Receta para fabricar un producto terminado |
| `LineaBOM` | Entity | Componente + cantidad necesaria dentro de una BOM |
| `RutaProduccion` | AggregateRoot | Secuencia de operaciones para fabricar un producto |
| `OperacionRuta` | Entity | Paso individual: máquina, tiempo estimado, instrucciones |
| `OrdenFabricacion` (OF) | AggregateRoot | Orden concreta de producir X unidades de un producto |
| `LineaConsumoOF` | Entity | Materiales consumidos en la OF (contra stock) |
| `ParteProduccion` | Entity | Registro de producción real: unidades buenas, defectuosas, merma |

#### Value Objects

```csharp
public record CodigoOF(string Value);                       // "OF-2026-04-0023"
public record TiempoEstimado(decimal Minutos);
public record CantidadPlanificada(decimal Cantidad, string UnidadMedida);
```

#### Enums

```csharp
public enum EstadoOF { Planificada, EnPreparacion, EnCurso, Pausada, Completada, Cancelada }
public enum TipoOperacion { Corte, Soldadura, Ensamblaje, Pintura, Embalaje, Inspeccion, Otro }
```

#### Domain Events

```csharp
OFCreadaEvent(OrdenFabricacionId, ProductoId, decimal Cantidad)
OFIniciadaEvent(OrdenFabricacionId)
OFCompletadaEvent(OrdenFabricacionId, decimal UnidadesBuenas, decimal Defectuosas)
OFCanceladaEvent(OrdenFabricacionId, string Motivo)
MaterialConsumidoEvent(OrdenFabricacionId, ProductoId, LoteId, decimal Cantidad)
ProduccionRegistradaEvent(OrdenFabricacionId, decimal Cantidad, LoteId LoteGenerado)
```

#### Tablas PostgreSQL (`schema: produccion`)

```sql
produccion.listas_materiales         -- BOM headers
produccion.lineas_bom                -- componentes de cada BOM
produccion.rutas_produccion
produccion.operaciones_ruta
produccion.ordenes_fabricacion
produccion.lineas_consumo_of         -- materiales consumidos
produccion.partes_produccion         -- output registrado
```

#### Flujo de una Orden de Fabricación

```
1. Crear OF (estado: Planificada)
   → Verificar BOM existe para el producto
   → Verificar stock suficiente de componentes

2. Preparar OF (estado: EnPreparacion)
   → Reservar materiales (MovimientoStock tipo: Reserva)
   → Asignar ruta de producción

3. Iniciar OF (estado: EnCurso)
   → Consumir materiales del stock (MovimientoStock tipo: Salida)
   → publish MaterialConsumidoEvent → Inventario actualiza stock

4. Registrar producción (ParteProduccion)
   → Unidades buenas → MovimientoStock tipo: Entrada (producto terminado)
   → Unidades defectuosas → Merma o reproceso
   → publish ProduccionRegistradaEvent → Inventario recibe stock

5. Completar OF (estado: Completada)
   → Calcular eficiencia (buenas / planificadas * 100)
   → Calcular coste real (materiales + tiempo)
   → publish OFCompletadaEvent → Calidad puede inspeccionar
```

---

### 4.3 Módulo Compras (`schema: compras`)

> Proveedores, órdenes de compra y recepciones de material.

#### Entidades

| Entidad | Tipo | Descripción |
|---|---|---|
| `Proveedor` | AggregateRoot | Datos del proveedor, contacto, condiciones de pago |
| `ContactoProveedor` | Entity | Personas de contacto dentro del proveedor |
| `OrdenCompra` (OC) | AggregateRoot | Pedido al proveedor |
| `LineaOrdenCompra` | Entity | Producto, cantidad, precio unitario |
| `Recepcion` | AggregateRoot | Recepción de mercancía contra una OC |
| `LineaRecepcion` | Entity | Cantidad recibida, lote asignado, ubicación destino |
| `CondicionesPago` | ValueObject | Días de pago, descuento pronto pago |

#### Enums

```csharp
public enum EstadoOC { Borrador, Enviada, ParcialmenteRecibida, Recibida, Cancelada }
public enum EstadoRecepcion { Pendiente, EnInspeccion, Aceptada, Rechazada }
```

#### Domain Events

```csharp
OCEnviadaEvent(OrdenCompraId, ProveedorId)
RecepcionRegistradaEvent(RecepcionId, OrdenCompraId, List<LineaRecepcionDto>)
RecepcionAceptadaEvent(RecepcionId) // → Inventario genera MovimientoStock entrada + Lotes
OCCompletadaEvent(OrdenCompraId)
```

#### Tablas PostgreSQL (`schema: compras`)

```sql
compras.proveedores
compras.contactos_proveedor
compras.ordenes_compra
compras.lineas_orden_compra
compras.recepciones
compras.lineas_recepcion
```

---

### 4.4 Módulo Ventas (`schema: ventas`)

> Clientes, pedidos y expediciones.

#### Entidades

| Entidad | Tipo | Descripción |
|---|---|---|
| `Cliente` | AggregateRoot | Datos del cliente, dirección de envío/facturación |
| `ContactoCliente` | Entity | Personas de contacto |
| `Pedido` | AggregateRoot | Pedido de venta |
| `LineaPedido` | Entity | Producto, cantidad, precio, descuento |
| `Expedicion` | AggregateRoot | Envío de mercancía contra un pedido |
| `LineaExpedicion` | Entity | Producto, cantidad, lote de origen |

#### Enums

```csharp
public enum EstadoPedido { Borrador, Confirmado, EnPreparacion, ParcialmenteEnviado, Enviado, Entregado, Cancelado }
public enum EstadoExpedicion { Pendiente, EnPicking, Empaquetada, Enviada, Entregada }
```

#### Domain Events

```csharp
PedidoConfirmadoEvent(PedidoId, ClienteId)
ExpedicionPreparadaEvent(ExpedicionId, PedidoId)
ExpedicionEnviadaEvent(ExpedicionId) // → Inventario genera MovimientoStock salida
PedidoEntregadoEvent(PedidoId)
```

#### Tablas PostgreSQL (`schema: ventas`)

```sql
ventas.clientes
ventas.contactos_cliente
ventas.pedidos
ventas.lineas_pedido
ventas.expediciones
ventas.lineas_expedicion
```

---

### 4.5 Módulo Calidad (`schema: calidad`)

> Control de calidad, inspecciones y no conformidades.

#### Entidades

| Entidad | Tipo | Descripción |
|---|---|---|
| `PlantillaInspeccion` | AggregateRoot | Template de inspección reutilizable |
| `CriterioInspeccion` | Entity | Check individual: medida, rango aceptable, tipo |
| `Inspeccion` | AggregateRoot | Inspección ejecutada sobre un lote, recepción u OF |
| `ResultadoCriterio` | Entity | Resultado de cada criterio en la inspección |
| `NoConformidad` | AggregateRoot | Registro de no conformidad con acciones correctivas |
| `AccionCorrectiva` | Entity | Acción tomada para resolver la no conformidad |

#### Enums

```csharp
public enum OrigenInspeccion { Recepcion, Produccion, Expedicion, Inventario }
public enum ResultadoInspeccion { Aprobada, Rechazada, AprobadaConObservaciones }
public enum EstadoNoConformidad { Abierta, EnInvestigacion, AccionDefinida, Resuelta, Cerrada }
public enum SeveridadNC { Menor, Mayor, Critica }
```

#### Domain Events

```csharp
InspeccionCompletadaEvent(InspeccionId, ResultadoInspeccion)
InspeccionRechazadaEvent(InspeccionId, LoteId) // → Inventario bloquea lote
NoConformidadAbiertaEvent(NoConformidadId, OrigenInspeccion)
NoConformidadResueltaEvent(NoConformidadId)
```

#### Tablas PostgreSQL (`schema: calidad`)

```sql
calidad.plantillas_inspeccion
calidad.criterios_inspeccion
calidad.inspecciones
calidad.resultados_criterio
calidad.no_conformidades
calidad.acciones_correctivas
```

---

### 4.6 Módulo Seguridad (`schema: seguridad`)

> Usuarios, roles, permisos y auditoría.

#### Entidades

| Entidad | Tipo | Descripción |
|---|---|---|
| `Usuario` | AggregateRoot | Extiende Identity, con datos de empresa |
| `Empresa` | AggregateRoot | Tenant — cada empresa tiene sus datos aislados |
| `ConfiguracionEmpresa` | Entity | Settings por empresa (moneda, zona horaria, logo) |
| `LogAuditoria` | Entity | Registro inmutable de acciones (quién, qué, cuándo) |

#### Roles del sistema

| Rol | Acceso |
|---|---|
| `Administrador` | Todo el sistema + configuración de empresa |
| `GerentePlanta` | Dashboard, KPIs, aprobaciones, todos los módulos |
| `JefeAlmacen` | Inventario, recepciones, expediciones, ubicaciones |
| `JefeProduccion` | Producción, BOM, órdenes de fabricación |
| `Compras` | Proveedores, órdenes de compra, recepciones |
| `Ventas` | Clientes, pedidos, expediciones |
| `Operario` | Movimientos de stock, fichaje en OF, escaneo lotes |
| `Calidad` | Inspecciones, no conformidades |

#### Tablas PostgreSQL (`schema: seguridad`)

```sql
seguridad.empresas
seguridad.configuraciones_empresa
seguridad.asp_net_users              -- Identity tables con schema propio
seguridad.asp_net_roles
seguridad.asp_net_user_roles
seguridad.log_auditoria
```

---

## 5. Comunicación Entre Módulos — Solo DomainEvents

```
REGLA FUNDAMENTAL: ningún módulo importa ni referencia a otro módulo.
La única vía de comunicación es DomainEvents publicados via MediatR.
```

### 5.1 Flujo completo: Compra → Inventario → Producción → Venta

```
── COMPRA ──
Compras crea OC → envía al proveedor
  → Proveedor entrega → Recepcion registrada
    → publish RecepcionAceptadaEvent
      → Handler en Inventario:
        → Crear Lote con trazabilidad
        → MovimientoStock tipo: Entrada
        → Actualizar StockUbicacion
        → Si stock < mínimo resuelto: cerrar AlertaStock

── PRODUCCIÓN ──
Producción crea OF para fabricar producto terminado
  → Consulta BOM → necesita componentes del Inventario
  → Inicia OF → Consume materiales
    → publish MaterialConsumidoEvent
      → Handler en Inventario:
        → MovimientoStock tipo: Salida
        → Actualizar StockUbicacion
        → Si stock < mínimo: publish StockBajoMinimoEvent
          → Handler en Compras: crear OC automática (si auto-reorden activo)
  → Completa OF → Registra producción
    → publish ProduccionRegistradaEvent
      → Handler en Inventario:
        → Crear Lote de producto terminado
        → MovimientoStock tipo: Entrada
      → Handler en Calidad:
        → Crear Inspección automática (si plantilla configurada)

── VENTA ──
Ventas confirma Pedido → genera Expedición
  → Picking: seleccionar lotes (FIFO por defecto)
    → publish ExpedicionEnviadaEvent
      → Handler en Inventario:
        → MovimientoStock tipo: Salida
        → Actualizar StockUbicacion
```

### 5.2 Eventos cross-módulo (via Outbox + Hangfire)

```csharp
// Para side-effects que requieren consistencia eventual:
// tabla: shared.outbox_messages
// Hangfire los procesa con retry y exponential backoff

// Ejemplo: RecepcionAceptadaEvent
// 1. Compras.Infrastructure guarda evento en outbox dentro de la misma transacción
// 2. Hangfire lee outbox → publica via MediatR
// 3. Inventario.Application handler procesa el evento
// 4. Si falla → retry con backoff (1s, 5s, 30s, 5min)
```

---

## 6. PostgreSQL — Schemas por Módulo

```sql
-- Schemas
inventario       → productos, almacenes, ubicaciones, stock, lotes, movimientos, alertas
produccion       → bom, rutas, ordenes_fabricacion, consumos, partes
compras          → proveedores, ordenes_compra, recepciones
ventas           → clientes, pedidos, expediciones
calidad          → plantillas, inspecciones, no_conformidades
seguridad        → empresas, usuarios (Identity), roles, auditoria
shared           → outbox_messages, __EFMigrationsHistory
```

### 6.1 DbContext — uno por módulo

```csharp
// Cada módulo tiene su propio DbContext
public class InventarioDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("inventario");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Soft delete global filter
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(IHasSoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateSoftDeleteFilter(entityType.ClrType));
            }
        }
    }
}
```

### 6.2 Migraciones — por módulo

```bash
# Cada módulo tiene sus propias migraciones
dotnet ef migrations add InitialCreate \
  --project src/PlanTA.Inventario.Infrastructure \
  --startup-project src/PlanTA.API

dotnet ef migrations add InitialCreate \
  --project src/PlanTA.Produccion.Infrastructure \
  --startup-project src/PlanTA.API
```

---

## 7. API — Minimal API con IEndpointGroup

### 7.1 Convención de endpoints

```csharp
// NO controllers — usamos Minimal API
// Cada módulo define sus endpoints en IEndpointGroup

public sealed class ProductosEndpoints : IEndpointGroup
{
    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetProductosQuery(), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProductos")
        .WithTags("Inventario - Productos")
        .RequireAuthorization()
        .Produces<List<ProductoDto>>(200)
        .WithOpenApi();

        group.MapPost("/", async (CrearProductoRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(req.ToCommand(), ct);
            return result.ToHttpResult(201);
        })
        .WithName("CrearProducto")
        .RequireAuthorization("JefeAlmacen")
        .Produces<Guid>(201)
        .ProducesProblem(400)
        .WithOpenApi();
    }
}
```

### 7.2 Estructura de URLs

```
/api/v1/inventario/productos
/api/v1/inventario/productos/{id}
/api/v1/inventario/almacenes
/api/v1/inventario/almacenes/{id}/ubicaciones
/api/v1/inventario/movimientos
/api/v1/inventario/lotes
/api/v1/inventario/lotes/{id}/trazabilidad
/api/v1/inventario/alertas

/api/v1/produccion/bom
/api/v1/produccion/bom/{id}/lineas
/api/v1/produccion/rutas
/api/v1/produccion/ordenes
/api/v1/produccion/ordenes/{id}/consumos
/api/v1/produccion/ordenes/{id}/partes

/api/v1/compras/proveedores
/api/v1/compras/ordenes
/api/v1/compras/ordenes/{id}/lineas
/api/v1/compras/recepciones

/api/v1/ventas/clientes
/api/v1/ventas/pedidos
/api/v1/ventas/pedidos/{id}/lineas
/api/v1/ventas/expediciones

/api/v1/calidad/plantillas
/api/v1/calidad/inspecciones
/api/v1/calidad/no-conformidades

/api/v1/seguridad/auth/login
/api/v1/seguridad/auth/refresh
/api/v1/seguridad/usuarios
/api/v1/seguridad/empresas
/api/v1/seguridad/auditoria
```

---

## 8. Frontend — Angular 21

### 8.1 Estructura

```
planta-web/src/app/
├── core/
│   ├── guards/             # authGuard, roleGuard, guestGuard
│   ├── interceptors/       # tokenInterceptor, errorInterceptor
│   ├── models/             # User, AuthResponse, interfaces globales
│   └── services/           # AuthService, NotificationService
│
├── shared/
│   ├── components/         # Tabla genérica, KPI card, estado badge, confirm dialog
│   ├── directives/         # scroll-animate, click-outside
│   ├── pipes/              # currency-es, date-relative, unit-format
│   └── styles/             # _variables.scss, _mixins.scss, _layouts.scss
│
├── features/
│   ├── landing/            # Landing page pública (reutilizada de FixFlow)
│   ├── auth/               # Login, Register, Forgot Password (reutilizado)
│   │
│   ├── dashboard/          # KPIs globales, alertas, resumen por módulo
│   │
│   ├── inventario/
│   │   ├── productos/      # CRUD productos, categorías
│   │   ├── almacenes/      # Gestión almacenes + ubicaciones
│   │   ├── stock/          # Vista de stock por producto/almacén/ubicación
│   │   ├── movimientos/    # Log de movimientos con filtros
│   │   ├── lotes/          # Trazabilidad de lotes
│   │   └── alertas/        # Configuración de alertas de stock
│   │
│   ├── produccion/
│   │   ├── bom/            # Listas de materiales (árbol de componentes)
│   │   ├── rutas/          # Rutas de producción (pasos secuenciales)
│   │   ├── ordenes/        # Órdenes de fabricación (kanban + lista)
│   │   └── partes/         # Registro de producción
│   │
│   ├── compras/
│   │   ├── proveedores/    # CRUD proveedores
│   │   ├── ordenes/        # Órdenes de compra
│   │   └── recepciones/    # Registro de recepciones
│   │
│   ├── ventas/
│   │   ├── clientes/       # CRUD clientes
│   │   ├── pedidos/        # Pedidos de venta
│   │   └── expediciones/   # Preparación y envío
│   │
│   ├── calidad/
│   │   ├── plantillas/     # Templates de inspección
│   │   ├── inspecciones/   # Ejecutar/ver inspecciones
│   │   └── no-conformidades/ # NCRs y acciones correctivas
│   │
│   └── configuracion/
│       ├── empresa/        # Datos de empresa, logo, moneda
│       ├── usuarios/       # Gestión de usuarios y roles
│       └── auditoria/      # Log de auditoría
│
├── layouts/
│   ├── app-shell/          # Shell con sidebar (reutilizado de FixFlow)
│   └── public-layout/      # Layout para landing/auth
│
└── app.routes.ts
```

### 8.2 Rutas

```typescript
// Públicas
/                           → LandingComponent
/login                      → LoginComponent (guestGuard)
/register                   → RegisterComponent (guestGuard)
/forgot-password            → ForgotPasswordComponent (guestGuard)

// App (authGuard + roleGuard)
/app                        → AppShellComponent
  /app/dashboard            → DashboardComponent (todos los roles)

  // Inventario
  /app/inventario/productos         → ProductosListComponent
  /app/inventario/productos/nuevo   → ProductoFormComponent
  /app/inventario/productos/:id     → ProductoDetailComponent
  /app/inventario/almacenes         → AlmacenesListComponent
  /app/inventario/almacenes/:id     → AlmacenDetailComponent (con ubicaciones)
  /app/inventario/stock             → StockOverviewComponent
  /app/inventario/movimientos       → MovimientosLogComponent
  /app/inventario/lotes             → LotesListComponent
  /app/inventario/lotes/:id         → LoteTrazabilidadComponent
  /app/inventario/alertas           → AlertasConfigComponent

  // Producción
  /app/produccion/bom               → BOMListComponent
  /app/produccion/bom/:id           → BOMDetailComponent (árbol)
  /app/produccion/rutas             → RutasListComponent
  /app/produccion/ordenes           → OrdenesListComponent (kanban + tabla)
  /app/produccion/ordenes/:id       → OrdenDetailComponent
  /app/produccion/ordenes/:id/parte → RegistroProduccionComponent

  // Compras
  /app/compras/proveedores          → ProveedoresListComponent
  /app/compras/proveedores/:id      → ProveedorDetailComponent
  /app/compras/ordenes              → OCListComponent
  /app/compras/ordenes/:id          → OCDetailComponent
  /app/compras/recepciones          → RecepcionesListComponent
  /app/compras/recepciones/nueva    → NuevaRecepcionComponent

  // Ventas
  /app/ventas/clientes              → ClientesListComponent
  /app/ventas/clientes/:id          → ClienteDetailComponent
  /app/ventas/pedidos               → PedidosListComponent
  /app/ventas/pedidos/:id           → PedidoDetailComponent
  /app/ventas/expediciones          → ExpedicionesListComponent

  // Calidad
  /app/calidad/plantillas           → PlantillasListComponent
  /app/calidad/inspecciones         → InspeccionesListComponent
  /app/calidad/inspecciones/:id     → InspeccionDetailComponent
  /app/calidad/no-conformidades     → NCListComponent
  /app/calidad/no-conformidades/:id → NCDetailComponent

  // Configuración
  /app/config/empresa               → EmpresaConfigComponent
  /app/config/usuarios              → UsuariosListComponent
  /app/config/auditoria             → AuditoriaLogComponent
```

### 8.3 Guards

| Guard | Comportamiento |
|---|---|
| `authGuard` | Redirige a `/login` si no autenticado |
| `guestGuard` | Redirige a `/app/dashboard` si ya autenticado |
| `roleGuard` | Verifica que el usuario tiene el rol requerido en `data.roles` |

### 8.4 Redirección inicial por rol

| Rol | Ruta inicial |
|---|---|
| `Administrador` | `/app/dashboard` |
| `GerentePlanta` | `/app/dashboard` |
| `JefeAlmacen` | `/app/inventario/stock` |
| `JefeProduccion` | `/app/produccion/ordenes` |
| `Compras` | `/app/compras/ordenes` |
| `Ventas` | `/app/ventas/pedidos` |
| `Operario` | `/app/inventario/movimientos` |
| `Calidad` | `/app/calidad/inspecciones` |

### 8.5 Patrones Frontend obligatorios

```typescript
// ── Standalone + OnPush siempre ──
@Component({
  selector: 'app-productos-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink, ...],
  templateUrl: './productos-list.component.html'
})

// ── Signals para estado ──
readonly productos = signal<ProductoDto[]>([]);
readonly isLoading = signal(false);
readonly filtro = signal<string>('');
readonly productosFiltrados = computed(() =>
  this.productos().filter(p =>
    p.nombre.toLowerCase().includes(this.filtro().toLowerCase())
  )
);

// ── Lazy loading obligatorio ──
{
  path: 'inventario',
  loadChildren: () => import('./features/inventario/inventario.routes')
    .then(m => m.INVENTARIO_ROUTES)
}

// ── Mock data como constante de módulo ──
const PRODUCTOS_MOCK: ProductoDto[] = [ ... ];

@Component({ ... })
export class ProductosListComponent {
  readonly productos = signal<ProductoDto[]>(PRODUCTOS_MOCK);
}
```

---

## 9. Autenticación — JWT + Refresh Token

### 9.1 Flujo

```
1. POST /api/v1/seguridad/auth/login { email, password }
   → 200 { accessToken, refreshToken, user }

2. Frontend guarda en localStorage:
   'planta_access_token'    // JWT corta duración (15 min)
   'planta_refresh_token'   // Opaque larga duración (7 días)
   'planta_user'            // UserDto JSON

3. tokenInterceptor añade Bearer token a todas las requests /api

4. Si 401 → interceptor llama POST /api/v1/seguridad/auth/refresh
   → Si refresh OK → reintentar request original
   → Si refresh falla → logout + redirigir a /login
```

### 9.2 Multi-tenant

```csharp
// Cada usuario pertenece a una empresa (EmpresaId en el JWT claim)
// Todos los queries filtran por EmpresaId automáticamente
// El middleware extrae EmpresaId del token y lo inyecta como ICurrentTenant
public interface ICurrentTenant
{
    Guid EmpresaId { get; }
    Guid UserId { get; }
    string Role { get; }
}
```

---

## 10. Código Reutilizado de FixFlow

El proyecto hereda código del frontend FixFlow (Angular 21) que ya está implementado:

### Reutilizable directamente (renombrar prefijos fixflow_ → planta_)

| Componente | Estado | Cambios necesarios |
|---|---|---|
| `core/services/auth.service.ts` | Mock con signals | Renombrar keys localStorage, adaptar roles |
| `core/interceptors/token.interceptor.ts` | Funcional | Sin cambios |
| `core/guards/auth.guard.ts` | Funcional | Sin cambios |
| `core/guards/guest.guard.ts` | Funcional | Sin cambios |
| `core/models/user.model.ts` | Interfaces básicas | Ampliar con EmpresaId, roles PlanTA |
| `app.config.ts` | Providers configurados | Sin cambios |
| `pages/app/app-shell/` | Sidebar responsive | Cambiar navItems a módulos PlanTA |
| `pages/auth/login/` | Login con signals | Cambiar branding |
| `pages/auth/register/` | Register con signals | Añadir campo empresa |
| `pages/auth/forgot-password/` | Forgot password | Cambiar branding |
| `pages/landing/` | Landing marketing | Reescribir copy para PlanTA |
| `shared/directives/scroll-animate` | Directiva de scroll | Sin cambios |
| `shared/styles/` | Variables SCSS | Adaptar paleta de colores |

### Backend — estructura de carpetas

La estructura de carpetas del backend (`AcademiaOposiciones/`) se renombrará a `PlanTA/` con los namespaces correspondientes. Solo `BaseEntity.cs` tiene código, que será reemplazado por el SharedKernel completo.

---

## 11. Roadmap de Implementación

### Fase 0 — Fundación (Sprint 1-2)
- [ ] Renombrar proyecto: `mini-SAP` → estructura `PlanTA`
- [ ] Crear `PlanTA.SharedKernel` con todas las primitivas
- [ ] Crear `PlanTA.API` con Minimal API, auth, Swagger
- [ ] Crear `PlanTA.Seguridad` (Identity, JWT, roles, empresa)
- [ ] Frontend: renombrar FixFlow → PlanTA, adaptar branding
- [ ] Frontend: app-shell con navegación por módulos
- [ ] Docker: `docker-compose.yml` con PostgreSQL + Seq
- [ ] CI: GitHub Actions básico (build + test)

### Fase 1 — Inventario Core (Sprint 3-5)
- [ ] `PlanTA.Inventario` completo: Productos, Almacenes, Ubicaciones
- [ ] CRUD de productos con categorías y SKU
- [ ] Gestión de almacenes y ubicaciones (código pasillo-estante-nivel)
- [ ] Movimientos de stock (entrada, salida, ajuste, transferencia)
- [ ] Lotes con trazabilidad
- [ ] Alertas de stock mínimo/máximo
- [ ] Importación masiva desde CSV/Excel
- [ ] Frontend: vistas de inventario, stock overview, log de movimientos

### Fase 2 — Compras + Ventas (Sprint 6-8)
- [ ] `PlanTA.Compras`: Proveedores, OC, Recepciones
- [ ] `PlanTA.Ventas`: Clientes, Pedidos, Expediciones
- [ ] Integración: Recepción → genera entrada en Inventario (DomainEvent)
- [ ] Integración: Expedición → genera salida en Inventario (DomainEvent)
- [ ] Auto-reorden: StockBajoMinimoEvent → sugerir/crear OC
- [ ] Frontend: vistas de compras y ventas

### Fase 3 — Producción (Sprint 9-12)
- [ ] `PlanTA.Produccion`: BOM, Rutas, Órdenes de Fabricación
- [ ] Flujo completo OF: Planificada → EnCurso → Completada
- [ ] Consumo de materiales contra stock
- [ ] Registro de producción (partes, buenas/defectuosas)
- [ ] Cálculo de costes de producción
- [ ] Frontend: BOM como árbol, OF como kanban + tabla

### Fase 4 — Calidad + Analítica (Sprint 13-15)
- [ ] `PlanTA.Calidad`: Plantillas, Inspecciones, No Conformidades
- [ ] Inspección automática en recepción y producción
- [ ] Dashboard analítico: rotación de stock, eficiencia producción, costes
- [ ] Exportación de informes (PDF/Excel)
- [ ] Previsión de demanda simple (media móvil)

### Fase 5 — Automatizaciones + Polish (Sprint 16+)
- [ ] Importación masiva Excel/CSV con preview y validación
- [ ] Notificaciones email/WhatsApp (alertas stock, OF completada)
- [ ] Escaneo de códigos de barras/QR (preparado para mobile)
- [ ] Multi-tenant completo (aislamiento por empresa)
- [ ] Documentación de usuario

---

## 12. Variables de Entorno

### Local (no commitear)

```json
// src/PlanTA.API/appsettings.Development.json (en .gitignore)
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=planta_dev;Username=planta_user;Password=planta_dev"
  },
  "Jwt": {
    "Key": "dev-key-min-32-chars-for-hmac-sha256!!",
    "Issuer": "planta-api",
    "Audience": "planta-app",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Hangfire": { "DashboardEnabled": true },
  "Seq": { "ServerUrl": "http://localhost:5341" }
}
```

### Producción (Render env vars)

```
ConnectionStrings__DefaultConnection  → Neon connection string
Jwt__Key                              → Secret de 32+ chars
Frontend__Url                         → URL de Vercel
ASPNETCORE_ENVIRONMENT                → Production
```

---

## 13. Convenciones de Código

### Backend

```
Commands:      CrearProductoCommand          (verbo español + entidad + Command)
Queries:       GetProductosQuery             (Get + entidad plural + Query)
Handlers:      CrearProductoCommandHandler   (mismo nombre + Handler)
DTOs:          ProductoDto, ProductoDetailDto (entidad + [Detail/Resumen/Lista] + Dto)
Interfaces:    IProductoRepository           (I + nombre)
Errors:        ProductoErrors                (entidad + Errors)
IDs:           ProductoId(Guid Value)        (entidad + Id, record : EntityId)
Endpoints:     ProductosEndpoints            (entidad plural + Endpoints)
DbContext:     InventarioDbContext            (módulo + DbContext)
```

### Frontend

```
Componentes:   app-productos-list            (prefijo app-)
Servicios:     InventarioService, AuthService
State signals: readonly productos = signal<ProductoDto[]>([])
Routes:        INVENTARIO_ROUTES             (SCREAMING_SNAKE_CASE)
Archivos:      productos-list.component.ts   (kebab-case)
```

### Nombres de dominio en español

Producto, Almacen, OrdenFabricacion, Proveedor, Pedido, Lote, Inspeccion — las entidades de negocio van en español. Infraestructura y patrones técnicos en inglés.

---

## 14. Checklist antes de commitear

- [ ] Sin secrets en código
- [ ] Los endpoints usan `result.ToHttpResult()`, no lógica de status manual
- [ ] Los handlers no lanzan excepciones para errores de negocio
- [ ] Las entidades tienen Strongly Typed IDs
- [ ] Los nuevos componentes Angular son `standalone: true` + `OnPush`
- [ ] Las nuevas rutas usan `loadComponent` / `loadChildren`
- [ ] Los mock data son constantes de módulo, no métodos de clase
- [ ] `appsettings.Development.json` en `.gitignore`
- [ ] Ningún módulo importa directamente a otro módulo (solo SharedKernel)
- [ ] Nuevos DbContext usan `builder.HasDefaultSchema("nombre_modulo")`
- [ ] Soft delete en todas las entidades principales
- [ ] Multi-tenant: queries filtran por EmpresaId
