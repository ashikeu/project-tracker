import 'dart:convert';

import 'package:get/get.dart';

import '../../core/database/app_database.dart';
import '../../data/models/task_payload.dart';
import '../../data/repositories/task_repository.dart';
import '../../data/sync/sync_engine.dart';

class SyncCenterController extends GetxController {
  SyncCenterController(this._sync, this._tasks);

  final SyncEngine _sync;
  final TaskRepository _tasks;

  final RxList<SyncQueueItem> failed = <SyncQueueItem>[].obs;
  final RxList<ConflictItem> conflicts = <ConflictItem>[].obs;

  SyncEngine get engine => _sync;

  @override
  void onInit() {
    super.onInit();
    // Refresh the lists whenever the underlying tables change.
    _sync.watchFailedCount().listen((_) => _reload());
    _sync.watchConflictCount().listen((_) => _reload());
    _reload();
  }

  Future<void> _reload() async {
    failed.assignAll(await _sync.failedOps());
    conflicts.assignAll(await _sync.conflicts());
  }

  Future<void> syncNow() => _sync.sync();

  Future<void> retry(String operationId) => _sync.retryFailed(operationId);

  Future<void> keepServer(ConflictItem c) =>
      _sync.resolveConflictServerWins(c.id, c.entityId);

  Future<void> keepMine(ConflictItem c) =>
      _sync.resolveConflictClientWins(c.id, c.entityId, _tasks);

  /// Human-readable one-liner describing how two versions differ.
  String diffSummary(ConflictItem c) {
    try {
      final local = TaskPayload.fromJson(jsonDecode(c.localPayloadJson) as Map<String, dynamic>);
      final server = TaskPayload.fromJson(jsonDecode(c.serverPayloadJson) as Map<String, dynamic>);
      final diffs = <String>[];
      if (local.title != server.title) diffs.add('title');
      if (local.description != server.description) diffs.add('description');
      if (local.status != server.status) diffs.add('status');
      if (local.priority != server.priority) diffs.add('priority');
      if (local.assigneeUserId != server.assigneeUserId) diffs.add('assignee');
      if (local.storyPoints != server.storyPoints) diffs.add('story points');
      if (local.position != server.position) diffs.add('position');
      return diffs.isEmpty ? 'Differs from the server copy.' : 'Differs in: ${diffs.join(', ')}.';
    } catch (_) {
      return 'Differs from the server copy.';
    }
  }

  String titleOf(ConflictItem c) {
    try {
      return TaskPayload.fromJson(jsonDecode(c.serverPayloadJson) as Map<String, dynamic>).title;
    } catch (_) {
      return 'Task';
    }
  }
}
