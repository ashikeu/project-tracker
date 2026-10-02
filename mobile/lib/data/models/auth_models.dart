/// Mirrors AuthResultDto from the backend.
class AuthResult {
  final String accessToken;
  final DateTime accessTokenExpiresAt;
  final String refreshToken;
  final DateTime refreshTokenExpiresAt;
  final UserModel user;
  final List<OrgMembership> organizations;

  AuthResult({
    required this.accessToken,
    required this.accessTokenExpiresAt,
    required this.refreshToken,
    required this.refreshTokenExpiresAt,
    required this.user,
    required this.organizations,
  });

  factory AuthResult.fromJson(Map<String, dynamic> json) => AuthResult(
        accessToken: json['accessToken'] as String,
        accessTokenExpiresAt: DateTime.parse(json['accessTokenExpiresAt'] as String),
        refreshToken: json['refreshToken'] as String,
        refreshTokenExpiresAt: DateTime.parse(json['refreshTokenExpiresAt'] as String),
        user: UserModel.fromJson(json['user'] as Map<String, dynamic>),
        organizations: (json['organizations'] as List<dynamic>? ?? [])
            .map((e) => OrgMembership.fromJson(e as Map<String, dynamic>))
            .toList(),
      );

  /// The membership we issue a session for (first org; login targets one org).
  OrgMembership? get primaryOrg =>
      organizations.isEmpty ? null : organizations.first;
}

class UserModel {
  final String id;
  final String email;
  final String firstName;
  final String lastName;
  final String? avatarUrl;
  final bool isEmailVerified;

  UserModel({
    required this.id,
    required this.email,
    required this.firstName,
    required this.lastName,
    this.avatarUrl,
    required this.isEmailVerified,
  });

  String get fullName => '$firstName $lastName'.trim();

  factory UserModel.fromJson(Map<String, dynamic> json) => UserModel(
        id: json['id'] as String,
        email: json['email'] as String,
        firstName: (json['firstName'] ?? '') as String,
        lastName: (json['lastName'] ?? '') as String,
        avatarUrl: json['avatarUrl'] as String?,
        isEmailVerified: json['isEmailVerified'] == true,
      );
}

class OrgMembership {
  final String organizationId;
  final String organizationName;
  final String organizationSlug;
  final String roleId;
  final String roleName;
  final List<String> permissions;

  OrgMembership({
    required this.organizationId,
    required this.organizationName,
    required this.organizationSlug,
    required this.roleId,
    required this.roleName,
    required this.permissions,
  });

  factory OrgMembership.fromJson(Map<String, dynamic> json) => OrgMembership(
        organizationId: json['organizationId'] as String,
        organizationName: (json['organizationName'] ?? '') as String,
        organizationSlug: (json['organizationSlug'] ?? '') as String,
        roleId: (json['roleId'] ?? '') as String,
        roleName: (json['roleName'] ?? '') as String,
        permissions: (json['permissions'] as List<dynamic>? ?? [])
            .map((e) => e.toString())
            .toList(),
      );
}
