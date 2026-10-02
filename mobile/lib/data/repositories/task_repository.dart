import 'dart:convert';

import 'package:drift/drift.dart';
import 'package:uuid/uuid.dart';

import '../../core/database/app_database.dart';
import '../models/task_payload.dart';

/// The offline-first write path. Every task mutation is applied to Drift FIRST
/// (so the UI updates instantly, online or off) and an operation is enqueued in
/// the outbox for the SyncEngine to push. Tasks are the one offline-editable
/// entity, matching the backend's /sync scope.
class TaskRepository {
  TaskRepository(this._db, {Uuid? uuid}) : _uuid = uuid ?? const Uuid();

  final AppDatabase _db;
  final Uuid _uuid;

  // ---- Reads (Drift is the source of truth) ----

  /// Live top-level tasks for a project (excludes soft-deleted + subtasks).
  Stream<List<LocalTask>> watchProjectTasks(String projectId) {
    return (_db.select(_db.localTasks)
          ..where((t) =>
              t.projectId.equals(projectId) &
              t.isDeleted.equals(false) &
              t.parentTaskId.isNull())
          ..orderBy([(t) => OrderingTerm(expression: t.position)]))
        .watch();
  }

  Future<LocalTask?> getTask(String id) =>
      (_db.select(_db.localTasks)..where((t) => t.id.equals(id))).getSingleOrNull();

  Stream<LocalTask?> watchTask(String id) =>
      (_db.select(_db.localTasks)..where((t) => t.id.equals(id))).watchSingleOrNull();

  Future<List<LocalTask>> subtasksOf(String parentId) =>
      (_db.select(_db.localTasks)
            ..where((t) => t.parentTaskId.equals(parentId) & t.isDeleted.equals(false)))
          .get();

  // ---- Writes (local-first, then enqueue) ----

  /// Creates a task offline. Generates the Guid locally (no server round-trip);
  /// TaskKey stays empty until the server assigns it on first successful push.
  Future<String> createTask({
    required String projectId,
    required String title,
    String? description,
    int taskType = 0,
    int status = 0,
    int priority = 2,
    String? assigneeUserId,
    String? boardColumnId,
    int? storyPoints,
    DateTime? dueDate,
    required double position,
    String? reporterUserId,
  }) async {
    final id = _uuid.v4();
    await _db.into(_db.localTasks).insert(LocalTasksCompanion.insert(
          id: id,
          projectId: projectId,
          title: title,
          description: Value(description),
          taskType: Value(taskType),
          status: Value(status),
          priority: Value(priority),
          assigneeUserId: Value(assigneeUserId),
          reporterUserId: Value(reporterUserId),
          boardColumnId: Value(boardColumnId),
          storyPoints: Value(storyPoints),
          dueDate: Value(dueDate),
          position: Value(position),
          syncState: const Value('pendingCreate'),
          updatedAt: Value(DateTime.now().toUtc()),
        ));
    await _enqueue(id, 'created');
    return id;
  }

  /// Applies an arbitrary field patch locally and enqueues an update (or keeps a
  /// not-yet-synced create as a create — the payload is rebuilt at push time).
  Future<void> updateTask(String id, LocalTasksCompanion patch) async {
    final current = await getTask(id);
    if (current == null) return;

    final nextState = current.syncState == 'pendingCreate' ? 'pendingCreate' : 'pendingUpdate';
    await (_db.update(_db.localTasks)..where((t) => t.id.equals(id))).write(
      patch.copyWith(
        syncState: Value(nextState),
        updatedAt: Value(DateTime.now().toUtc()),
      ),
    );
    await _enqueue(id, nextState == 'pendingCreate' ? 'created' : 'updated');
  }

  /// Board move: status + column + fractional position, as one local update.
  Future<void> moveTask(String id, {
    required int status,
    required String boardColumnId,
    required double position,
  }) {
    return updateTask(
      id,
      LocalTasksCompanion(
        status: Value(status),
        boardColumnId: Value(boardColumnId),
        position: Value(position),
      ),
    );
  }

