import 'package:dio/dio.dart';

import '../../../core/constants/api_constants.dart';
import '../../../core/network/response_handler.dart';
import '../../models/auth_models.dart';

/// Thin wrapper over the /auth endpoints. The login/refresh calls are marked
/// `noAuth` so the interceptor doesn't attach a (possibly stale) bearer token.
class AuthRemote {
  AuthRemote(this._dio);
  final Dio _dio;

  Future<AuthResult> login(String email, String password, {String? organizationId}) async {
    try {
      final resp = await _dio.post(
        ApiConstants.login,
        data: {
          'email': email,
          'password': password,
          'organizationId': ?organizationId,
        },
        options: Options(extra: {'noAuth': true}),
      );
      final data = unwrap(resp) as Map<String, dynamic>;
      return AuthResult.fromJson(data);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<void> logout(String refreshToken) async {
    try {
      await _dio.post(
        ApiConstants.logout,
        data: {'refreshToken': refreshToken},
        options: Options(extra: {'noAuth': true}),
      );
    } catch (_) {
      // Logout is best-effort; a failed server logout still clears local state.
    }
  }
}
