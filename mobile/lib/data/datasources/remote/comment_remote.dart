import 'package:dio/dio.dart';

import '../../../core/constants/api_constants.dart';
import '../../../core/network/response_handler.dart';
import '../../models/comment_model.dart';

/// Task comments are server-owned (not offline-synced); a thin online remote
/// over the task-scoped comments endpoints. Mentions are optional and omitted
/// by this client (the server accepts an empty mention set).
class CommentRemote {
  CommentRemote(this._dio);
  final Dio _dio;

  Future<List<CommentModel>> list(String taskId,
      {int page = 1, int pageSize = 100}) async {
    try {
      final resp = await _dio.get(ApiConstants.taskComments(taskId),
          queryParameters: {'page': page, 'pageSize': pageSize});
      final data = unwrap(resp) as Map<String, dynamic>;
      final items = (data['items'] as List<dynamic>? ?? []);
      return items
          .map((e) => CommentModel.fromJson(e as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<CommentModel> create(String taskId, String body,
      {String? parentCommentId}) async {
    try {
      final resp = await _dio.post(ApiConstants.taskComments(taskId), data: {
        'body': body,
        'parentCommentId': ?parentCommentId,
      });
      return CommentModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<CommentModel> update(String id, String body) async {
    try {
      final resp = await _dio.put(ApiConstants.comment(id), data: {'body': body});
      return CommentModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<void> delete(String id) async {
    try {
      final resp = await _dio.delete(ApiConstants.comment(id));
      unwrap(resp);
    } catch (e) {
      throw mapDioError(e);
    }
  }
}
