# PlanTA — Flujo de Pruebas

Guía paso a paso para arrancar PlanTA en local y validar **todos** los módulos. Sigue el orden — cada bloque depende del anterior.

---

## 0. Requisitos previos

| Herramienta | Versión | Comprobar con |
|---|---|---|
| .NET SDK | 9.0+ | `dotnet --version` |
| Node.js | 20+ | `node -v` |
| Angular CLI | 19+ | `ng version` |
| Docker Desktop | última | `docker --version` |
| PostgreSQL client (opcional) | — | `psql --version` |

> **Aviso:** el usuario tiene PostgreSQL local en el puerto 5432. PlanTA usa Docker en **5433** para no chocar.

---

## 1. Arrancar la base de datos

```bash
cd P:\ClaudeCode\projects\mini-SAP
docker-compose up -d
```

**Validar:**
- `docker ps` muestra los contenedores `postgres` (5433) y `seq` (5341 si está incluido).
- Conexión manual opcional:
  ```bash
  psql -h localhost -p 5433 -U postgres -d planta
  ```
- La BD `planta` existe (la crea EF al arrancar la API si no existe).

---

## 2. Arrancar la API .NET

```bash
cd src/PlanTA.API
dotnet run
```

**Qué validar:**
- Arranca en `http://localhost:5071`.
- Logs Serilog en consola sin excepciones.
- En el primer arranque, `DbInitializer` crea **16 schemas** (los 15 módulos + `shared`) y siembra:
  - Roles: Admin, Gerente, JefeAlmacen, JefeProduccion, JefeCalidad, Comercial, Tecnico, Operario.
  - Empresa demo.
  - Usuarios demo.
- Swagger disponible en `http://localhost:5071/swagger`.
- `GET http://localhost:5071/health` devuelve 200.

**Probar el seed con psql:**
```sql
\dn                              -- lista schemas, deben aparecer los 16
SELECT * FROM seguridad."AspNetRoles";
SELECT * FROM seguridad."AspNetUsers";
```

---

## 3. Arrancar el frontend Angular

```bash
cd planta-web
npm install                      # solo la primera vez
npm start
```

**Validar:**
- Arranca en `http://localhost:4200`.
- El proxy `/api` → `http://localhost:5071` está activo (`proxy.conf.json`).
- No hay errores rojos en consola del navegador.
- Aparece la pantalla de login con branding PlanTA.

---

## 4. Login y roles

### 4.1 Login como Admin

- Email: `admin@demo.com`
- Password: `Demo1234!`

**Validar:**
- Redirige al dashboard.
- AppShell muestra el sidebar con **todos los módulos** y la entrada de "Auditoría".
- Topbar muestra `Admin Demo · Empresa Demo S.L.`.
- LocalStorage guarda el JWT.

### 4.2 Probar otros roles

Logout y login como:
- `gerente@demo.com` → ve todo menos auditoría.
- `tecnico@demo.com` → solo Mantenimiento + Incidencias + Móvil.

**Validar:** el sidebar se filtra por rol vía `roleGuard`.

### 4.3 Probar acceso denegado

Como `tecnico@demo.com`, intenta navegar manualmente a `/inventario/productos`.

**Validar:** redirige a "no autorizado" o al dashboard.

---

## 5. Dashboard

Como Admin, en el dashboard:
- Validar que se cargan los **KPIs reales** vía `forkJoin` (productos totales, OFs activas, OCs pendientes, etc.).
- Si la API está caída, debe mostrar "Sin conexión API" como fallback (no crash).
- Accesos rápidos a los módulos.

---

## 6. Módulo Inventario

### 6.1 Productos

