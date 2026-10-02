import 'package:dio/dio.dart';

import '../../../core/constants/api_constants.dart';
import '../../../core/network/response_handler.dart';
import '../../models/time_entry_model.dart';

/// Time entries are server-owned (not offline-synced); a thin online remote over
/// the time-tracking endpoints. The server enforces one running timer per user
/// and recomputes task logged/remaining hours from the entry set on every change.
class TimeEntryRemote {
  TimeEntryRemote(this._dio);
  final Dio _dio;

  Future<List<TimeEntryModel>> list(String taskId,
      {int page = 1, int pageSize = 100}) async {
    try {
      final resp = await _dio.get(ApiConstants.taskTimeEntries(taskId),
          queryParameters: {'page': page, 'pageSize': pageSize});
      final data = unwrap(resp) as Map<String, dynamic>;
      final items = (data['items'] as List<dynamic>? ?? []);
      return items
          .map((e) => TimeEntryModel.fromJson(e as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<TaskTimeSummaryModel> summary(String taskId) async {
    try {
      final resp = await _dio.get(ApiConstants.taskTimeSummary(taskId));
      return TaskTimeSummaryModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  /// Manual entry. [date] is "yyyy-MM-dd" (the server's DateOnly wire form).
  Future<TimeEntryModel> create(
    String taskId, {
    required String date,
    required int durationMinutes,
    bool isBillable = false,
    String? description,
  }) async {
    try {
      final resp = await _dio.post(ApiConstants.taskTimeEntries(taskId), data: {
        'date': date,
        'durationMinutes': durationMinutes,
        'isBillable': isBillable,
        if (description != null && description.isNotEmpty) 'description': description,
      });
      return TimeEntryModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<TimeEntryModel> startTimer(String taskId,
      {bool isBillable = false, String? description}) async {
    try {
      final resp = await _dio.post(ApiConstants.taskTimerStart(taskId), data: {
        'isBillable': isBillable,
        if (description != null && description.isNotEmpty) 'description': description,
      });
      return TimeEntryModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<TimeEntryModel> stopTimer() async {
    try {
      final resp = await _dio.post(ApiConstants.timerStop);
      return TimeEntryModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<TimeEntryModel> update(
    String id, {
    required String date,
    required int durationMinutes,
    bool isBillable = false,
    String? description,
  }) async {
    try {
      final resp = await _dio.put(ApiConstants.timeEntry(id), data: {
        'date': date,
        'durationMinutes': durationMinutes,
        'isBillable': isBillable,
        if (description != null && description.isNotEmpty) 'description': description,
      });
      return TimeEntryModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<void> delete(String id) async {
    try {
      final resp = await _dio.delete(ApiConstants.timeEntry(id));
      unwrap(resp);
    } catch (e) {
      throw mapDioError(e);
    }
  }
}
