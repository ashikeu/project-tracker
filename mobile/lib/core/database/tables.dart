import 'package:drift/drift.dart';

/// Local mirror of the server's TaskItem — the one offline-editable entity.
/// Enum fields (taskType/status/priority) are stored as ints to match the sync
/// wire payload (TaskSyncPayload). [syncState] and [needsReview] are client-only
/// bookkeeping columns, never sent to the server.
class LocalTasks extends Table {
  TextColumn get id => text()(); // Guid (client-generatable)
  TextColumn get taskKey => text().withDefault(const Constant(''))();
  TextColumn get projectId => text()();
  TextColumn get epicId => text().nullable()();
  TextColumn get sprintId => text().nullable()();
  TextColumn get parentTaskId => text().nullable()();
  TextColumn get title => text()();
  TextColumn get description => text().nullable()();
  IntColumn get taskType => integer().withDefault(const Constant(0))();
  IntColumn get status => integer().withDefault(const Constant(0))();
  IntColumn get priority => integer().withDefault(const Constant(2))();
  TextColumn get assigneeUserId => text().nullable()();
  TextColumn get reporterUserId => text().nullable()();
  TextColumn get teamId => text().nullable()();
  TextColumn get boardColumnId => text().nullable()();
  IntColumn get storyPoints => integer().nullable()();
  RealColumn get estimatedHours => real().nullable()();
  RealColumn get remainingHours => real().nullable()();
  RealColumn get loggedHours => real().withDefault(const Constant(0))();
  DateTimeColumn get startDate => dateTime().nullable()();
  DateTimeColumn get dueDate => dateTime().nullable()();
  RealColumn get position => real().withDefault(const Constant(0))();
  BoolColumn get isDeleted => boolean().withDefault(const Constant(false))();

  /// base64 of the server RowVersion; empty until the row has been synced once.
  TextColumn get rowVersion => text().withDefault(const Constant(''))();
  DateTimeColumn get updatedAt => dateTime().nullable()();

  /// Client sync bookkeeping: 'synced' | 'pendingCreate' | 'pendingUpdate' | 'pendingDelete'.
  TextColumn get syncState => text().withDefault(const Constant('synced'))();

  /// True when a server conflict diverged from local edits and the user must reconcile.
  BoolColumn get needsReview => boolean().withDefault(const Constant(false))();

  @override
  Set<Column> get primaryKey => {id};
}

/// The outbox of mutations waiting to be pushed to /sync/push. One row per
/// client operation, idempotent on [operationId].
class SyncQueueItems extends Table {
  TextColumn get operationId => text()(); // Guid, the idempotency key
  TextColumn get entityType => text().withDefault(const Constant('Task'))();
  TextColumn get entityId => text()();
  TextColumn get changeType => text()(); // created | updated | deleted
  TextColumn get payloadJson => text().nullable()();
  TextColumn get baseRowVersion => text().nullable()();

  /// 'pending' | 'failed'. A 'failed' item exceeded max retries and awaits the user.
  TextColumn get status => text().withDefault(const Constant('pending'))();
  IntColumn get attemptCount => integer().withDefault(const Constant(0))();
  DateTimeColumn get nextAttemptAt => dateTime().nullable()();
  TextColumn get lastError => text().nullable()();
  DateTimeColumn get createdAt => dateTime()();

  @override
  Set<Column> get primaryKey => {operationId};
}

/// A sync push that came back Conflict — kept until the user resolves it
/// (server-wins by default, or client-wins override). Never silently dropped.
class ConflictItems extends Table {
  IntColumn get id => integer().autoIncrement()();
  TextColumn get entityType => text().withDefault(const Constant('Task'))();
  TextColumn get entityId => text()();
  TextColumn get localPayloadJson => text()();
  TextColumn get serverPayloadJson => text()();
  DateTimeColumn get createdAt => dateTime()();
}

/// Read-only cache of projects fetched online (projects aren't offline-editable).
class LocalProjects extends Table {
  TextColumn get id => text()();
  TextColumn get key => text()();
  TextColumn get name => text()();
  TextColumn get description => text().nullable()();
  TextColumn get status => text().withDefault(const Constant('Active'))();
  TextColumn get priority => text().withDefault(const Constant('Medium'))();
  BoolColumn get isArchived => boolean().withDefault(const Constant(false))();

  @override
  Set<Column> get primaryKey => {id};
}

/// Read-only cache of a project's board columns (status buckets for the Kanban board).
class LocalBoardColumns extends Table {
  TextColumn get id => text()();
  TextColumn get projectId => text()();
  TextColumn get name => text()();
  TextColumn get color => text().withDefault(const Constant('#6B7280'))();
  IntColumn get order => integer().withDefault(const Constant(0))();
  IntColumn get wipLimit => integer().nullable()();
  TextColumn get statusMapping => text().withDefault(const Constant('Todo'))();
  BoolColumn get isCompletionColumn => boolean().withDefault(const Constant(false))();

  @override
  Set<Column> get primaryKey => {id};
}

/// Read-only cache of org members, for assignee names on task cards.
class LocalUsers extends Table {
  TextColumn get id => text()();
  TextColumn get email => text().withDefault(const Constant(''))();
  TextColumn get firstName => text().withDefault(const Constant(''))();
  TextColumn get lastName => text().withDefault(const Constant(''))();

  @override
  Set<Column> get primaryKey => {id};
}

/// Simple key/value store for sync metadata (e.g. the pull watermark 'lastSyncVersion').
class SyncMeta extends Table {
  TextColumn get key => text()();
  TextColumn get value => text()();

  @override
  Set<Column> get primaryKey => {key};
}