- `Inventario → Productos`. Lista paginada vacía (o con seed si lo hay).
- **Crear:** botón "+", abre modal con form (SKU, nombre, familia, stock mínimo, etc.). Guardar.
- **Validar:** toast verde "Producto creado", aparece en la lista.
- **Detail:** clic en la fila → página de detalle con datos + historial de movimientos.
- **Editar:** desde detalle.
- **Borrar:** modal de confirmación → toast.
- **Buscar:** input con debounce 300 ms.
- **Paginación:** crear ≥ 11 productos para ver el paginator.
- **Export CSV:** botón → descarga `productos.csv` con BOM (Excel lo abre con tildes correctas).

### 6.2 Almacenes y Movimientos

- Crear 2 almacenes.
- Crear un movimiento: tipo "Entrada", producto, almacén, cantidad → guardar.
- **Validar:** el stock del producto sube en el detail.

### 6.3 Lotes y Alertas

- Crear lote asociado a un producto.
- Reducir stock por debajo del mínimo → debe aparecer una alerta en `Inventario → Alertas`.
- Cerrar la alerta.

---

## 7. Módulo Producción

### 7.1 BOMs

- Crear un BOM para uno de los productos: añadir 2-3 líneas (componentes + cantidades).
- Detail muestra las líneas.

### 7.2 Rutas

- Crear una ruta de producción con 2 operaciones (tiempo estimado).

### 7.3 Órdenes de Fabricación

- Crear OF: producto, cantidad, BOM, ruta.
- **Validar:** estado `Borrador`.
- Cambiar estado a `EnCurso` desde el detail.
- Registrar producción (parte): cantidad real producida.
- Cambiar estado a `Finalizada`.
- **Validar Outbox:** el `OFFinalizadaEvent` se publica → el stock del producto terminado sube en Inventario (espera ~5 s al poll del OutboxProcessor).

---

## 8. Módulo Compras

### 8.1 Proveedores

- Crear proveedor con contacto.
- CRUD completo.

### 8.2 Órdenes de Compra

- Crear OC: proveedor, líneas (producto + cantidad + precio).
- Cambiar estado: `Borrador → Enviada → Recibida`.

### 8.3 Recepciones

- Desde una OC en estado `Enviada`, crear recepción.
- Confirmar recepción.
- **Validar Outbox:** stock del producto sube en Inventario tras ~5 s.

---

## 9. Módulo Ventas

### 9.1 Clientes

- CRUD de clientes con contactos.

### 9.2 Pedidos

- Crear pedido: cliente, líneas.
- Confirmar pedido.

### 9.3 Expediciones

- Desde pedido confirmado, crear expedición.
- Confirmar expedición.
- **Validar Outbox:** stock baja en Inventario.

---

## 10. Módulo Calidad

### 10.1 Plantillas

- Crear plantilla "Inspección recepción" con 3 criterios.

### 10.2 Inspecciones

- Crear inspección sobre una recepción usando la plantilla.
- Registrar resultado de cada criterio.
- Completar inspección.

### 10.3 No Conformidades

- Si un criterio falla, abrir NC.
- Añadir acción correctiva.
- Cambiar estado: `Abierta → EnCurso → Cerrada`.

---

## 11. Módulo Activos

- `Activos → Listado`. Crear 3 activos: máquina, herramienta, instalación.
- Marcar uno como crítico.
- Cambiar estado a "En mantenimiento".

---

## 12. Módulo Mantenimiento

### 12.1 Órdenes desde web

- Crear orden de mantenimiento correctiva sobre un activo.
- Asignar técnico (`tecnico@demo.com`).
- Cambiar estado a `EnCurso → Cerrada`.

### 12.2 Plan preventivo

- Crear plan preventivo (frecuencia mensual sobre un activo).
- **Validar:** el plan genera OTs automáticamente (verificar al día siguiente o forzando con job manual si existe).

---

## 13. Módulo Incidencias

- `Incidencias → Listado`. Crear incidencia desde web.
- Asignar a técnico.
- Comentar.
- Cerrar.
- **Validar Outbox:** una incidencia crítica genera OT en Mantenimiento.

---

## 14. PWA Móvil

