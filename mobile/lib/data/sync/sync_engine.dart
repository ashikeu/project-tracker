import 'dart:async';
import 'dart:convert';

import 'package:drift/drift.dart';
import 'package:get/get.dart' hide Value;

import '../../core/database/app_database.dart';
import '../../core/errors/failure.dart';
import '../../core/services/connectivity_service.dart';
import '../datasources/remote/sync_remote.dart';
import '../models/sync_models.dart';
import '../models/task_payload.dart';
import '../repositories/task_repository.dart';

/// Reconciles the local Drift DB with the server. Push drains the outbox
/// (idempotent by OperationId); pull applies the server's delta feed. Runs on
/// reconnect and on a periodic timer; a mutex guarantees a single drain at a time.
class SyncEngine extends GetxService {
  SyncEngine(this._db, this._remote, this._connectivity);

  final AppDatabase _db;
  final SyncRemote _remote;
  final ConnectivityService _connectivity;

  static const int _maxBatch = 200;
  static const Duration _period = Duration(seconds: 30);

  final RxBool isSyncing = false.obs;
  final Rxn<DateTime> lastSyncAt = Rxn<DateTime>();
  final RxString lastError = ''.obs;

  /// Exponential backoff schedule for retrying after a failed sync (capped).
  /// Indexed by consecutive-failure count; the last entry repeats.
  static const List<Duration> backoffSchedule = [
    Duration(seconds: 5),
    Duration(seconds: 15),
    Duration(seconds: 60),
    Duration(minutes: 5),
  ];

  /// The delay before the next retry given how many syncs have failed in a row.
  static Duration backoffFor(int consecutiveFailures) {
    if (consecutiveFailures <= 0) return Duration.zero;
    final index = (consecutiveFailures - 1).clamp(0, backoffSchedule.length - 1);
    return backoffSchedule[index];
  }

  Timer? _timer;
  Timer? _retryTimer;
  StreamSubscription<void>? _reconnectSub;
  bool _draining = false;
  int _consecutiveFailures = 0;

  SyncEngine start() {
    // Reconnecting clears the backoff and retries immediately.
    _reconnectSub = _connectivity.onReconnect.listen((_) {
      _consecutiveFailures = 0;
      sync();
    });
    // A slow heartbeat picks up work queued while online between failures.
    _timer = Timer.periodic(_period, (_) {
      if (_connectivity.isOnline.value) sync();
    });
    return this;
  }

  // ---- Reactive counters for the UI (pending / failed / conflicts) ----

  Stream<int> watchPendingCount() => _db
      .customSelect('SELECT COUNT(*) AS c FROM sync_queue_items WHERE status = ?',
          variables: [Variable('pending')], readsFrom: {_db.syncQueueItems})
      .watchSingle()
      .map((r) => r.read<int>('c'));

  Stream<int> watchFailedCount() => _db
      .customSelect('SELECT COUNT(*) AS c FROM sync_queue_items WHERE status = ?',
          variables: [Variable('failed')], readsFrom: {_db.syncQueueItems})
      .watchSingle()
      .map((r) => r.read<int>('c'));

  Stream<int> watchConflictCount() => _db
      .customSelect('SELECT COUNT(*) AS c FROM conflict_items', readsFrom: {_db.conflictItems})
      .watchSingle()
      .map((r) => r.read<int>('c'));

  // ---- Orchestration ----

  /// Full reconcile: push local changes first (so our edits win where there's no
  /// conflict), then pull the server delta. No-op (returns false) if already
  /// running or offline.
  Future<bool> sync() async {
    if (_draining) return false;
    if (!_connectivity.isOnline.value) return false;
    _draining = true;
    isSyncing.value = true;
    lastError.value = '';
    try {
      await _drainOutbox();
      await _pullDelta();
      lastSyncAt.value = DateTime.now();
      _consecutiveFailures = 0;
      _retryTimer?.cancel();
      return true;
    } on NetworkFailure {
      // Stay offline-tolerant; retry on reconnect or after a backoff delay.
      _scheduleBackoffRetry();
      return false;
    } on Failure catch (f) {
      lastError.value = f.message;
      _scheduleBackoffRetry();
      return false;
    } finally {
      _draining = false;
      isSyncing.value = false;
    }
  }

