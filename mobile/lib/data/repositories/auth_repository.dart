import '../../core/storage/secure_storage_service.dart';
import '../datasources/remote/auth_remote.dart';
import '../models/auth_models.dart';

/// Owns the sign-in lifecycle and the persisted session. On success it writes
/// tokens to secure storage; the DioClient reads them from there on every call.
class AuthRepository {
  AuthRepository(this._remote, this._storage);

  final AuthRemote _remote;
  final SecureStorageService _storage;

  Future<AuthResult> login(String email, String password) async {
    final result = await _remote.login(email, password);
    final org = result.primaryOrg;
    await _storage.saveSession(
      accessToken: result.accessToken,
      refreshToken: result.refreshToken,
      accessExpiresAt: result.accessTokenExpiresAt,
      userId: result.user.id,
      organizationId: org?.organizationId ?? '',
      userName: result.user.fullName,
    );
    return result;
  }

  Future<void> logout() async {
    final token = await _storage.refreshToken;
    if (token != null) {
      await _remote.logout(token);
    }
    await _storage.clear();
  }

  Future<bool> hasSession() => _storage.hasSession();
  Future<String?> currentUserId() => _storage.userId;
  Future<String?> currentUserName() => _storage.userName;
}
