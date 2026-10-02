import 'package:get/get.dart';

import '../../core/errors/failure.dart';
import '../../data/datasources/remote/time_entry_remote.dart';
import '../../data/models/time_entry_model.dart';
import '../../data/repositories/auth_repository.dart';

/// Task time tracking (online-only): the rolled-up summary, the entry list, a
/// manual-entry form, and a start/stop timer. The server enforces one running
/// timer per user and recomputes the task's logged/remaining hours on each
/// change, so this controller just re-reads after every mutation.
class TimeController extends GetxController {
  TimeController(this._remote, this._auth);

  final TimeEntryRemote _remote;
  final AuthRepository _auth;

  late final String taskId;
  final RxList<TimeEntryModel> entries = <TimeEntryModel>[].obs;
  final Rxn<TaskTimeSummaryModel> summary = Rxn<TaskTimeSummaryModel>();
  final RxBool isLoading = false.obs;
  final RxString error = ''.obs;
  final RxBool busy = false.obs;
  final RxnString currentUserId = RxnString();

  @override
  void onInit() {
    super.onInit();
    taskId = (Get.arguments as Map)['taskId'] as String;
    _loadUser();
    load();
  }

  Future<void> _loadUser() async {
    currentUserId.value = await _auth.currentUserId();
  }

  bool isMine(TimeEntryModel e) =>
      currentUserId.value != null && e.userId == currentUserId.value;

  /// The current user's running timer on this task, if any.
  TimeEntryModel? get myRunningTimer =>
      entries.firstWhereOrNull((e) => e.isRunningTimer && isMine(e));

  Future<void> load() async {
    isLoading.value = true;
    error.value = '';
    try {
      final results = await Future.wait([
        _remote.list(taskId),
        _remote.summary(taskId),
      ]);
      entries.assignAll(results[0] as List<TimeEntryModel>);
      summary.value = results[1] as TaskTimeSummaryModel;
    } on Failure catch (f) {
      error.value = f.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<bool> logManual({
    required String date,
    required int durationMinutes,
    bool isBillable = false,
    String? description,
  }) =>
      _run(() => _remote.create(
            taskId,
            date: date,
            durationMinutes: durationMinutes,
            isBillable: isBillable,
            description: description,
          ));

  Future<bool> startTimer({bool isBillable = false, String? description}) =>
      _run(() => _remote.startTimer(taskId, isBillable: isBillable, description: description));

  Future<bool> stopTimer() => _run(() => _remote.stopTimer());

  Future<bool> editEntry(
    String id, {
    required String date,
    required int durationMinutes,
    bool isBillable = false,
    String? description,
  }) =>
      _run(() => _remote.update(
            id,
            date: date,
            durationMinutes: durationMinutes,
            isBillable: isBillable,
            description: description,
          ));

  Future<bool> removeEntry(String id) => _run(() => _remote.delete(id));

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