  /// Arranges the next retry on an exponential backoff after a failure, so a
  /// flaky connection or a struggling server is retried progressively rather
  /// than hammered. Never gives up silently — pending ops stay queued.
  void _scheduleBackoffRetry() {
    _consecutiveFailures++;
    final delay = backoffFor(_consecutiveFailures);
    _retryTimer?.cancel();
    _retryTimer = Timer(delay, () {
      if (_connectivity.isOnline.value) sync();
    });
  }

  // ---- Push ----

  Future<void> _drainOutbox() async {
    while (true) {
      final batch = await (_db.select(_db.syncQueueItems)
            ..where((q) => q.status.equals('pending'))
            ..orderBy([(q) => OrderingTerm(expression: q.createdAt)])
            ..limit(_maxBatch))
          .get();
      if (batch.isEmpty) return;

      final inputs = <SyncOperationInput>[];
      final byOpId = <String, SyncQueueItem>{};
      for (final q in batch) {
        final task = await (_db.select(_db.localTasks)..where((t) => t.id.equals(q.entityId)))
            .getSingleOrNull();
        if (task == null) {
          // Nothing to send (e.g. a pendingCreate that was dropped) — discard.
          await _deleteOp(q.operationId);
          continue;
        }
        final isUpdate = q.changeType == 'updated';
        final isDelete = q.changeType == 'deleted';
        inputs.add(SyncOperationInput(
          operationId: q.operationId,
          entityId: q.entityId,
          changeType: q.changeType,
          payloadJson: isDelete ? null : TaskRepository.payloadJsonFromRow(task),
          baseRowVersion: (isUpdate || isDelete) && task.rowVersion.isNotEmpty
              ? task.rowVersion
              : null,
        ));
        byOpId[q.operationId] = q;
      }

      if (inputs.isEmpty) continue;

      final results = await _remote.push(inputs); // throws NetworkFailure if offline
      for (final r in results) {
        final q = byOpId[r.operationId];
        if (q == null) continue;
        await _applyPushResult(q, r);
      }
    }
  }

  Future<void> _applyPushResult(SyncQueueItem q, SyncOperationResult r) async {
    if (r.isApplied) {
      await _markTaskSynced(q.entityId, rowVersion: r.rowVersion, taskKey: r.taskKey);
      await _deleteOp(q.operationId);
    } else if (r.isConflict) {
      await _recordConflict(q.entityId, r);
      await _deleteOp(q.operationId);
    } else {
      // Rejected — terminal. Park it in the Failed bucket for the user.
      await (_db.update(_db.syncQueueItems)..where((x) => x.operationId.equals(q.operationId)))
          .write(SyncQueueItemsCompanion(
        status: const Value('failed'),
        attemptCount: Value(q.attemptCount + 1),
        lastError: Value(r.message ?? 'Rejected by server.'),
      ));
    }
  }

  Future<void> _markTaskSynced(String id, {String? rowVersion, String? taskKey}) async {
    await (_db.update(_db.localTasks)..where((t) => t.id.equals(id))).write(LocalTasksCompanion(
      syncState: const Value('synced'),
      needsReview: const Value(false),
      rowVersion: rowVersion == null ? const Value.absent() : Value(rowVersion),
      taskKey: (taskKey == null || taskKey.isEmpty) ? const Value.absent() : Value(taskKey),
    ));
  }

  /// Server-wins by default: apply the server's version locally and flag the row
  /// for review, keeping the user's version in the conflict bucket so they can
  /// override (client-wins) without ever silently losing their edit.
  Future<void> _recordConflict(String id, SyncOperationResult r) async {
    final local = await (_db.select(_db.localTasks)..where((t) => t.id.equals(id)))
        .getSingleOrNull();
    final localJson = local == null ? '{}' : TaskRepository.payloadJsonFromRow(local);
    final serverJson = r.serverPayloadJson ?? '{}';

    await _db.into(_db.conflictItems).insert(ConflictItemsCompanion.insert(
          entityId: id,
          localPayloadJson: localJson,
          serverPayloadJson: serverJson,
          createdAt: DateTime.now().toUtc(),
        ));

    final serverPayload = TaskPayload.fromJson(jsonDecode(serverJson) as Map<String, dynamic>);
    await _db.into(_db.localTasks).insertOnConflictUpdate(
          TaskRepository.companionFromPayload(serverPayload, needsReview: true),
        );
  }

