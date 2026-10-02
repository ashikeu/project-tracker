# Planning — Architecture, Data Model, API, Sync, Permissions

Companion to `CLAUDE.md` (conventions) and `tasks.md` (phase checklist).

## 1. Architecture overview

```
Flutter (GetX)                                  ASP.NET Core Web API 8
────────────────                                 ──────────────────────
presentation/  (screens, GetX controllers)        Api          (Controllers, Middleware, Filters,
        │                                                       Swagger, Auth, HealthChecks)
domain/       (entities, repo interfaces,             │
               usecases)                          Application  (Commands, Queries, DTOs,
        │                                                       Validators, Interfaces, Mappings)
data/         (models, datasources, repos,             │
               sync engine)                       Domain       (Entities, ValueObjects, Enums,
        │                                                       DomainEvents, Specifications,
core/         (network/Dio, storage/secure,                     BusinessRules, Exceptions)
               database/Drift-SQLite)                  │
                                                    Infrastructure (EF Core, Repositories,
        └──────────── HTTPS/JSON, Sync API ───────►                UnitOfWork, FileStorage,
                                                                     Notifications, BackgroundJobs)
                                                         │
                                                    SQL Server
```

Dependency rule: `Api → Application → Domain`, `Infrastructure → Application` (implements its
interfaces) `→ Domain`. Domain never references Application/Infrastructure/Api.

### Application layer pattern (replaces MediatR)

Per feature slice (e.g. `Application/Features/Tasks/Commands/CreateTask`):
- `CreateTaskCommand` (record/DTO input)
- `ICreateTaskHandler { Task<Result<TaskDto>> Handle(CreateTaskCommand cmd, CancellationToken ct); }`
- `CreateTaskHandler : ICreateTaskHandler` — orchestrates validation → domain rules → repository →
  unit of work → domain event publication.
- `CreateTaskValidator : AbstractValidator<CreateTaskCommand>` (FluentValidation, shape only).
- Business rules (e.g. "cannot assign task outside project team") live in
  `Domain/BusinessRules/*Rule.cs` classes with an `IsSatisfiedBy`/`Check` method, invoked from the
  handler.
- Handlers are registered in DI as scoped services; controllers inject the specific handler
  interfaces they need (or a thin per-feature "Facade" service that groups a feature's handlers,
  when a controller needs several). This gives CQRS-style separation without a mediator
  indirection layer.
- Cross-cutting concerns (logging, validation execution, transaction wrapping) are implemented as
  small decorator classes wrapping handlers (manual decorator registration in DI), replacing what
  MediatR pipeline behaviors would normally do.

## 2. Core domain hierarchy

```
Organization → Team → Project → Epic → Task → Subtask
```

A Project also directly owns Sprints, Backlog items, and Labels. A Task owns Subtasks, Comments,
Attachments, TimeEntries, ActivityLogs, Watchers, Labels, Checklists, Dependencies.

## 3. Entity relationship summary (SQL Server / EF Core)

Key tables (all with `Id (uniqueidentifier, PK)`, `CreatedAt`, `CreatedBy`, `UpdatedAt`,
`UpdatedBy`, `RowVersion (rowversion)`; soft-delete tables also get `IsDeleted`, `DeletedAt`,
`DeletedBy`):

