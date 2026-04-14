# PlanTA — Documentación Funcional

ERP/MES modular para PYMEs industriales que sustituye a Excel + WhatsApp + papel. Construido con .NET 9 (Clean Architecture + CQRS) y Angular 19 (standalone + signals). Multi-tenant por `EmpresaId`, multi-módulo con schemas PostgreSQL separados, desplegado en Render + Vercel + Neon.

---

## 1. Arquitectura general

### Stack

| Capa | Tecnología |
|---|---|
| Backend | .NET 9, Minimal API, MediatR 12, FluentValidation, Serilog |
| ORM / DB | Entity Framework Core 9 + PostgreSQL 16 (Neon en prod, Docker en local puerto 5433) |
| Auth | ASP.NET Core Identity + JWT |
| Frontend | Angular 19 standalone components, signals, RxJS, lazy loading |
| Mensajería interna | Outbox Pattern + DomainEventInterceptor + BackgroundService |
| Logging | Serilog + Seq (local) |
| Deploy | Render (API), Vercel (frontend), Neon (PostgreSQL) |

### Capas (Clean Architecture)

```
src/
├── PlanTA.SharedKernel/                  # BaseEntity, AggregateRoot, Result<T>, Error,
│                                           ValueObject, ICurrentTenant, CQRS interfaces,
│                                           MediatR Behaviors, Integration Events
├── PlanTA.<Modulo>.Domain/                # Entidades, ValueObjects, Enums, Domain Events, Errors
├── PlanTA.<Modulo>.Application/           # CQRS (Commands, Queries, Handlers, Validators),
│                                           DTOs, Interfaces de DbContext
├── PlanTA.<Modulo>.Infrastructure/        # DbContext, EF Configurations, repositorios,
│                                           servicios externos
└── PlanTA.API/                            # Minimal API, IEndpointGroup auto-discovery,
                                            JWT, CORS, Swagger, Program.cs, OutboxProcessor
```

### Multi-tenancy

- Toda entidad de negocio hereda de `BaseEntity` con `EmpresaId` (`ICurrentTenant` lo resuelve del JWT).
- Los DbContext aplican filtros automáticos por `EmpresaId`.
- El JWT incluye `nombre`, `rol` y `empresaNombre` (mapeados al frontend como `name`, `role`, `company`).

### Patrón Result<T>

Todos los handlers devuelven `Result<T>` o `Result`. No se usan excepciones para lógica de negocio. Los endpoints las traducen a 200/400/404/409.

### Soft delete

Las entidades implementan `SoftDeletableEntity` con `IsDeleted` + `DeletedAt`. El DbContext aplica query filters globales.

---

## 2. Estructura de schemas PostgreSQL

Cada módulo tiene su propio schema en la misma base de datos:

| Schema | Módulo |
|---|---|
| `inventario` | Inventario Core |
| `produccion` | Producción / BOM / Órdenes Fabricación |
| `compras` | Compras / Proveedores / Recepciones |
| `ventas` | Ventas / Clientes / Expediciones |
| `calidad` | Calidad / Inspecciones / NCs |
| `activos` | Activos físicos |
| `mantenimiento` | Órdenes de mantenimiento |
| `incidencias` | Incidencias de planta |
| `crm` | CRM (oportunidades, pipeline) |
| `costes` | Centros de coste, imputación |
| `facturacion` | Facturas, líneas |
| `oee` | OEE / paradas / disponibilidad |
| `rrhh` | Empleados, partes de horas |
| `ia` | Integraciones LLM (Claude) |
| `importador` | Carga masiva (CSV/Excel) |
| `seguridad` | Identity (usuarios, roles, empresas) |
| `shared` | Outbox + Audit Log |

---

## 3. Módulos de negocio (15)

### 3.1 Inventario

**Entidades:** Producto, Familia, Almacén, UbicacionAlmacen, MovimientoInventario, Lote, Alerta.
**ValueObjects:** SKU, CantidadStock, NivelStock.
**Comandos clave:** CreateProducto, AjustarStock, MoverInventario, CrearLote, CerrarAlerta.
**Queries:** ListProductos (paged), ListMovimientos, GetAlertasActivas.
**Endpoints:** `/api/v1/inventario/productos`, `/almacenes`, `/movimientos`, `/lotes`, `/alertas`.

### 3.2 Producción

