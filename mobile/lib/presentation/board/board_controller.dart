import 'dart:async';

import 'package:get/get.dart';

import '../../core/database/app_database.dart';
import '../../data/repositories/auth_repository.dart';
import '../../data/repositories/project_repository.dart';
import '../../data/repositories/task_repository.dart';
import '../../data/sync/sync_engine.dart';
import '../../domain/enums.dart';

/// Drives the Kanban board for one project. Columns come from the (cached) board
/// config; tasks are watched live from Drift so local edits and pulled changes
/// both reflect instantly. All moves/creates go through the offline-first
/// TaskRepository.
class BoardController extends GetxController {
  BoardController(this._projects, this._tasks, this._sync, this._auth);

  final ProjectRepository _projects;
  final TaskRepository _tasks;
  final SyncEngine _sync;
  final AuthRepository _auth;

  late final String projectId;
  late final String projectName;
  late final String projectKey;

  final RxList<LocalBoardColumn> columns = <LocalBoardColumn>[].obs;
  final RxList<LocalTask> tasks = <LocalTask>[].obs;
  final RxBool isLoading = true.obs;

  StreamSubscription<List<LocalTask>>? _taskSub;
  String? _currentUserId;

  static const double _gap = 1024.0;

  @override
  void onInit() {
    super.onInit();
    final args = (Get.arguments as Map?) ?? {};
    projectId = args['projectId'] as String;
    projectName = (args['projectName'] ?? 'Board') as String;
    projectKey = (args['projectKey'] ?? '') as String;
    _init();
  }

  Future<void> _init() async {
    _currentUserId = await _auth.currentUserId();
    columns.assignAll(await _projects.cachedColumns(projectId));
    _taskSub = _tasks.watchProjectTasks(projectId).listen(tasks.assignAll);
    isLoading.value = false;

    // Refresh board config + pull latest tasks when possible.
    await _projects.tryRefreshColumns(projectId);
    columns.assignAll(await _projects.cachedColumns(projectId));
    unawaited(_sync.sync());
  }

  @override
  void onClose() {
    _taskSub?.cancel();
    super.onClose();
  }

  /// Tasks belonging to a column: by explicit boardColumnId, else by status
  /// mapping (so freshly-pulled tasks without a column still land somewhere).
  List<LocalTask> tasksForColumn(LocalBoardColumn column) {
    final mapped = TaskItemStatus.fromMapping(column.statusMapping).value;
    final list = tasks.where((t) {
      if (t.boardColumnId != null) return t.boardColumnId == column.id;
      return t.status == mapped;
    }).toList()
      ..sort((a, b) => a.position.compareTo(b.position));
    return list;
  }

  Future<void> createTask(LocalBoardColumn column, String title) async {
    final existing = tasksForColumn(column);
    final position = existing.isEmpty ? _gap : existing.last.position + _gap;
    await _tasks.createTask(
      projectId: projectId,
      title: title,
      status: TaskItemStatus.fromMapping(column.statusMapping).value,
      boardColumnId: column.id,
      position: position,
      reporterUserId: _currentUserId,
    );
    unawaited(_sync.sync());
  }

  /// Drop [taskId] at the end of [column]: updates status + column + position.
  Future<void> moveTaskToColumn(String taskId, LocalBoardColumn column) async {
    final task = tasks.firstWhereOrNull((t) => t.id == taskId);
    if (task == null) return;
    if (task.boardColumnId == column.id) return; // already here

    final existing = tasksForColumn(column).where((t) => t.id != taskId).toList();
    final position = existing.isEmpty ? _gap : existing.last.position + _gap;
    await _tasks.moveTask(
      taskId,
      status: TaskItemStatus.fromMapping(column.statusMapping).value,
      boardColumnId: column.id,
      position: position,
    );
    unawaited(_sync.sync());
  }

  bool exceedsWip(LocalBoardColumn column) {
    final limit = column.wipLimit;
    if (limit == null) return false;
    return tasksForColumn(column).length >= limit;
  }
}