```
Organizations (Id, Name, Slug, Status, Settings(json), CreatedAt, ...)
Users (Id, Email(unique), PasswordHash, FirstName, LastName, AvatarUrl, IsEmailVerified,
       IsActive, LastLoginAt, PreferencesJson, ...)
OrganizationUsers (Id, OrganizationId FK, UserId FK, Status, JoinedAt) — unique(OrgId,UserId)
Roles (Id, OrganizationId FK nullable[system roles have null], Name, IsSystemRole)
Permissions (Id, Key unique e.g. "task.assign", Description, Category)
RolePermissions (RoleId FK, PermissionId FK) — composite PK

Teams (Id, OrganizationId FK, Name, Description, LeadUserId FK, IsArchived)
TeamMembers (Id, TeamId FK, UserId FK, Role[Owner/Admin/Member/Viewer]) — unique(TeamId,UserId)

Projects (Id, OrganizationId FK, Key unique-per-org e.g. "ERP", Name, Description, OwnerUserId FK,
          ManagerUserId FK, Status, Priority, StartDate, TargetDate, NextTaskNumber int,
          IsArchived, SettingsJson)
ProjectMembers (Id, ProjectId FK, UserId FK, RoleId FK)
ProjectTeams (ProjectId FK, TeamId FK) — composite PK

Epics (Id, ProjectId FK, Name, Description, OwnerUserId FK, Status, Priority, Color, Icon,
       StartDate, EndDate)

Sprints (Id, ProjectId FK, Name, Goal, Status[Planning/Active/Completed/Cancelled], StartDate,
         EndDate, CompletedAt)

Labels (Id, ProjectId FK, Name, Color) — unique(ProjectId, Name)

BoardColumns (Id, ProjectId FK, Name, Color, Order, WipLimit nullable, StatusMapping,
              IsCompletionColumn)

Tasks (Id, TaskKey unique e.g. "ERP-101", ProjectId FK, EpicId FK nullable, SprintId FK nullable,
       ParentTaskId FK nullable [self-ref for subtasks], Title, Description, TaskType, Status,
       Priority, AssigneeUserId FK nullable, ReporterUserId FK, TeamId FK nullable,
       StoryPoints nullable, EstimatedHours nullable, RemainingHours nullable, LoggedHours,
       DueDate, StartDate, BoardColumnId FK, Position(float, for ordering),
       RecurrenceRuleJson nullable, ClientGeneratedId(guid, nullable — offline-origin tracking))
TaskLabels (TaskId FK, LabelId FK) — composite PK
TaskWatchers (TaskId FK, UserId FK) — composite PK
TaskDependencies (Id, TaskId FK, DependsOnTaskId FK, Type[Blocks/BlockedBy/RelatesTo/Duplicates])
ChecklistItems (Id, TaskId FK, Text, IsDone, Order)

Comments (Id, TaskId FK, AuthorUserId FK, Body, ParentCommentId FK nullable[threading],
          IsEdited, EditedAt)
CommentMentions (CommentId FK, UserId FK) — composite PK

Attachments (Id, EntityType[Task/Comment/Project/Epic], EntityId, FileName, OriginalFileName,
             ContentType, FileSize, StoragePath, UploadedByUserId FK, UploadedAt)

TimeEntries (Id, TaskId FK, UserId FK, Date, DurationMinutes, IsBillable, Description,
             IsRunningTimer, StartedAt nullable)

ActivityLogs (Id, OrganizationId FK, EntityType, EntityId, UserId FK, Action, OldValueJson,
              NewValueJson, CreatedAt) — append-only, no updates/deletes

Notifications (Id, UserId FK, Type, Title, Body, RelatedEntityType, RelatedEntityId, IsRead,
               ReadAt, CreatedAt)
NotificationPreferences (Id, UserId FK, NotificationType, InAppEnabled, PushEnabled)

SyncChanges (Id, OrganizationId FK, EntityType, EntityId, ChangeType[Created/Updated/Deleted],
             ServerVersion bigint identity-per-org, PayloadJson, CreatedAt)
SyncOperations (Id[OperationId from client], OrganizationId FK, UserId FK, EntityType, EntityId,
                Status[Applied/Rejected/Conflict], ProcessedAt) — PK is client OperationId, used
                for idempotency dedup

RefreshTokens (Id, UserId FK, TokenHash, ExpiresAt, RevokedAt nullable, ReplacedByTokenId nullable,
               CreatedByIp)
AuditLogs (Id, OrganizationId FK nullable, UserId FK nullable, Action, Entity, EntityId,
           OldValueJson, NewValueJson, IpAddress, UserAgent, CreatedAt)
```

Indexes: `OrganizationId` on every tenant table, `ProjectId`, `EpicId`, `SprintId`,
`AssigneeUserId`, `Status`, `Priority`, `CreatedAt`, `UpdatedAt`, plus
`(OrganizationId, EntityType, ServerVersion)` on `SyncChanges` for delta pulls.

Cascade-delete policy: parent → child cascades only within a single aggregate's direct ownership
(e.g. Task → Subtask, Task → Comment). Cross-aggregate FKs (e.g. Task.AssigneeUserId → Users) use
`ON DELETE NO ACTION` / `SET NULL` to avoid multiple-cascade-path errors and accidental data loss;
deactivation (soft-delete) is preferred over hard delete for Users/Projects/Teams.

## 4. Permission matrix (summary — full list enforced via policy-per-permission-key)

