# CLAUDE.md — JchLegal

Guía de contexto para Claude Code. Lee este archivo antes de cualquier análisis o implementación.

---

## Propósito del proyecto

JchLegal es un **gestor de expedientes legales** para estudios de abogados.
Consume el servicio de autenticación centralizado **AuthService** para la gestión de usuarios y sesiones.

Responsabilidades del servicio:
- Gestión de clientes (personas naturales y empresas)
- Gestión de expedientes (casos) con partes involucradas
- Bitácora de actividad por expediente
- Agenda de audiencias con alertas
- Control de honorarios y pagos
- Dashboard con KPIs del estudio

---

## Servicios relacionados

| Servicio | Rol | Puerto |
|---|---|---|
| **AuthService** | Autenticación, usuarios, roles, JWT | `http://localhost:58997` |
| **JchLegal** | Lógica de negocio legal | `http://localhost:XXXX` |
| **PostgreSQL** | Base de datos compartida (mismo contenedor Docker) | `host.docker.internal:5433` |

**Base de datos:** `jchlegal_db` en el mismo contenedor que `auth_db`.

---

## Stack técnico

| Capa | Tecnología |
|---|---|
| Runtime | .NET 9 |
| Framework web | ASP.NET Core 9 (minimal hosting) |
| ORM | Entity Framework Core 9 + Npgsql |
| Base de datos | PostgreSQL (`jchlegal_db`) |
| Mensajería interna | MediatR (CQRS) |
| Autenticación | JWT Bearer — el token lo emite AuthService |
| Logging | NLog 5 via `NLog.Web.AspNetCore` |
| Documentación API | Swagger (Swashbuckle.AspNetCore) |
| Validación | FluentValidation |
| Contenedor | Docker |

---

## Arquitectura DDD

```
JchLegal.ApplicationApi      ← Capa de presentación (entrada HTTP)
JchLegal.Domain              ← Núcleo del dominio (sin dependencias externas)
JchLegal.Infrastructure      ← Persistencia, DbContext EF Core
JchLegal.UnitTest            ← Tests unitarios (xUnit + Moq)
JchLegal.FunctionalTest      ← Tests de integración (TestHost)
```

### Dependencias entre capas

```
ApplicationApi → Domain
ApplicationApi → Infrastructure
Infrastructure → Domain
UnitTest       → ApplicationApi + Domain + Infrastructure
FunctionalTest → ApplicationApi + Domain + Infrastructure
```

`Domain` no debe depender de ningún otro proyecto del solution.

---

## Estructura de carpetas

```
JchLegal.ApplicationApi/
├── Application/
│   ├── Command/
│   │   ├── Clients/       ← CreateClientRequest, UpdateClientRequest
│   │   ├── Cases/         ← CreateCaseRequest, UpdateCaseRequest, PatchCaseStatusRequest
│   │   ├── Bitacora/      ← Create, Update, DeleteBitacoraEntryRequest
│   │   ├── Hearings/      ← Create, Update, PatchHearingStatusRequest
│   │   └── Fees/          ← CreatePaymentRequest
│   ├── Query/
│   │   ├── Clients/       ← GetClients, GetClientById, GetClientByUserId
│   │   ├── Cases/         ← GetCases, GetCaseById
│   │   ├── Bitacora/      ← GetBitacora
│   │   ├── Hearings/      ← GetHearings, GetHearingsByCase
│   │   ├── Fees/          ← GetCaseFee, GetAllFees
│   │   └── Dashboard/     ← GetDashboardSummary
│   └── DTOs/              ← ClientDto, CaseDto, BitacoraEntryDto, HearingDto, FeeDto, DashboardDto
├── Controllers/           ← ClientsController, CasesController, HearingsController, FeesController, DashboardController
├── Middleware/            ← TenantMiddleware, ExceptionMiddleware
├── Program.cs
└── appsettings.json

JchLegal.Domain/
├── Models/                ← Entidades de dominio + lookup tables
├── Repository/            ← Interfaces de repositorio (contratos)
├── SeedWork/              ← PagedResponse, DomainException, IUnitOfWork, IAppLogger
└── Services/              ← ITenantContext, JwtSettings

JchLegal.Infrastructure/
├── Context/               ← JchLegalDbContext (EF Core, mappings completos)
├── Repository/            ← Implementaciones: ClientRepository, CaseRepository, etc.
└── Services/              ← TenantContext
```

