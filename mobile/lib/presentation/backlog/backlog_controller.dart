import 'package:get/get.dart';

import '../../core/errors/failure.dart';
import '../../data/datasources/remote/backlog_remote.dart';
import '../../data/datasources/remote/sprint_remote.dart';
import '../../data/models/backlog_task_model.dart';
import '../../data/models/sprint_model.dart';

/// Lists and reorders the product backlog and assigns items to sprints
/// (online-only). Reorder is optimistic: the list is reordered locally for
/// instant feedback, then the server call confirms (and a failure reloads the
/// authoritative order).
class BacklogController extends GetxController {
  BacklogController(this._backlog, this._sprints);

  final BacklogRemote _backlog;
  final SprintRemote _sprints;

  late final String projectId;
  late final String projectName;

  final RxList<BacklogTaskModel> items = <BacklogTaskModel>[].obs;
  final RxList<SprintModel> sprints = <SprintModel>[].obs; // Planning/Active, for the picker
  final RxBool isLoading = false.obs;
  final RxString error = ''.obs;

  @override
  void onInit() {
    super.onInit();
    final args = (Get.arguments as Map?) ?? {};
    projectId = args['projectId'] as String;
    projectName = (args['projectName'] ?? 'Backlog') as String;
    load();
  }

  Future<void> load() async {
    isLoading.value = true;
    error.value = '';
    try {
      final results = await Future.wait([
        _backlog.list(projectId),
        _sprints.list(projectId),
      ]);
      items.assignAll(results[0] as List<BacklogTaskModel>);
      final all = results[1] as List<SprintModel>;
      // Only Planning/Active sprints are valid assignment targets.
      sprints.assignAll(all.where((s) => s.isPlanning || s.isActive));
    } on Failure catch (f) {
      error.value = f.message;
    } finally {
      isLoading.value = false;
    }
  }

  /// Optimistically moves the item at [oldIndex] to [newIndex], then persists.
  /// Wired to ReorderableListView's `onReorderItem`, so [newIndex] is already
  /// adjusted for the removed item (it's the target index among the others) —
  /// which is exactly the server's MoveBacklogItem TargetIndex.
  Future<void> reorder(int oldIndex, int newIndex) async {
    if (oldIndex == newIndex) return;
    final moved = items.removeAt(oldIndex);
    items.insert(newIndex, moved);
    items.refresh();
    try {
      await _backlog.reorder(moved.id, newIndex);
    } on Failure catch (f) {
      error.value = f.message;
      await load(); // snap back to the server's authoritative order
    }
  }

  Future<bool> assignToSprint(String taskId, String? sprintId) async {
    error.value = '';
    try {
      await _backlog.assignToSprint(taskId, sprintId);
      await load();
      return true;
    } on Failure catch (f) {
      error.value = f.message;
      return false;
    }
  }
}
