# CLAUDE.md — Project & Task Management Platform

This file orients any Claude session working in this repository. Read `planning.md` for
architecture/design detail and `tasks.md` for the current phase checklist before making changes.

## What this is

An offline-first, multi-tenant Project & Task Management platform (Jira/Trello-class) with:
- **Backend**: ASP.NET Core Web API 8, Clean Architecture, EF Core 8, SQL Server.
- **Mobile/Web/Desktop client**: Flutter + GetX, SQLite (Drift) for offline-first local storage,
  Dio for networking, a custom sync engine reconciling local changes with the server.

Build order for this project is **backend-first**: solidify the API contract (auth, RBAC, core
entities, sync endpoints) before building Flutter screens against it. See `tasks.md` for phase
status.

## Repository layout

```
Project Tracker/
├── CLAUDE.md              this file
├── planning.md             architecture, ERD, API spec, permission matrix, sync design
├── tasks.md                 phase-by-phase task checklist (source of truth for progress)
├── backend/
│   ├── ProjectTracker.sln
│   └── src/
│       ├── ProjectTracker.Api                (Controllers, Middleware, Filters, Swagger, Auth wiring, HealthChecks)
│       ├── ProjectTracker.Application         (Features/Commands/Queries, DTOs, Validators, Interfaces, Mappings)
│       ├── ProjectTracker.Domain              (Entities, ValueObjects, Enums, DomainEvents, Specifications, Exceptions)
│       ├── ProjectTracker.Infrastructure       (EF Core, Repositories, UnitOfWork, FileStorage, Notifications, BackgroundJobs)
│       └── ProjectTracker.Shared               (cross-cutting constants/result types shared by all layers)
│   └── tests/
│       ├── ProjectTracker.UnitTests
│       └── ProjectTracker.IntegrationTests
├── mobile/                  Flutter app (created in the mobile phase; see tasks.md)
└── docs/                    ERD exports, deployment notes, API docs snapshots
```

## Backend conventions

- **Clean Architecture dependency rule**: Api → Application → Domain ← Infrastructure. Domain has
  zero dependencies on other layers. Infrastructure implements Application's interfaces.
- **No MediatR.** CQRS-style separation is achieved with plain `I{X}Command`/`I{X}Query` handler
  classes registered via DI (see `planning.md` §Application Layer Pattern for the exact shape).
  Controllers call these handlers directly through injected interfaces — controllers stay thin.
- **Validation split**: FluentValidation validators handle object/shape validation
  (`Application/Validators`). Business-rule validation (e.g. "cannot start a sprint that's already
  active") lives in Domain as explicit rule classes (`Domain/BusinessRules`), invoked from
  Application handlers — never from controllers.
- **Async everywhere**: `async/await`, `CancellationToken` threaded through every handler/repo
  method, `IQueryable` composed at the repository/query layer, `AsNoTracking()` on all read-only
  queries.
- **Multi-tenancy**: every organization-owned entity carries `OrganizationId`. Tenant isolation is
  enforced with an EF Core global query filter tied to the current user's `OrganizationId` claim —
  never trust `OrganizationId` from the request body/query string. Exception: Login/RefreshToken
  handlers query `OrganizationUsers` with `.IgnoreQueryFilters()`, since at that point there is no
  `OrganizationId` claim yet — that's the one legitimate cross-tenant lookup (scoped by the
  already-credential-verified `UserId`), used to discover which org(s) to issue a token for.
- **Auth**: JWT bearer, claims include `sub` (UserId), `org` (OrganizationId), `role`, and
  permission claims. Refresh tokens are rotated and stored hashed. Authorization is
  permission-based (policy per permission string, e.g. `task.assign`), not just role-based.
- **Identifiers**: entities use `Guid` primary keys (required for offline-created records — see
  `planning.md` §Offline ID Strategy). Human-facing task numbers (`ERP-101`) are a separate
  server-assigned sequence per project, generated transactionally.
- **Idempotency**: every mutating sync operation carries a client-generated `OperationId`; the
  server persists processed operation ids and short-circuits duplicates (see `planning.md` §Sync).
- **Concurrency**: optimistic concurrency via a `RowVersion`/`byte[]` timestamp column on
  syncable entities; conflicting updates return 409 with both versions.
- **Naming**: the Domain entity for a work item is `TaskItem`, not `Task` — `System.Threading.
  Tasks.Task`/`TaskStatus` are implicitly in scope everywhere (`ImplicitUsings`) and would
  collide with a bare `Task`/`TaskStatus` type on every single `async Task<...>` method
  signature in the codebase. The enum is `TaskItemStatus` for the same reason. This is purely a
  C# identifier choice — JSON payloads, routes (`/api/v1/tasks`), permission keys (`task.*`),
  DTOs (`TaskDto`) and the human-facing "task key" (`ERP-101`) all still say "task" freely.
- **Secrets/config**: `appsettings.json` holds connection strings, JWT signing config, etc. for
  local dev. **Local SQL Server connection uses Windows Authentication**:
  `Server=localhost;Database=ProjectTracker;Trusted_Connection=True;TrustServerCertificate=True`.
  Do not commit real production secrets — use `appsettings.Development.json` (gitignored) or
  environment variables for anything sensitive beyond local dev defaults.

## What NOT to do (from the spec's hard rules)

- No business logic in controllers; no direct DB calls from Flutter widgets.
- No `NotImplementedException`/TODO placeholders in code presented as done.
- No silent overwrite of newer server data on conflict.
- No unbounded `GET` endpoints — every list endpoint is paginated.
- No plaintext password storage; no tokens logged; no tokens in non-secure storage on the client.

## Working conventions for this repo

- Follow the phase order in `tasks.md`. Mark tasks done there as they're completed — it's the
  progress source of truth across sessions.
- After each phase: build, run tests, fix errors, then proceed — don't stack unverified phases.
- Keep `planning.md` in sync if architecture decisions change during implementation.