**Entidades:** ListaMateriales (BOM), LineaBOM, RutaProduccion, OperacionRuta, OrdenFabricacion, LineaConsumoOF, ParteProduccion.
**ValueObjects:** CodigoOF, TiempoEstimado, CantidadPlanificada.
**Flujos:** crear BOM → asociar ruta → lanzar OF → registrar partes de producción → cerrar OF.
**Comandos:** CreateBOM, CreateRuta, CreateOF, CambiarEstadoOF, RegistrarProduccion.
**Endpoints:** `/api/v1/produccion/boms`, `/rutas`, `/ordenes-fabricacion`.

### 3.3 Compras

**Entidades:** Proveedor, ContactoProveedor, OrdenCompra, LineaOrdenCompra, Recepcion, LineaRecepcion.
**ValueObjects:** CondicionesPago.
**Flujo:** alta proveedor → crear OC → enviar → recepcionar → integración con Inventario (entrada de stock vía Outbox).
**Endpoints:** `/api/v1/compras/proveedores`, `/ordenes-compra`, `/recepciones`.

### 3.4 Ventas

**Entidades:** Cliente, ContactoCliente, Pedido, LineaPedido, Expedicion, LineaExpedicion.
**Flujo:** alta cliente → crear pedido → confirmar → expedir → integración con Inventario (salida de stock).
**Endpoints:** `/api/v1/ventas/clientes`, `/pedidos`, `/expediciones`.

### 3.5 Calidad

**Entidades:** PlantillaInspeccion, CriterioInspeccion, Inspeccion, ResultadoCriterio, NoConformidad, AccionCorrectiva.
**Flujo:** definir plantilla → ejecutar inspección sobre OF/recepción → si falla, abrir NC → asignar acciones correctivas → cerrar.
**Endpoints:** `/api/v1/calidad/plantillas`, `/inspecciones`, `/no-conformidades`.

### 3.6 Activos

**Entidades:** Activo, CategoriaActivo, HistorialActivo.
**Funcionalidad:** inventario de máquinas/equipos/instalaciones, ubicación, estado (activo/baja/en mantenimiento), criticidad. Base para Mantenimiento y OEE.
**Endpoints:** `/api/v1/activos`.

### 3.7 Mantenimiento

**Entidades:** OrdenMantenimiento, TipoMantenimiento (preventivo/correctivo/predictivo), PlanMantenimiento.
**Flujo:** planes preventivos generan OTs automáticas → técnico ejecuta → reporte de horas y materiales (consumo de Inventario vía Outbox) → cierre.
**Endpoints:** `/api/v1/mantenimiento/ordenes`, `/planes`.

### 3.8 Incidencias

**Entidades:** Incidencia, CategoriaIncidencia, ComentarioIncidencia.
**Funcionalidad:** captura de incidencias desde planta (móvil/PWA), asignación, escalado, integración con Mantenimiento (una incidencia puede generar OT).
**Endpoints:** `/api/v1/incidencias`.

### 3.9 CRM

**Entidades:** Oportunidad, EtapaPipeline, ContactoCRM, ActividadCRM.
**Funcionalidad:** pipeline de oportunidades comerciales con etapas, conversión a Cliente + Pedido cuando se gana.
**Endpoints:** `/api/v1/crm/oportunidades`, `/contactos`, `/actividades`.

### 3.10 Costes

**Entidades:** CentroCoste, ImputacionCoste, PresupuestoCentroCoste.
**Funcionalidad:** centros de coste, imputación de horas/materiales/otros, comparación presupuesto vs real.
**Endpoints:** `/api/v1/costes/centros`, `/imputaciones`.

### 3.11 Facturación

**Entidades:** Factura, LineaFactura, SerieFactura.
**Flujo:** generar factura desde Pedido (vía Integration Event de Ventas), numeración por serie, emisión.
**Endpoints:** `/api/v1/facturacion/facturas`, `/series`.

### 3.12 OEE (Overall Equipment Effectiveness)

**Entidades:** RegistroOEE, ParadaProduccion, MotivoParada.
**Funcionalidad:** cálculo de disponibilidad × rendimiento × calidad por activo y turno. Captura paradas con motivo.
**Endpoints:** `/api/v1/oee/registros`, `/paradas`.

### 3.13 RRHH

**Entidades:** Empleado, Departamento, ParteHoras, Ausencia.
**Funcionalidad:** datos básicos de empleados, partes de horas (input para Costes), gestión de ausencias.
**Endpoints:** `/api/v1/rrhh/empleados`, `/partes-horas`.

### 3.14 IA

**Funcionalidad:** integración con Anthropic Claude para:
- Sugerencias de planificación de OFs
- Análisis de no-conformidades recurrentes
- Resumen automático de incidencias
- Asistente conversacional sobre datos del ERP
**Endpoints:** `/api/v1/ia/chat`, `/sugerencias`.

