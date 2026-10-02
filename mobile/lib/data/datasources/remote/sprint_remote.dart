import 'package:dio/dio.dart';

import '../../../core/constants/api_constants.dart';
import '../../../core/network/response_handler.dart';
import '../../models/sprint_model.dart';

/// Sprints are server-owned (not part of the offline sync scope); a thin online
/// remote over the /sprints endpoints. Status transitions go through the
/// dedicated start/complete/cancel actions — the server owns the state machine.
class SprintRemote {
  SprintRemote(this._dio);
  final Dio _dio;

  Future<List<SprintModel>> list(String projectId, {String? status}) async {
    try {
      final resp = await _dio.get(ApiConstants.sprints, queryParameters: {
        'projectId': projectId,
        'status': ?status,
      });
      final data = unwrap(resp) as List<dynamic>;
      return data
          .map((e) => SprintModel.fromJson(e as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<SprintModel> create({
    required String projectId,
    required String name,
    String? goal,
    String? startDate,
    String? endDate,
  }) async {
    try {
      final resp = await _dio.post(ApiConstants.sprints, data: {
        'projectId': projectId,
        'name': name,
        if (goal != null && goal.isNotEmpty) 'goal': goal,
        'startDate': ?startDate,
        'endDate': ?endDate,
      });
      return SprintModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<SprintModel> update({
    required String id,
    required String name,
    String? goal,
    String? startDate,
    String? endDate,
  }) async {
    try {
      final resp = await _dio.put(ApiConstants.sprint(id), data: {
        'name': name,
        if (goal != null && goal.isNotEmpty) 'goal': goal,
        'startDate': ?startDate,
        'endDate': ?endDate,
      });
      return SprintModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<SprintModel> start(String id) async {
    try {
      final resp = await _dio.post(ApiConstants.sprintStart(id));
      return SprintModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<SprintModel> complete(String id, {bool moveIncompleteToBacklog = true}) async {
    try {
      final resp = await _dio.post(ApiConstants.sprintComplete(id),
          data: {'moveIncompleteToBacklog': moveIncompleteToBacklog});
      return SprintModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<SprintModel> cancel(String id) async {
    try {
      final resp = await _dio.post(ApiConstants.sprintCancel(id));
      return SprintModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<void> delete(String id) async {
    try {
      final resp = await _dio.delete(ApiConstants.sprint(id));
      unwrap(resp);
    } catch (e) {
      throw mapDioError(e);
    }
  }
}
