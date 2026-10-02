import 'package:dio/dio.dart';

import '../../../core/network/response_handler.dart';
import '../../models/dashboard_model.dart';

/// Reads the server-side aggregated dashboard for a project (online-only).
class DashboardRemote {
  DashboardRemote(this._dio);
  final Dio _dio;

  Future<DashboardModel> getDashboard(String projectId) async {
    try {
      final resp = await _dio.get('/dashboard/$projectId');
      final data = unwrap(resp) as Map<String, dynamic>;
      return DashboardModel.fromJson(data);
    } catch (e) {
      throw mapDioError(e);
    }
  }
}
