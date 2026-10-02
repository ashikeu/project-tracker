import 'package:dio/dio.dart';

import '../../../core/constants/api_constants.dart';
import '../../../core/network/response_handler.dart';
import '../../models/backlog_task_model.dart';

/// Reads the product backlog (top-level tasks with no sprint) and performs the
/// two backlog operations: reorder (fractional-index) and assign-to-sprint
/// (via the shared /tasks/bulk endpoint). Online-only — the backlog is derived
/// server-side, not part of the offline task sync.
class BacklogRemote {
  BacklogRemote(this._dio);
  final Dio _dio;

  Future<List<BacklogTaskModel>> list(String projectId,
      {int page = 1, int pageSize = 100}) async {
    try {
      final resp = await _dio.get(ApiConstants.backlog, queryParameters: {
        'projectId': projectId,
        'page': page,
        'pageSize': pageSize,
      });
      final data = unwrap(resp) as Map<String, dynamic>;
      final items = (data['items'] as List<dynamic>? ?? []);
      return items
          .map((e) => BacklogTaskModel.fromJson(e as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw mapDioError(e);
    }
  }

  /// Moves [taskId] to [targetIndex] (0-based position among the other backlog
  /// items). The server recomputes the fractional position and self-heals.
  Future<void> reorder(String taskId, int targetIndex) async {
    try {
      final resp = await _dio.post(ApiConstants.backlogReorder, data: {
        'taskId': taskId,
        'targetIndex': targetIndex,
      });
      unwrap(resp);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  /// Assigns a backlog task to a sprint (or back to the backlog when
  /// [sprintId] is null) via the bulk endpoint's ChangeSprint flag.
  Future<void> assignToSprint(String taskId, String? sprintId) async {
    try {
      final resp = await _dio.post(ApiConstants.tasksBulk, data: {
        'taskIds': [taskId],
        'changeAssignee': false,
        'changeEpic': false,
        'changeSprint': true,
        'sprintId': sprintId,
      });
      unwrap(resp);
    } catch (e) {
      throw mapDioError(e);
    }
  }
}