### 3.15 Importador

**Funcionalidad:** carga masiva desde CSV/Excel:
- Productos / clientes / proveedores
- Histórico de movimientos
- Plantilla descargable + validación previa + commit transaccional
**Endpoints:** `/api/v1/importador/upload`, `/plantillas/{tipo}`.

---

## 4. Cross-cutting (Phase 6)

### 4.1 Outbox Pattern

- Tabla `shared.OutboxMessages`.
- `DomainEventInterceptor` (SaveChangesInterceptor de EF Core): captura los `DomainEvents` de los AggregateRoots y los serializa al Outbox dentro de la misma transacción.
- `OutboxProcessor` (BackgroundService): poll cada 5 s, deserializa, publica vía MediatR a `IIntegrationEventHandler<T>` y marca como procesado.
- Garantía: at-least-once cross-módulo sin acoplamiento directo.

### 4.2 Integration Events

Definidos en `SharedKernel/IntegrationEvents/`:
- `RecepcionConfirmadaEvent` (Compras → Inventario: entrada stock)
- `ExpedicionConfirmadaEvent` (Ventas → Inventario: salida stock)
- `OFFinalizadaEvent` (Producción → Inventario: consumo + producto terminado)
- `InspeccionFalladaEvent` (Calidad → Mantenimiento: posible NC con OT)
- `PedidoConfirmadoEvent` (Ventas → Facturación)
- `IncidenciaCriticaEvent` (Incidencias → Mantenimiento)

### 4.3 Audit Log

- `AuditEntry` en schema `shared`: usuario, comando, payload, timestamp, resultado.
- `AuditBehavior` (MediatR pipeline): solo registra Commands, no Queries.
- Endpoint `/api/v1/auditoria` (rol Admin).

### 4.4 MediatR Behaviors

Pipeline en orden:
1. **LoggingBehavior** — Serilog start/stop con duración.
2. **ValidationBehavior** — FluentValidation, devuelve `Result.Failure` si falla.
3. **AuditBehavior** — escribe AuditEntry para Commands.
4. **TransactionBehavior** — abre transacción para Commands.

---

## 5. Seguridad

### 5.1 Identity + JWT

- `PlanTA.Seguridad.Infrastructure` integra ASP.NET Core Identity con `ApplicationUser` (extiende IdentityUser con `Nombre`, `EmpresaId`, `Rol`).
- Endpoints `/api/v1/seguridad/auth/login`, `/register`.
- JWT firmado con HS256, clave 32+ chars, expiración configurable.
- Claims: `sub`, `email`, `nombre`, `rol`, `empresaId`, `empresaNombre`.

### 5.2 Roles

| Rol | Acceso |
|---|---|
| **Admin** | Todo (incluido Auditoría) |
| **Gerente** | Todos los módulos de negocio |
| **JefeAlmacen** | Inventario + Compras (recepciones) |
| **JefeProduccion** | Producción + OEE |
| **JefeCalidad** | Calidad |
| **Comercial** | Ventas + CRM |
| **Tecnico** | Mantenimiento + Incidencias (PWA móvil) |
| **Operario** | PWA móvil: incidencias, partes, ver mis OTs |

El frontend filtra el sidebar según el rol vía `roleGuard`.

### 5.3 DB Seed

`DbInitializer.SeedAsync()` crea en cada arranque:
- Roles (los 8 anteriores).
- Empresa demo `Empresa Demo S.L.`.
- Usuarios demo: `admin@demo.com`, `gerente@demo.com`, `tecnico@demo.com` (password `Demo1234!`).

---

## 6. Frontend Angular 19

### 6.1 Estructura

```
planta-web/src/app/
├── core/
│   ├── auth/                 # AuthService, authGuard, roleGuard, AuthInterceptor (JWT)
│   ├── services/             # ApiService + 1 servicio HTTP por módulo
│   └── shared/               # ToastComponent, NotificationService, PaginationComponent,
│                                ExportService (CSV con BOM)
├── features/
│   ├── inventario/           # productos, almacenes, movimientos, lotes, alertas
│   ├── produccion/           # boms, rutas, ofs
│   ├── compras/              # proveedores, ocs, recepciones
│   ├── ventas/                # clientes, pedidos, expediciones
│   ├── calidad/               # plantillas, inspecciones, ncs
│   ├── activos/               # listado activos
│   ├── mantenimiento/         # órdenes
│   ├── incidencias/           # listado e historial
│   └── movil/                 # PWA: home, mis órdenes, registro de incidencia
├── layout/
│   ├── app-shell.component   # Sidebar filtrado por rol + topbar
│   └── login.component
└── app.routes.ts              # Lazy loading de cada feature
```

