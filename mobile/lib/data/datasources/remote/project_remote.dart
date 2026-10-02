import 'package:dio/dio.dart';

import '../../../core/constants/api_constants.dart';
import '../../../core/network/response_handler.dart';
import '../../models/project_model.dart';

/// Reads projects, their board columns, and members. These aren't offline-edited,
/// so there's only a read path here; the repository caches results to Drift.
class ProjectRemote {
  ProjectRemote(this._dio);
  final Dio _dio;

  Future<List<ProjectModel>> getProjects({int page = 1, int pageSize = 50}) async {
    try {
      final resp = await _dio.get(
        ApiConstants.projects,
        queryParameters: {'page': page, 'pageSize': pageSize},
      );
      final data = unwrap(resp) as Map<String, dynamic>;
      final items = (data['items'] as List<dynamic>? ?? []);
      return items.map((e) => ProjectModel.fromJson(e as Map<String, dynamic>)).toList();
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<ProjectModel> createProject({
    required String key,
    required String name,
    String? description,
  }) async {
    try {
      final resp = await _dio.post(ApiConstants.projects, data: {
        'key': key,
        'name': name,
        if (description != null && description.isNotEmpty) 'description': description,
      });
      final data = unwrap(resp) as Map<String, dynamic>;
      return ProjectModel.fromJson(data);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<List<BoardColumnModel>> getBoardColumns(String projectId) async {
    try {
      final resp = await _dio.get(ApiConstants.boardColumns(projectId));
      final data = unwrap(resp);
      final list = data is List ? data : (data as Map<String, dynamic>)['items'] as List<dynamic>;
      return list.map((e) => BoardColumnModel.fromJson(e as Map<String, dynamic>)).toList();
    } catch (e) {
      throw mapDioError(e);
    }
  }
}
