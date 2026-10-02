import 'package:dio/dio.dart';

import '../../../core/constants/api_constants.dart';
import '../../../core/network/response_handler.dart';
import '../../models/notification_model.dart';

/// Notifications are server-owned and read online (not part of the offline sync
/// scope). A thin remote over the /notifications endpoints.
class NotificationRemote {
  NotificationRemote(this._dio);
  final Dio _dio;

  Future<List<NotificationModel>> list({bool unreadOnly = false, int page = 1, int pageSize = 30}) async {
    try {
      final resp = await _dio.get(ApiConstants.notifications, queryParameters: {
        'unreadOnly': unreadOnly,
        'page': page,
        'pageSize': pageSize,
      });
      final data = unwrap(resp) as Map<String, dynamic>;
      final items = (data['items'] as List<dynamic>? ?? []);
      return items.map((e) => NotificationModel.fromJson(e as Map<String, dynamic>)).toList();
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<int> unreadCount() async {
    try {
      final resp = await _dio.get(ApiConstants.notificationsUnreadCount);
      final data = unwrap(resp) as Map<String, dynamic>;
      return (data['unreadCount'] as num?)?.toInt() ?? 0;
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<void> markRead(String id) async {
    try {
      await _dio.post('${ApiConstants.notifications}/$id/read');
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<void> markAllRead() async {
    try {
      await _dio.post('${ApiConstants.notifications}/read-all');
    } catch (e) {
      throw mapDioError(e);
    }
  }
}
