# Tasks — Phase Checklist

Source of truth for progress across sessions. Check items off as completed; add sub-items as
discovered. Build order is backend-first (see `planning.md` §8).

## Phase 1 — Architecture & Project Setup
- [x] planning.md: architecture, ERD, API map, Flutter structure, sync design, permission matrix
- [x] tasks.md created
- [x] CLAUDE.md created
- [x] Backend solution + 5 projects (Api/Application/Domain/Infrastructure/Shared) created, referencing correctly
- [x] Test projects created (UnitTests, IntegrationTests) — scaffolded, no tests written yet (see Phase 2 item below)
- [x] NuGet packages installed (EF Core/SqlServer, FluentValidation, Serilog, Mapster, Swashbuckle, JWT Bearer, HealthChecks)
- [x] Serilog structured logging wired (console + file sink, CorrelationId enrichment)
- [x] Global exception handling middleware
- [x] API versioning configured
- [x] Swagger/OpenAPI with JWT auth button configured (verified live at /swagger)
- [x] Health check endpoint (`/health`) incl. DB check (verified returns "Healthy")
- [x] appsettings.json: connection string = `Server=localhost;Database=ProjectTracker;Trusted_Connection=True;TrustServerCertificate=True` (Windows Auth), JWT config section
- [x] Solution builds clean

## Phase 2 — Authentication & Organization Management
- [x] Domain entities: User, Organization, OrganizationUser, Role, Permission, RolePermission, RefreshToken
- [x] EF Core DbContext + initial migration (applied to local SQL Server via Windows Auth)
- [x] Global query filter for tenant isolation (OrganizationId from claims) — with documented
      `.IgnoreQueryFilters()` exception in Login/RefreshToken (see CLAUDE.md)