### 14.1 Instalación

- Abre `http://localhost:4200/movil` en Chrome móvil (o emulador móvil de DevTools).
- Chrome ofrece "Instalar PlanTA" → instala como app.

### 14.2 Login como técnico

- `tecnico@demo.com / Demo1234!`.
- Home con accesos rápidos: Mis Órdenes, Nueva Incidencia, Listado.

### 14.3 Crear incidencia desde móvil

- Botón "Nueva incidencia" → form simplificado táctil.
- Guardar.
- **Validar:** aparece en `Incidencias` web inmediatamente.

### 14.4 Mis órdenes

- Lista solo las OTs asignadas a este técnico.
- Tap en una OT → vista detalle simplificada → cambiar estado.

---

## 15. Módulos backend-only (probar vía Swagger)

Los siguientes módulos **no tienen UI todavía** — pruébalos desde Swagger (`http://localhost:5071/swagger`) usando el JWT del Admin (botón "Authorize" pegando `Bearer <token>`).

### 15.1 CRM

- `POST /api/v1/crm/oportunidades` — crear oportunidad.
- `GET /api/v1/crm/oportunidades` — listar.

### 15.2 Costes

- `POST /api/v1/costes/centros` — crear centro de coste.
- `POST /api/v1/costes/imputaciones` — imputar horas/material.

### 15.3 Facturación

- `POST /api/v1/facturacion/series` — crear serie.
- `POST /api/v1/facturacion/facturas` — generar factura desde un pedido (vía Integration Event idealmente, o manual).

### 15.4 OEE

- `POST /api/v1/oee/registros` — registrar disponibilidad/rendimiento/calidad de un activo en un turno.
- `POST /api/v1/oee/paradas` — registrar parada con motivo.
- `GET /api/v1/oee/registros?activoId=...` — consultar OEE calculado.

### 15.5 RRHH

- `POST /api/v1/rrhh/empleados` — alta empleado.
- `POST /api/v1/rrhh/partes-horas` — registrar parte de horas.
- `POST /api/v1/rrhh/ausencias` — registrar ausencia.

### 15.6 IA

- `POST /api/v1/ia/chat` — payload `{ "mensaje": "¿Cuántos productos hay con stock bajo?" }`.
- **Validar:** respuesta de Claude con datos del ERP. Requiere `Anthropic__ApiKey` configurada.

### 15.7 Importador

- `GET /api/v1/importador/plantillas/productos` — descarga plantilla CSV.
- `POST /api/v1/importador/upload` — subir CSV de productos.
- **Validar:** los productos aparecen en Inventario.

---

## 16. Cross-cutting

### 16.1 Outbox Pattern

Ya validado implícitamente en los pasos 7.3, 8.3, 9.3, 13.

**Validación directa:**
```sql
SELECT * FROM shared."OutboxMessages" ORDER BY "OccurredOn" DESC LIMIT 10;
```
- Mensajes recientes deben tener `ProcessedOn` no nulo (procesados en ≤ 5 s).

### 16.2 Auditoría

- Como Admin, navega a `Auditoría` (sidebar).
- **Validar:** lista de Commands ejecutados con usuario, payload, timestamp.
- Filtrar por usuario.
- Filtrar por módulo.

### 16.3 Logging

- En la consola de la API debe verse Serilog con estructura:
  ```
  [INF] HTTP POST /api/v1/inventario/productos started
  [INF] CreateProductoCommand handled in 42ms
  [INF] HTTP POST /api/v1/inventario/productos finished in 67ms - 200
  ```

---

## 17. Multi-tenancy

### 17.1 Crear segunda empresa

Vía API o seed manual:
```sql
INSERT INTO seguridad."Empresas" ("Id", "Nombre") VALUES ('<guid>', 'Empresa B');
```

Crear un usuario `gerenteB@demo.com` asociado a Empresa B.

### 17.2 Aislamiento

