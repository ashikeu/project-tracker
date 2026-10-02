# Project Tracker — Flutter Client (offline-first)

Cross-platform (Android / iOS / Web / Windows) client for the Project Tracker API, built
offline-first: the local SQLite (Drift) database is the source of truth for reads, every task
mutation is applied locally first, and a background sync engine reconciles with the server.

## Stack

- **State / routing**: GetX
- **Local DB**: Drift (SQLite) — `lib/core/database`
- **Networking**: Dio with auth + refresh-token interceptors — `lib/core/network`
- **Secure tokens**: flutter_secure_storage (never in Drift/prefs)
- **Connectivity**: connectivity_plus (reactive online/offline)

## Architecture (see `../planning.md` §6–7)

```
Widget → GetX Controller → Repository → Drift (source of truth)  ⇄  SyncEngine ⇄ Dio → API
```

- `lib/core` — constants, theme, routes, DI, Drift database, Dio client, services, shared widgets
- `lib/data` — models (hand-written DTOs), remote datasources, repositories, `sync/` (SyncEngine)
- `lib/domain` — enums mirroring the backend
- `lib/presentation` — `auth`, `projects`, `board` (Kanban), `task`, `sync` (Sync Center)

### Offline-first model

- **Tasks** are the one offline-editable entity (matches the backend `/sync` scope). Create/edit/delete
  write to Drift immediately and enqueue an operation in the `SyncQueueItems` outbox.
- **SyncEngine** pushes the outbox (batched, idempotent by a per-change `OperationId`) then pulls the
  server delta (`/sync/pull?since=`), advancing a local watermark. It runs on reconnect and every 30s.
- **Conflicts** (stale RowVersion) come back per-op; the client applies server-wins by default, flags the
  task for review, and keeps the user's version in the conflict bucket (Sync Center) for a "Keep mine"
  override. Local edits are never silently dropped.
- **Projects / board columns / users** are online-read and cached to Drift for offline display only.

## Run

The API must be reachable (default `http://localhost:5250`, or `http://10.0.2.2:5250` on the Android
emulator). Override with `--dart-define=API_HOST=http://<host>:<port>`.

```bash
flutter pub get
dart run build_runner build          # generates Drift code (app_database.g.dart)
flutter run -d windows               # or -d chrome / an Android device
```

Dev login is pre-filled: `pm@acme.test` / `Passw0rd!` (from the backend's dev seeder).

## Tests

```bash
flutter test                         # unit/outbox tests (always run)
```

`test/live_api_test.dart` additionally exercises the real client networking/sync code against a running
backend (login → offline create → idempotent replay → delta pull → concurrency conflict). It
**auto-skips** if the API isn't reachable, so it's safe in offline CI. Start the backend first to run it
for real.

## Notes / deferrals

- Models are hand-written (not freezed/json_serializable) so codegen is limited to Drift — a contained,
  documented deviation from planning.md §6.
- Screens shipped: Login, Projects, Kanban board (drag/drop), Task detail, Sync Center. Dashboard,
  Backlog, Sprints, Comments, Attachments, Time tracking, Notifications and Profile are deferred (their
  backend endpoints already exist).