System roles (org-scoped unless noted): `SuperAdmin` (cross-org, platform ops only),
`OrganizationOwner`, `OrganizationAdmin`, `ProjectManager`, `TeamLead`, `Developer`, `Tester`,
`Member`, `Viewer`.

| Permission key        | Owner | OrgAdmin | PM | TeamLead | Dev/Tester | Member | Viewer |
|---|---|---|---|---|---|---|---|
| organization.manage    | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| project.create/update/delete | ✅ | ✅ | ✅(own) | ❌ | ❌ | ❌ | ❌ |
| project.view            | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| task.create/update       | ✅ | ✅ | ✅ | ✅ | ✅ | ✅(own) | ❌ |
| task.delete               | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| task.assign/move          | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| sprint.create/start/complete | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| comment.create             | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |
| comment.delete (own)        | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |
| attachment.upload/delete    | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |
| time.log/edit(own)           | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |
| user.manage/role.manage       | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |

Full permission key list lives in `Domain/Enums/PermissionKeys.cs` (seeded into `Permissions`
table); policies are registered dynamically from that list at startup, not hardcoded per-policy.

## 5. API endpoint map (v1, all under `/api/v1`)

```
POST   /auth/register
POST   /auth/login
POST   /auth/refresh-token
POST   /auth/logout
POST   /auth/forgot-password
POST   /auth/reset-password
POST   /auth/change-password
POST   /auth/verify-email

GET    /organizations/{id}
PUT    /organizations/{id}
GET    /organizations/{id}/members
POST   /organizations/{id}/invitations
GET    /organizations/{id}/roles

GET    /teams?projectId=&page=&pageSize=
POST   /teams
GET/PUT/DELETE /teams/{id}
POST   /teams/{id}/members

GET    /projects?page=&pageSize=&status=&search=
POST   /projects
GET/PUT/DELETE /projects/{id}
POST   /projects/{id}/archive | /restore
GET/POST /projects/{id}/members
GET/POST /projects/{id}/teams

GET/POST /epics?projectId=
GET/PUT/DELETE /epics/{id}

GET    /tasks?projectId=&epicId=&sprintId=&status=&assigneeId=&page=&pageSize=&sortBy=
POST   /tasks
GET/PUT/DELETE /tasks/{id}
PATCH  /tasks/{id}/status         (board move: status + position + column)
PATCH  /tasks/{id}/assign
POST   /tasks/{id}/watchers
GET/POST /tasks/{id}/comments
GET/POST /tasks/{id}/attachments
GET/POST /tasks/{id}/time-entries
GET    /tasks/{id}/activity
POST   /tasks/{id}/dependencies
POST   /tasks/bulk                (bulk assign/status/priority/sprint/epic/label)

GET/POST /sprints?projectId=
POST   /sprints/{id}/start | /complete | /cancel
GET    /sprints/{id}/burndown

GET    /backlog?projectId=
POST   /backlog/reorder

GET    /boards/{projectId}/columns
PUT    /boards/{projectId}/columns

GET    /notifications?unreadOnly=&page=
POST   /notifications/{id}/read
POST   /notifications/read-all

GET    /dashboard/{projectId}
GET    /reports/sprint/{sprintId}
GET    /reports/workload?projectId=

GET    /search?q=&types=

POST   /sync/push
GET    /sync/pull?since={lastSyncVersion}
```

Response envelope:
```json
{ "success": true, "data": {}, "message": null, "errors": [] }
```
Validation failure → HTTP 422 with `errors: [{ "field": "title", "message": "Title is required" }]`.
List endpoints return `data: { items: [...], page, pageSize, totalCount, totalPages }`.

## 6. Flutter module structure

```
lib/
├── core/ (constants, theme, routes, network/Dio+interceptors, storage/secure, database/Drift,
│          errors/Failure types, utils, widgets/shared)
├── data/ (models[freezed+json_serializable], datasources[remote via Dio / local via Drift],
│          repositories[implement domain repo interfaces, offline-first read/write], sync/
│          [SyncEngine, SyncQueue, ConflictResolver])
├── domain/ (entities, repository interfaces, usecases)
├── presentation/ (auth, dashboard, organizations, teams, projects, epics, backlog, kanban,
│                   sprints, tasks, comments, notifications, profile — each with
│                   screens/ + widgets/ + controllers[GetX] + bindings)
└── main.dart
```