- Login como `admin@demo.com` (Empresa Demo) → ve sus productos.
- Logout. Login como `gerenteB@demo.com` (Empresa B) → lista vacía.
- Crea un producto como B.
- **Validar:** Empresa Demo no lo ve.

---

## 18. Tests automáticos

```bash
cd P:\ClaudeCode\projects\mini-SAP
dotnet test
```

**Validar:** todos los tests pasan. Si no hay tests, anotarlo como pendiente.

```bash
cd planta-web
npm test
```

---

## 19. Deploy a producción

### 19.1 Push

```bash
git push
```

- Render detecta y empieza el build de la API.
- Vercel detecta cambios en `planta-web/` y empieza el build.

### 19.2 Validar Render

- Dashboard de Render → ver logs del deploy.
- **Validar:** `DbInitializer` crea los **16 schemas** sin error en Neon.
- `GET https://planta-erp.onrender.com/health` → 200.
- Login funcional contra el frontend de Vercel.

### 19.3 Validar Vercel

- `https://planta-erp.vercel.app` carga.
- Login con usuarios demo funciona.
- Network → las llamadas van a la URL de Render (no a localhost).
- CORS sin errores.

---

## 20. Checklist final

- [ ] Docker arranca PostgreSQL en 5433
- [ ] API arranca en 5071, schemas y seed creados
- [ ] Frontend arranca en 4200 con proxy
- [ ] Login con los 3 usuarios demo, sidebar filtrado por rol
- [ ] Dashboard con KPIs reales
- [ ] CRUD completo en Inventario (productos, almacenes, movimientos, lotes, alertas)
- [ ] BOM + Ruta + OF en Producción, Outbox actualiza stock
- [ ] Compras: OC → Recepción → stock sube
- [ ] Ventas: Pedido → Expedición → stock baja
- [ ] Calidad: Plantilla → Inspección → NC → Acción
- [ ] Activos: CRUD con criticidad y estados
- [ ] Mantenimiento: OT correctiva + plan preventivo
- [ ] Incidencias: web + integración con Mantenimiento
- [ ] PWA móvil instalable, login técnico, alta de incidencia
- [ ] Swagger: CRM, Costes, Facturación, OEE, RRHH, IA, Importador
- [ ] Outbox: mensajes procesados en ≤ 5 s
- [ ] Audit Log: Commands registrados
- [ ] Multi-tenancy: aislamiento entre empresas
- [ ] Export CSV con tildes correctas
- [ ] Paginación + búsqueda con debounce
- [ ] Toasts de éxito/error
- [ ] Deploy: Render + Vercel + Neon funcionando con los 16 schemas

---

## 21. Troubleshooting

| Síntoma | Causa | Solución |
|---|---|---|
| `port 5432 already in use` | PG local del usuario | Confirmar que docker usa 5433 (`docker-compose.yml`) |
| `relation "..." does not exist` en Neon | DbInitializer no registró el nuevo DbContext | Añadir el DbContext nuevo a `EnsureCreatedAsync` en `Program.cs` |
| Login devuelve 500 | JWT key < 32 chars o seed falló | Revisar `Jwt__Key` y logs de seed |
| CORS bloqueado en Vercel | Origen no incluido | Añadir URL real de Vercel a `Frontend__Url` en Render |
| `MediatR no resuelve handler` | Falta assembly ref | Verificar `AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(...))` con todos los Application |
| Sidebar muestra módulos prohibidos | `roleGuard` no filtra | Revisar el rol en el JWT (claim `rol`) |
| Outbox no procesa | `OutboxProcessor` no registrado como hosted service | `builder.Services.AddHostedService<OutboxProcessor>()` |
| Frontend sin estilos | Build sin Tailwind/SCSS | `npm run build` y revisar `angular.json` |

---

**Fin del flujo de pruebas.** Si el checklist de la sección 20 está marcado al 100%, PlanTA está validado en local y producción.
