import 'package:dio/dio.dart';

import '../constants/api_constants.dart';
import '../storage/secure_storage_service.dart';

/// Signals that the session is unrecoverable (refresh failed) so the app can
/// route back to login. The AuthController listens to this.
typedef OnSessionExpired = void Function();

/// Builds the app's single configured [Dio]. Two interceptors:
///  - attaches the bearer token to every request;
///  - on a 401, transparently refreshes the token once and replays the request;
///    if refresh also fails, clears the session and fires [onSessionExpired].
class DioClient {
  DioClient(this._storage, {this.onSessionExpired}) {
    dio = Dio(BaseOptions(
      baseUrl: ApiConstants.baseUrl,
      connectTimeout: const Duration(seconds: 15),
      receiveTimeout: const Duration(seconds: 20),
      // Let us inspect non-2xx bodies (the envelope) instead of throwing on them.
      validateStatus: (code) => code != null && code < 500,
      contentType: 'application/json',
    ));
    dio.interceptors.add(InterceptorsWrapper(
      onRequest: _onRequest,
      onError: _onError,
    ));
  }

  final SecureStorageService _storage;
  final OnSessionExpired? onSessionExpired;
  late final Dio dio;

  bool _refreshing = false;

  Future<void> _onRequest(RequestOptions options, RequestInterceptorHandler handler) async {
    if (options.extra['noAuth'] != true) {
      final token = await _storage.accessToken;
      if (token != null) {
        options.headers['Authorization'] = 'Bearer $token';
      }
    }
    handler.next(options);
  }

  Future<void> _onError(DioException err, ErrorInterceptorHandler handler) async {
    final status = err.response?.statusCode;
    final isAuthEndpoint = err.requestOptions.path.contains('/auth/');

    if (status == 401 && !isAuthEndpoint && err.requestOptions.extra['retried'] != true) {
      final refreshed = await _tryRefresh();
      if (refreshed) {
        try {
          final response = await _replay(err.requestOptions);
          return handler.resolve(response);
        } on DioException catch (e) {
          return handler.next(e);
        }
      } else {
        await _storage.clear();
        onSessionExpired?.call();
      }
    }
    handler.next(err);
  }

  Future<bool> _tryRefresh() async {
    if (_refreshing) return false; // avoid stampede; the first refresh wins
    _refreshing = true;
    try {
      final refreshToken = await _storage.refreshToken;
      if (refreshToken == null) return false;

      // A bare Dio with no interceptors, so a 401 here can't recurse.
      final bare = Dio(BaseOptions(baseUrl: ApiConstants.baseUrl));
      final resp = await bare.post(
        ApiConstants.refreshToken,
        data: {'refreshToken': refreshToken},
      );
      final body = resp.data;
      if (resp.statusCode == 200 && body is Map && body['success'] == true) {
        final data = body['data'] as Map<String, dynamic>;
        await _storage.updateTokens(
          accessToken: data['accessToken'] as String,
          refreshToken: data['refreshToken'] as String,
          accessExpiresAt: DateTime.parse(data['accessTokenExpiresAt'] as String),
        );
        return true;
      }
      return false;
    } catch (_) {
      return false;
    } finally {
      _refreshing = false;
    }
  }

  Future<Response<dynamic>> _replay(RequestOptions options) async {
    final token = await _storage.accessToken;
    options.headers['Authorization'] = 'Bearer $token';
    options.extra['retried'] = true;
    return dio.fetch(options);
  }
}