---

## Modelos de dominio

### Tablas de catálogo (lookup tables)

Todas tienen la misma estructura: `id (SMALLINT)`, `code (VARCHAR UNIQUE)`, `name (VARCHAR)`.
El frontend y los handlers usan siempre el `code` (string), nunca el `id` numérico.

| Entidad C# | Tabla SQL | Valores `code` |
|---|---|---|
| `ClientType` | `client_types` | `persona`, `empresa` |
| `CaseMateria` | `case_materias` | `civil`, `penal`, `laboral`, `familiar` |
| `CaseStatus` | `case_statuses` | `activo`, `cerrado`, `suspendido`, `en_apelacion`, `archivado`, `en_tramitacion` |
| `CasePartRole` | `case_part_roles` | `demandante`, `demandado`, `tercero` |
| `BitacoraEventType` | `bitacora_event_types` | `nota`, `audiencia`, `escrito`, `resolucion`, `juicio`, `apelacion`, `notificacion`, `diligencia` |
| `HearingStatus` | `hearing_statuses` | `programada`, `realizada`, `suspendida`, `reprogramada` |
| `FeeConcepto` | `fee_conceptos` | `consulta`, `representacion`, `tramitacion`, `otros` |
| `PaymentMethod` | `payment_methods` | `transferencia`, `efectivo`, `cheque`, `otro` |

### Entidades principales

| Entidad | Tabla | Descripción |
|---|---|---|
| `Client` | `clients` | Cliente del estudio (persona o empresa). Puede vincularse a un usuario de AuthService via `UserId` |
| `Case` | `cases` | Expediente legal. Pertenece a un `Client`. Tiene partes, bitácora, audiencias y honorario |
| `CasePart` | `case_parts` | Parte involucrada en el caso (demandante, demandado, tercero) |
| `BitacoraEntry` | `bitacora_entries` | Entrada de actividad en el expediente. Puede tener adjuntos |
| `Attachment` | `attachments` | Archivo adjunto a una entrada de bitácora |
| `Hearing` | `hearings` | Audiencia programada para un caso |
| `Fee` | `fees` | Honorario del caso (1:1 con Case). Contiene el monto total |
| `Payment` | `payments` | Pago registrado contra un honorario |

### PKs
Todas las entidades principales usan `UUID` como PK generado con `gen_random_uuid()`.

---

## Endpoints disponibles

### Multi-tenancy
Todos los endpoints (excepto Swagger y Health) requieren el header:
```
X-Tenant-Id: {tenantCode}
```

### Autenticación
Los endpoints de auth viven en **AuthService**, no en JchLegal.

```
POST /api/User/authenticate  → AuthService
POST /api/User/refresh       → AuthService
POST /api/User/logout        → AuthService
GET  /api/User/me            → AuthService
GET  /api/Users              → AuthService
```

### JchLegal endpoints

```
Clientes
  GET    /api/Clients                    ?search=&type=&page=&pageSize=
  GET    /api/Clients/{id}
  GET    /api/Clients/by-user/{userId}   ← portal del cliente
  POST   /api/Clients
  PUT    /api/Clients/{id}

Expedientes
  GET    /api/Cases                      ?status=&materia=&assignedLawyerId=&clientId=&search=&page=&pageSize=
  GET    /api/Cases/{id}
  POST   /api/Cases
  PUT    /api/Cases/{id}
  PATCH  /api/Cases/{id}/status          body: { "status": "cerrado" }

Bitácora (anidado en Cases)
  GET    /api/Cases/{caseId}/bitacora    ?visibleToClient=
  POST   /api/Cases/{caseId}/bitacora
  PUT    /api/Cases/{caseId}/bitacora/{entryId}
  DELETE /api/Cases/{caseId}/bitacora/{entryId}

Audiencias
  GET    /api/Cases/{caseId}/hearings
  GET    /api/Hearings                   ?caseId=&status=&from=&to=
  POST   /api/Hearings
  PUT    /api/Hearings/{id}
  PATCH  /api/Hearings/{id}/status       body: { "status": "realizada" }

Honorarios
  GET    /api/Cases/{caseId}/fees
  POST   /api/Cases/{caseId}/fees/payments
  GET    /api/Fees                       ?status=&clientId=&assignedLawyerId=

Dashboard
  GET    /api/Dashboard/summary
```

