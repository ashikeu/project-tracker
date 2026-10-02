# API Reference

All routes are prefixed with `/api/v1`. Every response uses the envelope
`{ "success": bool, "data": ..., "message": string?, "errors": [] }`. All endpoints except the
`auth` ones require a `Authorization: Bearer <accessToken>` header; most mutations additionally
require a specific permission (policy per permission key). The live OpenAPI document is served at
`/swagger/v1/swagger.json` (Swagger UI at `/swagger` in Development).

Every list endpoint is paginated (`page`, `pageSize`; `pageSize` capped at 200, sync at 500).

## Auth  `/auth`  *(anonymous; rate-limited 30/min per IP)*
`POST register` · `POST login` · `POST refresh-token` · `POST logout` ·
`POST forgot-password` · `POST reset-password` · `POST change-password` · `POST verify-email`

## Organizations  `/organizations`
`GET/PUT {id}` · `GET {id}/members` · `POST {id}/invitations` · `GET {id}/roles`

## Teams  `/teams`
`GET/POST` · `GET/PUT/DELETE {id}` · members + roles · archive · (soft-delete)

## Projects  `/projects`
`GET/POST` · `GET/PUT/DELETE {id}` · `POST {id}/archive` · members · teams · labels
(`?includeArchived=true` reveals archived; `?search=`, `?status=`)

## Epics  `/epics`
`GET?projectId=` · `GET/POST` · `PUT/DELETE {id}`  (delete blocked while tasks reference it)

## Tasks  `/tasks`
`GET` (filters: `projectId, epicId, sprintId, status, priority, taskType, assigneeUserId,
reporterUserId, teamId, labelId, dueBefore/After, search, unassignedOnly, topLevelOnly, sortBy,
sortDescending`) · `GET/POST` · `PUT/DELETE {id}` ·
`PATCH {id}/status` (board move) · `PATCH {id}/assign` ·
`POST/DELETE {id}/watchers` · `POST/PUT/DELETE {id}/checklist` ·
`POST/DELETE {id}/labels/{labelId}` · `POST {id}/dependencies` · `DELETE dependencies/{id}` ·
`POST bulk` (assign/status/priority/epic/sprint/label) · `GET {id}/activity`

## Sprints  `/sprints`
`GET?projectId=&status=` · `GET/POST` · `PUT/DELETE {id}` ·
`POST {id}/start | /complete | /cancel` · `GET {id}/burndown`

## Backlog  `/backlog`
`GET?projectId=` · `POST reorder`

## Boards  `/boards` (columns via `/tasks/{id}/status` for moves)
`GET/POST/PUT/DELETE` board columns · reorder

## Comments
`GET/POST /tasks/{taskId}/comments` · `PUT/DELETE /comments/{id}`  (threaded one level; mentions)

## Attachments
`GET/POST /tasks/{taskId}/attachments` (multipart) · `GET /attachments/{id}/download` ·
`DELETE /attachments/{id}`  (size + MIME validated)

## Time tracking
`GET/POST /tasks/{taskId}/time-entries` · `GET /tasks/{taskId}/time-entries/summary` ·
`POST /tasks/{taskId}/time-entries/start` · `POST /time-entries/stop` ·
`PUT/DELETE /time-entries/{id}`

## Notifications  `/notifications`
`GET?unreadOnly=` · `GET unread-count` · `POST {id}/read` · `POST read-all` ·
`GET/PUT preferences`

## Dashboard & Reports
`GET /dashboard/{projectId}` ·
`GET /reports/sprint/{sprintId}` · `GET /reports/workload?projectId=` ·
`GET /reports/epic-progress?projectId=` · `GET /reports/time-tracking?projectId=&from=&to=` ·
`GET /reports/team-productivity?projectId=&from=&to=` · `GET /reports/completion?projectId=&from=&to=`

## Search  `/search`
`GET ?q=&types=` (types: `projects,epics,tasks,users,comments`; exact task-key ranked first)

## Saved filters  `/saved-filters`
`GET ?scope=&projectId=` · `POST` · `DELETE {id}`

## Sync  `/sync`
`POST push` (batch ≤200, idempotent by OperationId; per-op `Applied | Conflict | Rejected`) ·
`GET pull?since=&pageSize=` (delta feed; tombstones for deletes; `nextVersion` + `hasMore`)

## Health
`GET /health` (includes a DB check)
