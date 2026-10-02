import 'package:drift/drift.dart' show Value;
import 'package:get/get.dart' hide Value;

import '../../core/database/app_database.dart';
import '../../data/repositories/task_repository.dart';
import '../../data/sync/sync_engine.dart';

/// View/edit one task. Reads the live row from Drift (so pulled/synced updates
/// reflect) and writes edits through the offline-first repository.
class TaskDetailController extends GetxController {
  TaskDetailController(this._tasks, this._sync);

  final TaskRepository _tasks;
  final SyncEngine _sync;

  late final String taskId;
  final Rxn<LocalTask> task = Rxn<LocalTask>();

  @override
  void onInit() {
    super.onInit();
    taskId = (Get.arguments as Map)['taskId'] as String;
    _tasks.watchTask(taskId).listen(task.call);
  }

  Future<void> save({
    required String title,
    String? description,
    required int taskType,
    required int status,
    required int priority,
    int? storyPoints,
    DateTime? dueDate,
  }) async {
    await _tasks.updateTask(
      taskId,
      LocalTasksCompanion(
        title: Value(title),
        description: Value(description),
        taskType: Value(taskType),
        status: Value(status),
        priority: Value(priority),
        storyPoints: Value(storyPoints),
        dueDate: Value(dueDate),
      ),
    );
    _sync.sync();
  }

  Future<void> delete() async {
    await _tasks.deleteTask(taskId);
    _sync.sync();
    Get.back();
  }
}