- [x] Password hashing (ASP.NET Core Identity's `PasswordHasher<T>`, PBKDF2-HMAC-SHA256)
- [x] JWT issuance (access + refresh token rotation), claims: UserId, OrganizationId, Role, Permissions
- [x] Auth endpoints: register, login, refresh-token, logout, forgot/reset/change-password, verify-email
- [x] Organization endpoints: get/update, members, invitations, roles
- [x] Permission-based authorization policies generated from seeded Permissions table
- [x] RBAC seed: system roles + permission list + role-permission mappings (idempotent, runs on every startup)
- [x] Dev seed data: 1 org, 5 users across different roles (Development environment only)
- [ ] Unit + integration tests for auth flows and tenant isolation — manually verified via curl
      (register, login, refresh, cross-tenant 403, validation 422); automated test suite still TODO
- [x] Build + tests pass (build passes; automated tests not yet written)

**Manually verified end-to-end**: register → org+user+RBAC created and JWT issued; dev-seeded
user login across all 5 roles; protected org endpoints (get/members/roles) enforce JWT +
permission policies; cross-tenant access to another org's data returns 403; FluentValidation
failures return 422 in the standard envelope; Swagger UI live with JWT auth button.

## Phase 3 — Teams & Projects
- [x] Team CRUD + members + roles (Owner/Admin/Member/Viewer) + archive + soft-delete
- [x] Project CRUD + unique project Key (per-org, validated) + members + teams + archive/restore + soft-delete
- [x] Project NextTaskNumber field added to entity (sequence logic itself lands in Phase 4 when tasks are created)
- [ ] Automated tests — manually verified via curl instead (see below); still TODO before/at Phase 12

**Manually verified end-to-end**: create/update team & project; add/remove team members (with
"must keep one Owner" rule enforced); add/remove project members and project-team links;
duplicate project key rejected with 422; archive hides from default list, `includeArchived=true`
reveals it, restore un-archives; delete soft-deletes (global query filter hides it, "not found"
on subsequent fetch); cross-tenant user cannot see another org's team/project.

**Two real bugs found and fixed during this phase** (both now documented in code comments):
1. JWT claim mapping: ASP.NET Core's default inbound claim mapping silently renamed `sub` to a
   legacy XML-schema URI, breaking `ICurrentUserService.UserId` everywhere. Fixed with
   `options.MapInboundClaims = false` in `AddJwtAuthentication`.
2. Tenant query filter regression: refactoring the global query filter to also cover soft-delete
   used `Expression.Constant(_currentUserService.OrganizationId)`, which bakes in whatever value
   was current when EF Core built the model **once** at startup (null, from the seeding scope) —
   freezing every tenant-scoped list query to return nothing, forever. Fixed by reading the value
   through an instance property (`ApplicationDbContext.CurrentOrganizationId`) accessed via
   `Expression.Property` on `Expression.Constant(this)`, which EF Core correctly re-evaluates
   per executing DbContext instance (see the comment on `CurrentOrganizationId`).

## Phase 4 — Epics, Tasks & Subtasks
- [x] Epic CRUD (create/update/delete guarded when tasks still reference it/list/detail)
- [x] Task CRUD (all fields per planning.md §3), TaskKey generation on create (ERP-1, ERP-2, ...)
- [x] Subtask support (ParentTaskId self-reference, one level deep only); "progress" surfaced as
      SubtaskCount/CompletedSubtaskCount on TaskDto rather than auto-mutating parent status
      (documented simplification — see TaskItem.cs)
- [x] Labels (project-scoped CRUD + task association), Watchers, Checklists, Dependencies (with
      cycle prevention via BFS over the existing dependency graph)
- [x] Bulk operations endpoint (assign/status/priority/epic/label; sprint bulk-assign added in Phase 6)
- [ ] Automated tests — manually verified via curl instead; still TODO before/at Phase 12

**Manually verified end-to-end**: create epic → create task (TaskKey assigned) → create subtask
→ add checklist item, watcher, label, dependency → cyclic dependency correctly rejected → bulk
status update across two tasks → filter by status/topLevelOnly → epic delete blocked while it
has tasks → deleting a parent task cascades (soft-delete) to its subtask.

**Naming decision**: the Domain entity is `TaskItem`, not `Task` — see CLAUDE.md "Naming" for why
(`System.Threading.Tasks.Task`/`TaskStatus` are implicitly in scope everywhere via
`ImplicitUsings` and would collide with every single `async Task<...>` method signature).

**Cascade-path fix**: `Epic → Task` (EpicId) had to become `Restrict` instead of `SetNull` — SQL
Server treats `SET NULL` as a cascading action too, and `Project → Task` already cascades
directly, so both together created the classic "multiple cascade paths" error via
`Project → Epic → Task`. The app now blocks deleting an epic that still has tasks instead.

**Tenant isolation hardened**: `Epic` and `TaskItem` got a denormalized `OrganizationId` column
(copied from `Project.OrganizationId` at creation) so they're `ITenantEntity` in their own right,
protected by the same global query filter as `Team`/`Project`/`OrganizationUser` — not just
implicitly safe via a `ProjectId` join that a future query could forget.

## Phase 5 — Kanban Board & Drag/Drop
- [x] BoardColumns CRUD (name/color/order/WIP limit/status mapping/completion flag) + reorder
      endpoint; every new Project auto-seeds the 6 default columns from planning.md §12
- [x] Task status/position update endpoint (PATCH /tasks/{id}/status) with fractional-index
      positioning, WIP-limit enforcement, and self-healing reindex when positions get too close
- [x] Board filters (assignee/priority/epic/label/type/status) — already covered by GetTasksHandler
      from Phase 4; sprint filter added in Phase 6 (GetTasksQuery.SprintId)
- [ ] Automated tests (incl. concurrent reorder) — manually verified via curl instead; still TODO

**Manually verified end-to-end**: new project auto-seeds 6 columns → create tasks → drag three
tasks into the same column at different indexes and confirm fractional positions land exactly
between their neighbors (0, 512, 1024) → set a WIP limit and confirm a 4th task is rejected →
reorder columns → delete an empty column (succeeds) → delete a column with tasks (blocked).

**Design note**: `BoardColumn → Task` (BoardColumnId) is `Restrict`, same cascade-path reasoning
as `Epic → Task` in Phase 4 — the app blocks deleting a column that still has tasks in it rather
than relying on the database to cascade/null them out.

## Phase 6 — Sprints & Backlog
- [x] Sprint CRUD + state machine (Planning→Active→Completed/Cancelled) with invalid-transition guards
      (guard lives in Domain/BusinessRules/SprintStatusTransition, invoked from handlers per CLAUDE.md)
- [x] Backlog listing + reorder + sprint/epic assignment (backlog = top-level tasks with no Sprint;
      reorder via same fractional-index approach as the Kanban board; sprint/epic assignment via the
      extended /tasks/bulk endpoint — no duplicate single-purpose endpoints)
- [x] Sprint velocity calculation (committed vs completed story points surfaced on SprintDto)
- [ ] Automated tests incl. invalid transitions — manually verified via curl instead; still TODO before/at Phase 12

**Manually verified end-to-end**: create sprint → complete-while-Planning rejected (invalid transition)
→ start (Planning→Active) → start-again rejected ("already Active") → 2nd sprint start rejected
("project already has an active sprint") → bulk-assign backlog task to active sprint → assign-to-
cancelled-sprint rejected (422) → velocity totals (committed 13 / completed 8 SP) → complete with
moveIncompleteToBacklog=true: sprint Completed + CompletedAt set, incomplete task returned to backlog
(SprintId null), Done task retained on the completed sprint → backlog reorder → delete sprint with
tasks rejected, delete empty sprint allowed.

**Decisions**:
- **One active sprint per project**: StartSprint rejects a start when another sprint in the project is
  already Active (Scrum invariant; not spelled out in planning.md but a natural state-machine guard).
- **No silent task mutation on complete**: completing a sprint moves incomplete tasks back to the
  backlog only when the caller opts in (`moveIncompleteToBacklog`, default true); Done tasks always
  stay on the sprint as history. Cancelling always returns all its tasks to the backlog.
- **Sprint FK cascade**: `Sprint → Task` (SprintId) is `Restrict` (same "multiple cascade paths via
  Project" reasoning as Epic/BoardColumn); handlers reassign tasks off a sprint rather than DB-cascade.
- **Backlog/board share TaskItem.Position**: backlog ordering reuses the existing fractional Position
  field, scoped to (project, SprintId==null, top-level) — consistent with the field's documented
  "unique only within its current ordering scope" contract.
- Sprint status changes go only through start/complete/cancel; UpdateSprint edits descriptive fields
  only, so SprintStatusTransition is the single gate for status.

## Phase 7 — Comments, Attachments, Time Tracking
- [x] Comments (threaded one level, mentions, edit/delete). Mentions are supplied explicitly by the
      client (`mentionedUserIds`), validated as org members, persisted to CommentMentions —
      notification *generation* on mention is deferred to Phase 8 (the mention data it consumes is
      now captured). Edit is author-only + sets IsEdited/EditedAt; delete is author-or-comment.delete.
- [x] IFileStorageService abstraction (Application) + LocalFileStorageService (Infrastructure,
      date-sharded, path-traversal-guarded, opaque server-generated StoragePath)
- [x] Attachment upload/list/download/delete with size + MIME validation (via IFileUploadPolicy over
      FileStorageSettings). Polymorphic (Task/Comment/Project/Epic); task-scoped upload/list wired,
      download/delete by id. Metadata row is deleted before the blob so no dangling rows.
- [x] TimeEntries (start/stop timer, manual entry, billable flag). Task LoggedHours/RemainingHours
      are *recomputed* from the sum of finalized entries on every mutation (never incrementally
      patched) so they can't drift; summary endpoint exposes estimated/logged/remaining.
- [ ] Automated tests — manually verified via curl instead; still TODO before/at Phase 12

**Manually verified end-to-end**: comment with mention (mention resolved to member name) → reply →
reply-to-a-reply rejected (one level deep) → author edit sets IsEdited and re-syncs mentions →
threaded list returns top-level threads with nested replies. Upload text file → list → download
returns bytes → delete removes row + blob (list count drops). Manual 90-min entry → summary logged
1.5h / remaining 8.5h (est 10h) → start timer (running, StartedAt set) → second start rejected (one
running timer per user) → stop (duration computed, min 1 min) → second stop rejected → non-owner
edit rejected → owner edit 90→120 recomputes summary → delete recomputes summary.

**Decisions**:
- **Explicit mentions, not body-parsing**: the client sends `mentionedUserIds` from its @-picker
  rather than the server regex-parsing Body — unambiguous and server-validatable. Editing a comment
  replaces its mention set.
- **Comments soft-deleted, attachments/time-entries hard-deleted**: comments keep thread/mention
  history (BaseSoftDeleteEntity); an attachment hard-delete also removes the stored blob, and time
  entries are transient log rows, so both are removed outright.
- **Derived task hours**: LoggedHours/RemainingHours are always recomputed from the entry set
  (TaskHoursRecalculator), never `+=`'d — the single source of truth is the entries.
- **One running timer per user** (across all tasks): a second concurrent timer would make "how long
  am I tracking" ambiguous; StopTimer finds the user's single running entry.
- **IFileUploadPolicy** abstracts size/MIME limits so Application validates uploads without
  depending on Infrastructure's FileStorageSettings type. Empty AllowedContentTypes = allow-any
  (dev); Phase 12 hardens the allow-list. Uploads root (App_Data/) is gitignored.
- **Attachment FK**: polymorphic (EntityType + EntityId), so no FK to the owning entity; existence +
  tenant ownership is checked in the handler per type. Comment→Task cascades; ParentComment
  self-ref is Restrict (no cascade cycle).

## Phase 8 — Notifications & Activity History
- [x] ActivityLogs written on significant task mutations (immutable, append-only) via IActivityLogger:
      Created, Updated, Assigned, StatusChanged (board move), Deleted, CommentAdded. Old/new JSON
      captures just the changed fields. Surfaced at GET /tasks/{id}/activity (tenant-scoped, paginated).
- [x] Notification generation: TaskAssigned (→assignee), TaskStatusChanged (→watchers+assignee, from
      both board move and edit-form status change), CommentMention (→mentioned users), CommentAdded
      (→watchers+assignee, deduped against the mention set), TaskDueSoon (background job). Actor is
      never self-notified; per-user/per-type preferences honored (opt-out model, default on).
- [x] Firebase Cloud Messaging integration for push — IPushSender seam + FcmPushSender. Dev logs the
      payload when FCM unconfigured (SmtpEmailService pattern); real device delivery needs device-token
      registration from the mobile client (Phase 10), so it logs a warning rather than silently
      claiming to send when configured-but-no-tokens. This is the single wire-in point for real FCM.
- [x] Notification endpoints: GET /notifications (unreadOnly, paged), GET /unread-count,
      POST /{id}/read, POST /read-all, GET/PUT /notifications/preferences
- [x] Due-soon background job (BackgroundService, hourly) — notifies assignees of tasks due within 24h,
      deduped per task+assignee within a 24h window
- [ ] Automated tests — manually verified via curl instead; still TODO before/at Phase 12

**Manually verified end-to-end**: PM creates task (due tomorrow) → assigns to dev (dev gets
TaskAssigned) → edits status (dev gets TaskStatusChanged) → comments mentioning dev (dev gets
CommentMention) → due-soon job fires ~30s after startup (dev gets TaskDueSoon). Activity feed shows
Created/Assigned/Updated/CommentAdded with actor + changed-field JSON. Dev mutes TaskStatusChanged →
next status change produces no new notification (count stays 1). Preferences list returns all types
with defaults + the override. Mark-one-read, mark-all-read (returns count), unread-count all correct.
PM (the actor) received 0 of the notifications (self-exclusion).

**Decisions**:
- **Notifications are per-user, not tenant entities**: isolation is by the authenticated UserId
  (handlers always filter by it); no cross-user read path, so no OrganizationId/global filter needed.
  ActivityLog *is* an ITenantEntity (org-scoped feed).
- **Opt-out preferences**: a missing NotificationPreference row means "on" for both channels; a row is
  written only when the user changes a default. GET /preferences fills in defaults for every type.
- **Dispatcher owns fan-out + prefs + push + self-exclusion**; handlers just name recipients and the
  message. Push failures are swallowed (a dead token must not fail the originating request).
- **CommentAdded deduped against mentions**: a mentioned watcher gets only the CommentMention, not
  both (mentions dispatched first, then watchers/assignee minus the mention set).
- **Due-soon job runs cross-tenant**: no OrganizationId claim in a background scope would make the
  tenant global filter return nothing, so the job uses IgnoreQueryFilters() and re-applies not-deleted
  by hand — the same "no ambient tenant" situation as the RBAC/dev seeders.
- **Activity logging is best-effort & post-commit**: logged after the mutation's SaveChanges, and a
  missing org scope skips the log rather than throwing — logging never fails a mutation.
- **FCM real-send deferred to Phase 10**: the abstraction and dispatch path are complete and exercised;
  actual device delivery is blocked on device-token registration from the client (honest seam, not a
  placeholder — it's a working logging implementation, like the SMTP one).

## Phase 9 — Dashboard & Reporting
- [x] Dashboard aggregation endpoint GET /dashboard/{projectId}: task counts (total/open/done/overdue),
      status/priority/type distributions, per-assignee workload, epic progress, active-sprint summary
      (top-level tasks only, so numbers match the board/backlog)
- [x] Burndown endpoint GET /sprints/{id}/burndown: ideal (linear) vs actual remaining story points per
      day. Actual line reconstructed from ActivityLog (date each task reached Done), not just the
      current snapshot; days beyond today are null
- [x] Global search GET /search?q=&types=: projects/epics/tasks(+subtasks)/users/comments, exact
      task-key match ranked first, capped per type, tenant-scoped, 2-char minimum
- [x] Saved filters: SavedFilter entity + CRUD (GET/POST/DELETE /saved-filters). Criteria stored as
      opaque client-defined FilterJson (validated as well-formed JSON), owner-scoped
- [x] Reports (GET /reports/...): sprint/{id}, workload, epic-progress, time-tracking, team-productivity,
      completion. Time-bounded reports default to the last 30 days; "completed" work is reconstructed
      from ActivityLog Done-transitions
- [ ] Automated tests — manually verified via curl instead; still TODO before/at Phase 12

**Manually verified end-to-end**: dashboard returns correct counts/distributions/workload/epic-progress
for a project. Search: `q=ERP-5` returns the exact task first; `q=ledger&types=tasks,epics` returns
matching tasks + epics and no projects (type filter honored); 1-char query rejected. Burndown on a
past-dated sprint with a task completed today shows actual 5→5→0 across days and null for future days,
ideal declining linearly. Sprint report 8/8 pts = 100%. Workload/epic-progress/time-tracking/
team-productivity/completion all return correct shapes. Saved filter create (valid JSON) → list →
delete; invalid JSON rejected 422.

**Decisions**:
- **Reports/dashboard/search gated on `project.view`**; saved filters just require auth (owner-scoped).
  No new permission keys were needed.
- **History-accurate metrics via ActivityLog**: burndown's actual line, completion "completed" counts,
  and team-productivity credits all derive from Done-transition activity logs (matching
  `"Status":"Done"` in NewValueJson) rather than the current row state — so they reflect *when* work
  finished and *who* finished it. Caveat: tasks marked Done before Phase 8 (when activity logging
  landed) have no such log and won't appear in these history views — acceptable for pre-existing data.
- **SavedFilter.FilterJson is opaque**: the server validates it's well-formed JSON and stores/returns
  it verbatim, so new client-side filter fields never require a backend change.
- **Search caps at 10 per type** (not paginated): a global search box wants a quick top-N per category,
  not deep pagination — drilling in uses the type-specific paginated list endpoints (e.g. GET /tasks).

### ✅ MILESTONE: Backend feature-complete (Phases 2–9). All core API surface implemented and
manually verified via curl. Remaining backend work is the automated test suite (Phase 12) and
hardening (rate limiting, CORS, security headers, perf pass). Phases 10–11 are the Flutter client +
conflict resolution; Phase 13 is Docker/deploy.

## Phase 9.5 — Backend Sync Endpoints (server side of Phases 10–11)
Built ahead of the Flutter client per CLAUDE.md ("solidify sync endpoints before building Flutter
screens"). Fully curl-verified.
- [x] SyncChanges + SyncOperations tables/entities (+ SyncChangeType/SyncOperationStatus enums)
- [x] Change-feed outbox: ApplicationDbContext.SaveChangesAsync emits a SyncChange row for every
      insert/update/soft-delete of an ISyncableEntity, inside the same transaction as the mutation.
      ServerVersion is a DB IDENTITY (globally monotonic delta cursor). TaskItem is the marked
      syncable entity; adding more = mark the entity + add a payload builder (extension point).
- [x] GET /sync/pull?since=&pageSize= — delta feed of the caller's org changes with ServerVersion >
      cursor, ordered, paged (HasMore + NextVersion), tombstones (null payload) for deletes
- [x] POST /sync/push — batched (≤200), idempotent by OperationId (duplicate replays stored result),
      TaskItem create/update/delete. Create assigns TaskKey server-side transactionally; update/delete
      use optimistic concurrency via base64 RowVersion → per-op Conflict result carrying the server
      payload (never silently overwrites newer data). Unsupported entity types → Rejected.
- [ ] Automated tests — manually verified via curl instead; still TODO before/at Phase 12

**Manually verified end-to-end**: pull baseline watermark → normal-API task create emits a SyncChange
→ push an offline-created task (client GUID) returns Applied + server-assigned TaskKey (ERP-8) +
RowVersion → re-push same OperationId replays identical result (no duplicate row) → delta pull since
watermark sees the new task → push update with stale BaseRowVersion → Conflict + server payload →
push update with correct RowVersion → Applied → push delete → Applied, delta pull shows Created then
Deleted tombstone (null payload), and GET returns not-found → unsupported entity type ("Comment") →
Rejected with clear message.

**Decisions**:
- **Global-monotonic ServerVersion** (a single DB IDENTITY, not per-org): the client only reads its
  own org's rows above its watermark, so a global sequence is a correct, simpler, gap-tolerant cursor
  (planning.md said "identity-per-org"; global is operationally equivalent for the client and avoids
  per-org increment contention).
- **Outbox in SaveChanges, atomic with the mutation**: when any ISyncableEntity is dirty, the save
  wraps the mutation + its change rows in one transaction (change rows written post-mutation-save so
  they capture DB-generated RowVersion/TaskKey and their ServerVersion identity). No syncable changes
  → plain single save, zero overhead.
- **Conflicts are per-op results, not HTTP 409**: in a batch push, each op independently returns
  Applied | Conflict(serverPayload) | Rejected(reason) with HTTP 200 for the batch — the batched
  analogue of the spec's "409 with server+client payload". The client's Phase 10/11 conflict bucket
  consumes these per-op results.
- **Scope = TaskItem** (the offline-editable entity); the wire format carries EntityType so comments/
  time-entries/etc. can be added behind the same endpoint later without an API change.

## Phase 10 — Flutter Offline Database & Sync Engine
Flutter app lives in `mobile/` (Flutter 3.47 / Dart 3.13). Clean analyze (0 issues), 10 tests green
(8 local + 2 live-API), native Windows build succeeds. **Core offline-first vertical slice is complete
and verified end-to-end against the live backend**, plus Dashboard / Notifications / Profile screens in
a bottom-nav shell; some secondary screens remain deferred (see note).
- [x] Flutter project scaffold (lib/core, data, domain, presentation per planning.md §6)
- [x] Drift schema (LocalTasks + SyncQueueItems outbox + ConflictItems + read-caches for
      projects/columns/users + SyncMeta watermark). Enum fields stored as ints to match the sync wire.
- [x] Dio client + interceptors: bearer-attach + transparent 401 refresh-token rotation + replay;
      session-expired callback bounces to login. Typed Failure mapping (network/server/validation/auth).
- [x] flutter_secure_storage for tokens (never in Drift/prefs — honors the "no tokens in non-secure
      storage" rule)
- [x] connectivity_plus wiring + online/offline UI (OfflineBanner + SyncStatusChip app-bar indicator)
- [x] Repository layer: local-first reads (Drift is source of truth, streamed to the UI); writes go to
      Drift + enqueue a SyncQueue op. Projects/board-columns are online-read + cached (not sync-scoped).
- [x] SyncEngine: push (batched ≤200, idempotent via a fresh OperationId per logical change) + pull
      (delta via LastSyncVersion watermark, paged, tombstone-aware). Mutex-guarded single drain;
      pull won't clobber rows with un-pushed local edits.
- [x] Retry → Failed bucket: reconnect-trigger + 30s periodic heartbeat + **exponential-backoff
      schedule** (5s/15s/60s/5m, capped) scheduled after each failed sync, reset on success/reconnect;
      Rejected ops land in a Failed bucket (never infinite silent retry). `backoffFor()` is unit-tested.
- [x] Screens: Login, Projects (list/create), Kanban board (drag/drop across columns, WIP
      indicator, add task), Task detail (full edit + delete + sync-state badge), Sync Center
      (pending/failed/conflicts), **Dashboard** (KPIs + status/priority distributions + epic progress +
      workload + active-sprint), **Notifications** (list + unread badge + mark read/all), **Profile**
      (user + connection + sign-out), wrapped in a **bottom-nav home shell** (Projects/Alerts/Profile).
      **Now also done** (previously deferred): **Sprints** (list by status + create/edit + start/
      complete/cancel/delete via the server state machine + velocity), **Backlog** (drag-reorder via
      `/backlog/reorder` + assign-to-sprint via `/tasks/bulk`), **Comments** (threaded one level,
      post/reply/edit/delete), **Attachments** (list + upload via `file_picker` + download-to-documents
      + delete), **Time tracking** (summary + entries + manual log + start/stop timer + edit/delete).
      Sprints/Backlog open from the board app-bar menu; Comments/Attachments/Time open from Task detail
      (guarded until the task has been synced once, since these live only on the server).
- [x] Optimistic UI: every mutation reflects instantly from Drift, online or offline; unsynced/review
      state is surfaced per-card and on the detail screen. (A pre-edit *rollback* on hard-reject isn't
      implemented — rejected ops stay in the Failed bucket for the user instead.)
- [~] Tests: repository/outbox unit tests (create/update/coalesce/delete/payload round-trip), a backoff
      unit test, and three live end-to-end tests hitting the real API (sync round-trip incl. idempotent
      replay + concurrency conflict; dashboard + notifications remotes; **and the deferred-screen
      remotes — sprints lifecycle, backlog list/reorder/assign, comments CRUD + nesting, attachment
      upload/download byte round-trip/delete, time manual-log/summary/timer**, in
      `test/live_deferred_screens_test.dart`). Controller/widget tests still TODO. (Live tests auto-skip when the API is down. They exposed a backend concurrent-login 500 —
      now fixed in Phase 12 — so parallel isolates are fine again; `--concurrency=1` is only needed to
      avoid two isolates racing to create the same-named project.)

**Verified end-to-end (live API, `test/live_api_test.dart`)**: login as dev-seeded PM → create project
→ auto-seeded board columns fetched → push an offline-created task (client Guid) returns Applied +
server-assigned TaskKey → re-push same OperationId replays identical result (idempotent, no dup) →
delta pull from 0 sees the task → update with correct RowVersion Applied → update with stale
RowVersion returns Conflict carrying the server payload (server never overwritten).

**Decisions**:
- **Hand-written DTO models, not freezed/json_serializable**: codegen is limited to Drift only, which
  keeps the build fast and removes a whole class of codegen-coupling failures. A documented, contained
  deviation from planning.md §6's "freezed+json_serializable".
- **Sync payload casing**: the client sends camelCase (server deserializes case-insensitively) but the
  server *serializes* sync payloadJson with the default `JsonSerializer` (**PascalCase**), unlike the
  camelCase REST envelope. `TaskPayload.fromJson` is therefore case-insensitive (keys lowercased). This
  was caught by the live test — the unit tests alone would have missed it.
- **Enums as ints in Drift + on the sync wire** (TaskSyncPayload uses ints); the human-readable REST
  TaskDto uses enum strings, but the client's offline path is driven by the sync feed, so ints win locally.
- **Tasks are the only offline-editable entity** (matches the backend /sync scope). Projects, board
  columns and users are online-read, cached to Drift for offline *display* only.
- **One pending op per task, fresh OperationId each change**: the server caches results per OperationId
  and would replay a stale result if an id were reused for new data — so coalescing replaces the pending
  op with a new id rather than reusing it.
- **Sprints/Backlog/Comments/Attachments/Time are online-only** (same pattern as Dashboard/Notifications):
  thin remote → model → GetX controller → screen, no Drift/sync involvement, because none are in the
  backend `/sync` scope (only TaskItem is). The task-scoped screens (Comments/Attachments/Time) are
  therefore gated on the task being server-backed (`syncState != pendingCreate && taskKey != ''`) —
  an offline-created task has no server row yet, so these would 404 until its first push. The @-mention
  picker in Comments is intentionally omitted (mentions stay optional/empty) to avoid org-member/orgId
  plumbing the picker would need; comments are otherwise fully functional.
- **`file_picker` added** for attachment upload (the one new client dependency; Windows/desktop support
  is built in). Downloads stream raw bytes and save under the app documents `attachments/` folder.

## Phase 11 — Conflict Resolution & Concurrency
Conflict handling shipped alongside Phase 10's sync engine.
- [x] RowVersion concurrency: backend already returns a per-op Conflict carrying the server payload
      (Phase 9.5); the client consumes it. Verified by the live test's stale-RowVersion case.
- [x] Client conflict bucket UI (Sync Center): server-wins is applied by default (server payload written
      locally + row flagged NeedsReview, user's version preserved in ConflictItems), with a "Keep mine"
      override that re-applies the local edit on the server's latest RowVersion and re-pushes. Local
      edits are never silently discarded.
- [x] Duplicate-submission prevention (idempotency): verified by the live test (same OperationId replay)
      and the unit test (coalesce keeps one pending op with a fresh id).
- [~] Tests: covered by the repository + live-API tests above; a dedicated conflict-resolution
      controller/widget test is still TODO.

## Phase 12 — Testing, Security & Performance
- [x] Automated test suite (in progress → core coverage green): 25 tests passing.
      - **UnitTests** (11): SprintStatusTransition valid/invalid transitions + messages.
      - **IntegrationTests** (14, real SQL Server test DB via WebApplicationFactory): auth (login,
        wrong-password rejected, register-new-org, unauthorized without token, **concurrent logins all
        succeed** — regression for the LastLoginAt concurrency fix); RBAC (PM can create
        project, Developer/Tester get 403); tenant isolation (org B cannot read or list org A's
        project); sprint state machine (invalid transitions rejected, one-active-sprint rule); sync
        (push create Applied + server TaskKey + idempotent replay, stale-rowversion Conflict, delta
        pull after watermark). More handler-level coverage still worth adding before ship.
- [x] Rate limiting (per-IP global 600/min + stricter 30/min on /auth), CORS (permissive only in Dev;
      deployed envs require configured origins), secure headers (X-Content-Type-Options, X-Frame-Options,
      Referrer-Policy, X-Permitted-Cross-Domain-Policies) — verified: headers present on every response,
      auth endpoint returns 429 after 30 rapid attempts
- [x] File upload validation — size + MIME allow-list enforced via IFileUploadPolicy (Phase 7). Allow-list
      is empty (=allow-any) in dev; production hardening = populate FileStorage:AllowedContentTypes
- [~] Query performance — pagination is enforced on every list endpoint and indexes are defined per
      entity config (OrganizationId/ProjectId/EpicId/SprintId/Status/etc. + the sync delta index); a
      formal N+1/query-plan audit is not yet done
- [ ] Flutter widget/controller tests — deferred to the Flutter phase (Phase 10)

**Test infra**: integration tests run against a dedicated `ProjectTracker_Test` database under a
"Testing" environment (dev seed runs so tests have known users; rate limiting is skipped so the
suite's rapid logins aren't throttled). Tests run serially in one xUnit collection to avoid racing on
the shared DB. `Program` exposes `public partial class Program` for WebApplicationFactory.

**Decisions**:
- **Integration over in-memory**: tests hit real SQL Server, because the behaviors most worth
  testing (tenant global query filter, rowversion concurrency, cascade rules, sync outbox/identity)
  are exactly what EF InMemory can't reproduce faithfully.
- **Rate limiting off under test host**: gated on `!IsEnvironment("Testing")` so the suite's many
  logins don't hit the 30/min auth cap; verified separately that it does throttle in Development.
- **Concurrent-login 500 fixed** (found while running the Flutter live tests in parallel): `User`
  inherits the rowversion concurrency token, so writing `LastLoginAt` inside the login's tracked
  `SaveChanges` made simultaneous logins for the *same* user collide — the first bumped RowVersion and
  the rest threw `DbUpdateConcurrencyException`, surfacing as 500 (the middleware only mapped the
  *domain* `ConcurrencyConflictException` to 409). Fix: (1) persist the refresh token on its own save
  (an INSERT, no token), then stamp `LastLoginAt` in a separate best-effort save whose concurrency
  race is swallowed — a non-critical timestamp must never fail a valid sign-in; (2) defense-in-depth,
  the exception middleware now also maps raw `DbUpdateConcurrencyException` → 409 instead of 500.
  Reproduced (5/6 → 500 before), verified (16/16 → 200 after), and locked in with a regression test.

## Phase 13 — Docker & Deployment
- [x] Backend Dockerfile (`backend/Dockerfile`) — multi-stage (SDK build → aspnet runtime), layer-cached
      restore, listens on :8080, uploads dir created. `backend/.dockerignore` excludes bin/obj/logs/
      App_Data/tests. `dotnet publish` (the build stage's core) validated locally; the image itself
      could not be built here because the Docker **daemon isn't running** (CLI present, engine off).
- [x] docker-compose.yml (root): SQL Server 2022 + API, DB healthcheck + `depends_on: service_healthy`,
      SQL-auth connection string (Windows Auth can't work in a Linux container), persistent volumes for
      DB data and uploads. API migrates + seeds on startup.
- [x] Startup migrate hardened with a bounded retry (12×5s) so a container start that races the DB's
      first boot recovers instead of crashing.
- [x] Docs: `README.md` (overview, features, layout, run local/docker, tests), `docs/erd.md` (Mermaid
      ER diagram + notes), `docs/api.md` (grouped endpoint reference; live OpenAPI at /swagger), and
      `docs/deployment.md` (compose, config/env-var table, production checklist, health/observability).
      Architecture detail continues to live in `planning.md`.

**Verification note**: Dockerfile + compose are written and the publish step is validated, but the
image build / `docker compose up` were NOT run (Docker daemon not started in this environment). Start
Docker Desktop and run `docker compose up --build` to validate the full stack.

---
**Milestone check-ins**: report back to user after Phase 2 (auth working), Phase 5 (Kanban
working end-to-end via API), Phase 9 (backend feature-complete), Phase 10 (mobile app runs
offline-first against the API), Phase 13 (dockerized, deployable). Proceed autonomously between
those points per user's stated preference.
