import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// Tokens live only in the platform secure store (Keychain / Keystore), never
/// in SharedPreferences or the Drift DB — per the spec's "no tokens in non-secure
/// storage on the client" rule.
class SecureStorageService {
  SecureStorageService(this._storage);

  final FlutterSecureStorage _storage;

  static const _kAccessToken = 'access_token';
  static const _kRefreshToken = 'refresh_token';
  static const _kAccessExpiry = 'access_expiry';
  static const _kUserId = 'user_id';
  static const _kOrgId = 'org_id';
  static const _kUserName = 'user_name';

  Future<void> saveSession({
    required String accessToken,
    required String refreshToken,
    required DateTime accessExpiresAt,
    required String userId,
    required String organizationId,
    required String userName,
  }) async {
    await Future.wait([
      _storage.write(key: _kAccessToken, value: accessToken),
      _storage.write(key: _kRefreshToken, value: refreshToken),
      _storage.write(key: _kAccessExpiry, value: accessExpiresAt.toIso8601String()),
      _storage.write(key: _kUserId, value: userId),
      _storage.write(key: _kOrgId, value: organizationId),
      _storage.write(key: _kUserName, value: userName),
    ]);
  }

  Future<void> updateTokens({
    required String accessToken,
    required String refreshToken,
    required DateTime accessExpiresAt,
  }) async {
    await Future.wait([
      _storage.write(key: _kAccessToken, value: accessToken),
      _storage.write(key: _kRefreshToken, value: refreshToken),
      _storage.write(key: _kAccessExpiry, value: accessExpiresAt.toIso8601String()),
    ]);
  }

  Future<String?> get accessToken => _storage.read(key: _kAccessToken);
  Future<String?> get refreshToken => _storage.read(key: _kRefreshToken);
  Future<String?> get userId => _storage.read(key: _kUserId);
  Future<String?> get organizationId => _storage.read(key: _kOrgId);
  Future<String?> get userName => _storage.read(key: _kUserName);

  Future<bool> hasSession() async => (await accessToken) != null;

  Future<void> clear() => _storage.deleteAll();
}
