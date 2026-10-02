# Entity-Relationship Diagram

The core domain model. Every organization-owned table carries `OrganizationId` (tenant isolation via
an EF Core global query filter). Syncable/auditable tables also carry `RowVersion` (optimistic
concurrency) and, where soft-deletable, `IsDeleted`.

```mermaid
erDiagram
    Organization ||--o{ OrganizationUser : has
    User ||--o{ OrganizationUser : "member of"
    Organization ||--o{ Role : defines
    Role ||--o{ RolePermission : grants
    Permission ||--o{ RolePermission : "granted by"
    OrganizationUser }o--|| Role : "assigned"
    User ||--o{ RefreshToken : owns

    Organization ||--o{ Team : owns
    Team ||--o{ TeamMember : has
    User ||--o{ TeamMember : "member of"

    Organization ||--o{ Project : owns
    Project ||--o{ ProjectMember : has
    Project ||--o{ ProjectTeam : links
    Team ||--o{ ProjectTeam : links

    Project ||--o{ Epic : contains
    Project ||--o{ Sprint : contains
    Project ||--o{ Label : defines
    Project ||--o{ BoardColumn : defines

    Project ||--o{ TaskItem : contains
    Epic ||--o{ TaskItem : groups
    Sprint ||--o{ TaskItem : includes
    TaskItem ||--o{ TaskItem : "parent of (subtasks)"
    BoardColumn ||--o{ TaskItem : "holds"

    TaskItem ||--o{ TaskLabel : tagged
    Label ||--o{ TaskLabel : tags
    TaskItem ||--o{ TaskWatcher : watched
    TaskItem ||--o{ ChecklistItem : has
    TaskItem ||--o{ TaskDependency : "depends"
    TaskItem ||--o{ Comment : has
    TaskItem ||--o{ TimeEntry : logs

    Comment ||--o{ Comment : "reply to"
    Comment ||--o{ CommentMention : mentions
    User ||--o{ CommentMention : "mentioned in"

    User ||--o{ Notification : receives
    User ||--o{ NotificationPreference : sets
    Organization ||--o{ ActivityLog : records
    User ||--o{ SavedFilter : owns

    Organization ||--o{ SyncChange : "change feed"
    Organization ||--o{ SyncOperation : "idempotency ledger"
```

## Notes

- **`TaskItem`** (not `Task`) is the C# entity name — `System.Threading.Tasks.Task` is implicitly in
  scope everywhere via `ImplicitUsings`. JSON/routes/keys still say "task".
- **Attachments** are polymorphic: `(EntityType, EntityId)` binds a file to a Task, Comment, Project
  or Epic without a hard FK, so they aren't drawn as a single relationship above.
- **Cross-aggregate FKs** (assignee, reporter, owner → `User`; org denormalized onto Epic/Sprint/
  Task) use `RESTRICT`/`SET NULL` to avoid SQL Server multiple-cascade-path errors; cascades stay
  within a single aggregate (e.g. Task → Comment, Task → Subtask).
- **`SyncChange.ServerVersion`** is a globally-monotonic `bigint` identity — the client's delta
  cursor. **`SyncOperation.Id`** is the client-generated OperationId (idempotency key).

The authoritative field list is in `planning.md` §3 (Data Model).