---

## Patrón CQRS con MediatR

Mismo patrón que AuthService: `Request → Handler → Response/DTO`.

**Convención de nombres:**
- Comandos (mutan estado): carpeta `Command/`, archivos `{Accion}{Entidad}Request.cs` y `{Accion}{Entidad}Handler.cs`
- Queries (solo lectura): carpeta `Query/`, misma convención
- Los handlers resuelven lookup codes a IDs consultando las tablas de catálogo

---

## Interfaces de repositorio

```
IClientRepository   → GetAllAsync, GetByIdAsync, GetByUserIdAsync, CreateAsync, UpdateAsync
ICaseRepository     → GetAllAsync, GetByIdAsync, CreateAsync, UpdateAsync, PatchStatusAsync
IBitacoraRepository → GetByCaseAsync, GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync
IHearingRepository  → GetAllAsync, GetByCaseAsync, GetByIdAsync, CreateAsync, UpdateAsync, PatchStatusAsync
IFeeRepository      → GetByCaseAsync, GetAllAsync, CreatePaymentAsync, GetFeeByIdAsync
```

Registradas en `Program.cs` con `AddScoped`.

---

## Lógica de negocio importante

### Estado de honorarios (calculado, no almacenado)
```
sum(payments.amount) == 0              → "pendiente"
sum(payments.amount) >= fee.totalAmount → "pagado"
sum(payments.amount) > 0               → "parcial"
```

### Visibilidad de bitácora
`visibleToClient = true` → visible en el portal del cliente.
`visibleToClient = false` → solo visible para el estudio.

### Portal del cliente
`GET /api/Clients/by-user/{userId}` es el endpoint crítico para el portal.
El cliente se vincula a un usuario de AuthService via el campo `UserId (long?)` en la tabla `clients`.

---

## Multi-tenancy

`TenantMiddleware` extrae el header `X-Tenant-Id` y llama a `ITenantContext.SetTenant(id, code)`.
Todos los repositorios filtran por `TenantId` en cada consulta.

**TODO pendiente:** resolver el tenant desde `JchLegalDbContext` (tabla de tenants propia o llamada a AuthService) en lugar del hardcoded `id=1` actual.

---

## Configuración (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "MainConnectionString": "Host=host.docker.internal;Port=5433;Database=jchlegal_db;Username=auth_admin;Password=123456"
  },
  "Jwt": {
    "Key": "...",
    "Issuer": "AuthService",
    "Audience": "AuthServiceUsers"
  }
}
```

El JWT es emitido por **AuthService** y validado aquí. La clave y el issuer deben coincidir entre ambos servicios.

---

## Base de datos

- Motor: PostgreSQL 15+
- Base de datos: `jchlegal_db` (mismo contenedor que `auth_db`, puerto 5433)
- Convención snake_case para tablas y columnas
- PKs: UUID con `gen_random_uuid()`
- Las tablas de catálogo tienen datos iniciales (seed) que deben existir antes de arrancar la app

---

## Convenciones de código

- `Nullable=enable` en todos los proyectos
- `ImplicitUsings=enable` en todos los proyectos
- PascalCase para clases, propiedades y métodos
- `_camelCase` para campos privados
- Controladores heredan de `ControllerBase`, decorados con `[ApiController]` y `[Route("api/[controller]")]`
- Sin lógica en controladores — todo delegado al mediator
- Repositorios: interfaces en `Domain/Repository/`, implementaciones en `Infrastructure/Repository/`
- Los métodos async llevan sufijo `Async`
- Los lookups se resuelven siempre por `code` (string) → `id` (short)
