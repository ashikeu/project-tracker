# Project Tracker

An offline-first, multi-tenant **Project & Task Management platform** (Jira/Trello-class).

- **Backend** — ASP.NET Core Web API 8, Clean Architecture, EF Core 8, SQL Server. CQRS-style
  handlers (no MediatR), permission-based auth (JWT), tenant isolation via EF global query filters,
  and an offline sync engine (idempotent push + delta pull).
- **Client** — Flutter + GetX + Drift (offline-first), a custom sync engine reconciling local
  changes with the server. *(Planned — see `tasks.md` Phase 10.)*

The build order is **backend-first**: the API contract is solidified before the Flutter screens are
built against it. See `planning.md` for the full architecture and `tasks.md` for phase-by-phase
status.

## Features (backend, implemented)

- **Auth & orgs** — register, login, JWT access + rotating refresh tokens, password reset/change,
  email verification; organizations, members, invitations, roles.
- **RBAC** — permission-based policies (one policy per permission key, e.g. `task.assign`), seeded
  system roles, per-org isolation enforced by a global query filter.
- **Work items** — teams, projects (unique keys, `ERP-101` task numbering), epics, tasks + subtasks,
  labels, watchers, checklists, dependencies (cycle-checked), bulk operations.
- **Kanban** — board columns (WIP limits, status mapping), fractional-index drag/drop positioning.
- **Sprints & backlog** — guarded state machine (Planning → Active → Completed/Cancelled), backlog
  reorder, velocity.
- **Collaboration** — threaded comments with mentions, attachments (pluggable file storage), time
  tracking (manual + start/stop timer).
- **Notifications & activity** — in-app notifications (assign/status/mention/comment/due-soon) with
  per-user preferences, FCM push seam, append-only activity log.
- **Insight** — project dashboard, sprint burndown (reconstructed from activity history), global
  search, saved filters, and reports (sprint, workload, epic progress, time tracking, team
  productivity, completion).
- **Sync** — `POST /sync/push` (batched, idempotent by OperationId, optimistic-concurrency
  conflicts) and `GET /sync/pull?since=` (delta change-feed with tombstones).

## Repository layout

```
Project Tracker/
├── backend/
│   ├── ProjectTracker.sln
│   ├── Dockerfile
│   ├── src/
│   │   ├── ProjectTracker.Api             Controllers, middleware, filters, Swagger, auth wiring
│   │   ├── ProjectTracker.Application     Handlers (commands/queries), DTOs, validators, interfaces
│   │   ├── ProjectTracker.Domain          Entities, enums, business rules (zero external deps)
│   │   ├── ProjectTracker.Infrastructure  EF Core, repositories, file storage, notifications, jobs
│   │   └── ProjectTracker.Shared          Cross-cutting constants + result types
│   └── tests/
│       ├── ProjectTracker.UnitTests
│       └── ProjectTracker.IntegrationTests
├── docs/                                   ERD, API summary, deployment notes
├── docker-compose.yml                      Local dev stack (API + SQL Server)
├── planning.md                             Architecture, ERD, API spec, permission matrix, sync
└── tasks.md                                Phase checklist (progress source of truth)
```

## Running locally

### Option A — Docker (API + SQL Server)

```bash
docker compose up --build
```

- API: <http://localhost:8080> — Swagger at `/swagger`, health at `/health`
- SQL Server: `localhost:1433` (`sa` / `Your_strong_Pass123`)

The API waits for the DB healthcheck, applies EF migrations, and seeds RBAC + dev data on startup.

### Option B — On the host (against local SQL Server)

Requires .NET 8 SDK and a local SQL Server (the default connection string uses Windows Auth):

```bash
cd backend
dotnet run --project src/ProjectTracker.Api
```

The connection string lives in `src/ProjectTracker.Api/appsettings.json`
(`Server=localhost;Database=ProjectTracker;Trusted_Connection=True;TrustServerCertificate=True`).
Put anything sensitive in `appsettings.Development.json` (gitignored) or environment variables.

### Dev users

In Development (and the Docker stack), five users are seeded in one org, all with password
`Passw0rd!`: `owner@acme.test`, `pm@acme.test`, `lead@acme.test`, `dev@acme.test`,
`tester@acme.test`.

## Tests

```bash
cd backend
dotnet test                     # unit + integration
```

Integration tests run against a dedicated `ProjectTracker_Test` database via
`WebApplicationFactory` on real SQL Server (needed to exercise the tenant query filter, rowversion
concurrency, cascade rules, and the sync outbox).

## Docs

- `docs/erd.md` — entity-relationship diagram
- `docs/api.md` — endpoint summary (the live OpenAPI doc is at `/swagger/v1/swagger.json`)
- `docs/deployment.md` — deployment notes and configuration