Data flow: `Widget → GetX Controller → UseCase → Repository → (Drift local DB, source of truth
for reads) ⇄ SyncEngine ⇄ Dio → API`. Repositories always read/write local Drift first; the sync
engine reconciles with the server asynchronously. No widget calls Dio or Drift directly.

## 7. Offline sync architecture

```
User Action → Local DB write (Drift) → enqueue SyncOperation(OperationId=uuid, EntityType,
EntityId, OperationType, PayloadJson, Status=Pending)
        │
   connectivity_plus reports online?
        │ yes                                  │ no
        ▼                                       ▼
 SyncEngine drains queue in order    stays Pending, retried on
 → POST /sync/push (batch, idempotent   reconnect / periodic timer
   by OperationId)
        │
 Server: for each op — if OperationId already in SyncOperations → return prior result (no-op);
 else apply within a transaction, bump SyncChanges.ServerVersion, write SyncOperations row.
        │
 Response per op: Applied | Conflict(serverVersion, serverPayload) | Rejected(reason)
        │
 Client: Applied → mark Synced, update local RowVersion.
         Conflict → default "server wins" merge for non-conflicting fields, flag record
           NeedsReview for fields that diverged; user can override (client wins) from the
           conflict UI. Never silently discard local edits — they're kept in a Conflict bucket
           until resolved.
         Rejected → surface to user, keep in Failed bucket (not silently dropped).
```

Pull side: client calls `GET /sync/pull?since=LastSyncVersion`, server returns
`SyncChanges` rows with `ServerVersion > since` for the user's organization (delta only, paged),
including tombstones for deletes. Client applies them to Drift, then advances
`LastSyncVersion` to the max `ServerVersion` received.

Retry policy: exponential backoff (e.g. 5s, 15s, 60s, 5m, capped), max retry count before moving
an operation to `Failed` for manual user intervention — never infinite silent retry.

Offline entity IDs: all entities use client-generatable `Guid` PKs, so offline-created records
need no server round trip to get an ID. Human-readable `TaskKey` (`ERP-101`) is assigned
server-side transactionally on first successful sync of a new Task (using
`Projects.NextTaskNumber`), and pushed back to the client in the Applied response.

## 8. Implementation phases

See `tasks.md` for the actionable, checkable version of these phases. Order: Phase 1 (solution
scaffolding: Api/Application/Domain/Infrastructure/Shared projects, Serilog, Swagger, health
checks, global exception middleware, API versioning) → Phase 2 (Auth + Organization management +
RBAC seed) → Phase 3 (Teams + Projects) → Phase 4 (Epics/Tasks/Subtasks) → Phase 5 (Kanban board +
drag/drop endpoints) → Phase 6 (Sprints + Backlog) → Phase 7 (Comments/Attachments/Time tracking)
→ Phase 8 (Notifications + Activity history) → Phase 9 (Dashboard/Reporting) → Phase 10 (Flutter
offline DB + sync engine) → Phase 11 (Conflict resolution + concurrency) → Phase 12 (Testing,
security, performance pass) → Phase 13 (Docker + deployment docs).

## 9. Architectural risks & mitigations

- **Cascade-delete cycles** across the deep entity graph → mitigated by restricting cascade to
  direct aggregate ownership only (see §3); cross-aggregate FKs use `NO ACTION`/`SET NULL`.
- **N+1 queries on task lists with labels/watchers/subtask counts** → mitigated with projection
  DTOs built via `Select()` in the query layer (not lazy-loaded navigation properties), and
  `AsNoTracking()` everywhere on the read side.
- **Sync conflict storms on high-churn tasks (e.g. Kanban drag reordering)** → `Position` uses a
  fractional-index scheme (float/string-based) so concurrent reorders rarely collide, and full
  reindexing is a background job, not a sync-blocking operation.
- **Offline-created duplicate submissions on flaky connections** → mitigated by mandatory
  client-generated `OperationId` idempotency (see §7); server is the dedup authority.
- **Multi-tenant data leakage** → mitigated by a single EF Core global query filter sourcing
  `OrganizationId` from the authenticated claim, applied to every tenant entity, never from
  client-supplied parameters.
- **Flutter performance with 1,000+ tasks** → local Drift queries are paginated/indexed the same
  way the API is; UI uses virtualized/lazy lists, never loads a full project's tasks into memory.