### 6.2 Patrones

- **Standalone components** + **OnPush** + **signals** en todos los nuevos.
- **Lazy loading** por feature.
- **Reactive forms** para CRUD modales.
- **Debounce 300ms** en buscadores.
- **PagedResult<T>** del backend mapeado a paginator común.

### 6.3 PWA móvil

- `manifest.webmanifest` con `display: standalone`, theme color, iconos.
- Rutas `/movil/*` con layout simplificado (sin sidebar) para operarios:
  - Home con accesos rápidos.
  - Listado de incidencias propias.
  - Formulario de alta de incidencia (cámara → adjunto pendiente).
  - Mis órdenes de mantenimiento.
- Optimizado táctil, instalable como app.

### 6.4 Servicios HTTP

`ApiService` genérico envuelve `HttpClient` con `HttpParams`. Cada módulo expone su servicio (ej. `InventarioService`) con métodos `listProductos(query)`, `getProducto(id)`, `createProducto(dto)`, etc., todos devolviendo `Observable<PagedResult<T>>` o `Observable<T>`.

### 6.5 Funcionalidad transversal

- **Toasts**: `NotificationService.success/error/info` muestran `ToastComponent` con auto-dismiss 4 s.
- **Confirmación**: modal genérico antes de borrar.
- **Export CSV**: `ExportService.toCsv(rows, filename)` con BOM UTF-8 para compatibilidad Excel.
- **Loading + error states** unificados en cada list page.

---

## 7. Configuración y deploy

### 7.1 Configuración local

| Archivo | Contenido |
|---|---|
| `appsettings.json` | Defaults sin secretos |
| `appsettings.Development.json` | Connection string Docker (gitignoreado) |
| `docker-compose.yml` | PostgreSQL 16 puerto **5433** + Seq |
| `planta-web/src/environments/environment.ts` | `apiUrl: '/api/v1'` (proxy) |
| `planta-web/proxy.conf.json` | `/api → http://localhost:5071` |

### 7.2 Configuración producción

| Servicio | URL/host | Notas |
|---|---|---|
| Frontend | Vercel | `ng build --configuration=production` con `fileReplacements` |
| API | Render (region Frankfurt) | Web Service desde Dockerfile o build .NET |
| BD | Neon PostgreSQL | Connection string en env vars de Render |

**Variables de entorno Render:**
```
ConnectionStrings__DefaultConnection=<neon-string>
Jwt__Key=<32+ chars>
Anthropic__ApiKey=<api-key>
Frontend__Url=https://planta-erp.vercel.app
ASPNETCORE_ENVIRONMENT=Production
```

### 7.3 CI/CD

- Push a `master` → Render redeploya la API automáticamente.
- Vercel detecta cambios en `planta-web/` y redeploya el frontend.
- Sin tests en pipeline aún (próximo paso).

### 7.4 Inicialización de la BD

Al primer arranque en producción, `DbInitializer` ejecuta `EnsureCreatedAsync()` por **cada uno de los 16 DbContext** (15 módulos + Outbox/Audit). Tras eso siembra roles, empresa demo y usuarios demo. **Importante:** al añadir un módulo nuevo, asegurarse de registrar su DbContext en el initializer.

---

## 8. Estado actual (2026-04-14)

- ✅ 15 módulos backend implementados, build limpio (0 errores, 0 warnings).
- ✅ Cross-cutting (Outbox, Audit, MediatR Behaviors) funcionando.
- ✅ Frontend con 9 features web + PWA móvil.
- ✅ Deploy productivo Render + Vercel + Neon (los 5 módulos originales). Los 10 nuevos están commiteados y pendientes de redeploy + verificación de creación de tablas.
- ⚠️ Frontend de los módulos CRM, Costes, Facturación, IA, Importador, OEE, RRHH **aún no está implementado** — solo existe el backend.
- ⚠️ Tests E2E pendientes.

---

## 9. Roadmap próximo

1. Verificar que Render despliega los 10 nuevos schemas.
2. Implementar frontend para CRM, Facturación, OEE, RRHH (los más prioritarios para el cliente).
3. Tests de integración por módulo (xUnit + Testcontainers).
4. Webhooks de Stripe para facturación SaaS.
5. Notificaciones push en la PWA móvil.
