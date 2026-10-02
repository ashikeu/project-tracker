import 'package:dio/dio.dart';

import '../../../core/constants/api_constants.dart';
import '../../../core/network/response_handler.dart';
import '../../models/sync_models.dart';

/// The two sync endpoints: push a batch of operations, pull a delta of changes.
class SyncRemote {
  SyncRemote(this._dio);
  final Dio _dio;

  Future<List<SyncOperationResult>> push(List<SyncOperationInput> operations) async {
    try {
      final resp = await _dio.post(ApiConstants.syncPush, data: {
        'operations': operations.map((o) => o.toJson()).toList(),
      });
      final data = unwrap(resp) as Map<String, dynamic>;
      final results = (data['results'] as List<dynamic>? ?? []);
      return results.map((e) => SyncOperationResult.fromJson(e as Map<String, dynamic>)).toList();
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<SyncPullResult> pull({required int since, int pageSize = 200}) async {
    try {
      final resp = await _dio.get(
        ApiConstants.syncPull,
        queryParameters: {'since': since, 'pageSize': pageSize},
      );
      final data = unwrap(resp) as Map<String, dynamic>;
      return SyncPullResult.fromJson(data);
    } catch (e) {
      throw mapDioError(e);
    }
  }
}