  /// Soft-deletes a task. A task that was never synced (still pendingCreate with
  /// no server RowVersion) is simply dropped locally — the server never saw it,
  /// so there's nothing to push.
  Future<void> deleteTask(String id) async {
    final current = await getTask(id);
    if (current == null) return;

    if (current.syncState == 'pendingCreate' && current.rowVersion.isEmpty) {
      await (_db.delete(_db.localTasks)..where((t) => t.id.equals(id))).go();
      await _removePendingOps(id);
      return;
    }

    await (_db.update(_db.localTasks)..where((t) => t.id.equals(id))).write(
      LocalTasksCompanion(
        isDeleted: const Value(true),
        syncState: const Value('pendingDelete'),
        updatedAt: Value(DateTime.now().toUtc()),
      ),
    );
    await _enqueue(id, 'deleted');
  }

  // ---- Outbox helpers ----

  /// Replaces any existing *pending* op for this task with a fresh one. A fresh
  /// OperationId each time is required: the server caches results per OperationId
  /// and would replay the stale result if we reused a spent id for new data.
  Future<void> _enqueue(String entityId, String changeType) async {
    await _removePendingOps(entityId);
    await _db.into(_db.syncQueueItems).insert(SyncQueueItemsCompanion.insert(
          operationId: _uuid.v4(),
          entityId: entityId,
          changeType: changeType,
          createdAt: DateTime.now().toUtc(),
        ));
  }

  Future<void> _removePendingOps(String entityId) {
    return (_db.delete(_db.syncQueueItems)
          ..where((q) => q.entityId.equals(entityId) & q.status.equals('pending')))
        .go();
  }

  // ---- Payload mapping (Drift row <-> wire payload) ----

  static TaskPayload payloadFromRow(LocalTask t) => TaskPayload(
        id: t.id,
        taskKey: t.taskKey,
        projectId: t.projectId,
        epicId: t.epicId,
        sprintId: t.sprintId,
        parentTaskId: t.parentTaskId,
        title: t.title,
        description: t.description,
        taskType: t.taskType,
        status: t.status,
        priority: t.priority,
        assigneeUserId: t.assigneeUserId,
        reporterUserId: t.reporterUserId,
        teamId: t.teamId,
        boardColumnId: t.boardColumnId,
        storyPoints: t.storyPoints,
        estimatedHours: t.estimatedHours,
        remainingHours: t.remainingHours,
        loggedHours: t.loggedHours,
        startDate: t.startDate,
        dueDate: t.dueDate,
        position: t.position,
        isDeleted: t.isDeleted,
        rowVersion: t.rowVersion,
        updatedAt: t.updatedAt,
      );

  static String payloadJsonFromRow(LocalTask t) => jsonEncode(payloadFromRow(t).toJson());

  /// Upserts a task row from a server payload (used by the pull path and by
  /// server-wins conflict resolution). Always lands as 'synced'.
  static LocalTasksCompanion companionFromPayload(TaskPayload p, {bool needsReview = false}) =>
      LocalTasksCompanion(
        id: Value(p.id),
        taskKey: Value(p.taskKey),
        projectId: Value(p.projectId),
        epicId: Value(p.epicId),
        sprintId: Value(p.sprintId),
        parentTaskId: Value(p.parentTaskId),
        title: Value(p.title),
        description: Value(p.description),
        taskType: Value(p.taskType),
        status: Value(p.status),
        priority: Value(p.priority),
        assigneeUserId: Value(p.assigneeUserId),
        reporterUserId: Value(p.reporterUserId),
        teamId: Value(p.teamId),
        boardColumnId: Value(p.boardColumnId),
        storyPoints: Value(p.storyPoints),
        estimatedHours: Value(p.estimatedHours),
        remainingHours: Value(p.remainingHours),
        loggedHours: Value(p.loggedHours),
        startDate: Value(p.startDate),
        dueDate: Value(p.dueDate),
        position: Value(p.position),
        isDeleted: Value(p.isDeleted),
        rowVersion: Value(p.rowVersion),
        updatedAt: Value(p.updatedAt),
        syncState: const Value('synced'),
        needsReview: Value(needsReview),
      );
}
