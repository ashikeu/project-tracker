import 'package:get/get.dart';

import '../../core/errors/failure.dart';
import '../../data/datasources/remote/sprint_remote.dart';
import '../../data/models/sprint_model.dart';

/// Lists and manages a project's sprints (online-only). The server owns the
/// state machine, so this controller just invokes start/complete/cancel and
/// re-reads the list on success.
class SprintsController extends GetxController {
  SprintsController(this._remote);

  final SprintRemote _remote;

  late final String projectId;
  late final String projectName;

  final RxList<SprintModel> sprints = <SprintModel>[].obs;
  final RxBool isLoading = false.obs;
  final RxString error = ''.obs;
  final RxBool busy = false.obs; // a lifecycle action is in flight

  @override
  void onInit() {
    super.onInit();
    final args = (Get.arguments as Map?) ?? {};
    projectId = args['projectId'] as String;
    projectName = (args['projectName'] ?? 'Sprints') as String;
    load();
  }

  Future<void> load() async {
    isLoading.value = true;
    error.value = '';
    try {
      sprints.assignAll(await _remote.list(projectId));
    } on Failure catch (f) {
      error.value = f.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<bool> create({
    required String name,
    String? goal,
    String? startDate,
    String? endDate,
  }) =>
      _run(() => _remote.create(
            projectId: projectId,
            name: name,
            goal: goal,
            startDate: startDate,
            endDate: endDate,
          ));

  Future<bool> updateSprint({
    required String id,
    required String name,
    String? goal,
    String? startDate,
    String? endDate,
  }) =>
      _run(() => _remote.update(
            id: id,
            name: name,
            goal: goal,
            startDate: startDate,
            endDate: endDate,
          ));

  Future<bool> start(String id) => _run(() => _remote.start(id));

  Future<bool> complete(String id, {bool moveIncompleteToBacklog = true}) =>
      _run(() => _remote.complete(id, moveIncompleteToBacklog: moveIncompleteToBacklog));

  Future<bool> cancel(String id) => _run(() => _remote.cancel(id));

  Future<bool> remove(String id) => _run(() => _remote.delete(id));

  /// Runs a mutating action, surfaces its error message to [error] on failure,
  /// and refreshes the list on success. Returns whether it succeeded.
  Future<bool> _run(Future<void> Function() action) async {
    busy.value = true;
    error.value = '';
    try {
      await action();
      await load();
      return true;
    } on Failure catch (f) {
      error.value = f.message;
      return false;
    } finally {
      busy.value = false;
    }
  }
}