  // ---- Pull ----

  Future<void> _pullDelta() async {
    var since = await _db.getLastSyncVersion();
    while (true) {
      final result = await _remote.pull(since: since, pageSize: _maxBatch);
      for (final change in result.changes) {
        await _applyPulledChange(change);
      }
      if (result.nextVersion > since) {
        since = result.nextVersion;
        await _db.setLastSyncVersion(since);
      }
      if (!result.hasMore) break;
    }
  }

  Future<void> _applyPulledChange(SyncChangeDto change) async {
    if (change.entityType.toLowerCase() != 'task') return;

    final local = await (_db.select(_db.localTasks)..where((t) => t.id.equals(change.entityId)))
        .getSingleOrNull();

    // Don't clobber a row that still has un-pushed local edits; push resolves it.
    if (local != null && local.syncState != 'synced') return;

    if (change.isTombstone) {
      if (local != null) {
        await (_db.update(_db.localTasks)..where((t) => t.id.equals(change.entityId)))
            .write(const LocalTasksCompanion(
          isDeleted: Value(true),
          syncState: Value('synced'),
        ));
      }
      return;
    }

    final payload = TaskPayload.fromJson(jsonDecode(change.payloadJson!) as Map<String, dynamic>);
    await _db.into(_db.localTasks).insertOnConflictUpdate(
          TaskRepository.companionFromPayload(payload),
        );
  }

  Future<void> _deleteOp(String operationId) =>
      (_db.delete(_db.syncQueueItems)..where((q) => q.operationId.equals(operationId))).go();

  // ---- Failed / conflict management (called from the UI) ----

  /// Retry a failed op by moving it back to pending, then kicking a sync.
  Future<void> retryFailed(String operationId) async {
    await (_db.update(_db.syncQueueItems)..where((q) => q.operationId.equals(operationId)))
        .write(const SyncQueueItemsCompanion(status: Value('pending'), lastError: Value(null)));
    await sync();
  }

  Future<List<SyncQueueItem>> failedOps() =>
      (_db.select(_db.syncQueueItems)..where((q) => q.status.equals('failed'))).get();

  Future<List<ConflictItem>> conflicts() => _db.select(_db.conflictItems).get();

  /// Keep the server's version (already applied locally); just clear the flag.
  Future<void> resolveConflictServerWins(int conflictId, String entityId) async {
    await (_db.delete(_db.conflictItems)..where((c) => c.id.equals(conflictId))).go();
    await (_db.update(_db.localTasks)..where((t) => t.id.equals(entityId)))
        .write(const LocalTasksCompanion(needsReview: Value(false)));
  }

  /// Override with the user's version: re-apply the local payload on top of the
  /// server's current RowVersion and enqueue a fresh update to push it.
  Future<void> resolveConflictClientWins(
      int conflictId, String entityId, TaskRepository tasks) async {
    final conflict = await (_db.select(_db.conflictItems)..where((c) => c.id.equals(conflictId)))
        .getSingleOrNull();
    if (conflict == null) return;

    final localPayload =
        TaskPayload.fromJson(jsonDecode(conflict.localPayloadJson) as Map<String, dynamic>);

    // Re-apply the user's field values, but on the server's latest RowVersion
    // (already in the local row from the server-wins step) so the retry passes
    // the concurrency check.
    await tasks.updateTask(
      entityId,
      LocalTasksCompanion(
        title: Value(localPayload.title),
        description: Value(localPayload.description),
        taskType: Value(localPayload.taskType),
        status: Value(localPayload.status),
        priority: Value(localPayload.priority),
        assigneeUserId: Value(localPayload.assigneeUserId),
        boardColumnId: Value(localPayload.boardColumnId),
        storyPoints: Value(localPayload.storyPoints),
        estimatedHours: Value(localPayload.estimatedHours),
        remainingHours: Value(localPayload.remainingHours),
        startDate: Value(localPayload.startDate),
        dueDate: Value(localPayload.dueDate),
        position: Value(localPayload.position),
      ),
    );
    await (_db.delete(_db.conflictItems)..where((c) => c.id.equals(conflictId))).go();
    await sync();
  }

  @override
  void onClose() {
    _timer?.cancel();
    _retryTimer?.cancel();
    _reconnectSub?.cancel();
    super.onClose();
  }
}
