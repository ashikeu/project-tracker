/// Mirror of the backend's response envelope: { success, data, message, errors }.
/// List endpoints put a paged object in [data]: { items, page, pageSize, totalCount, totalPages }.
class ApiEnvelope {
  final bool success;
  final dynamic data;
  final String? message;
  final List<ApiFieldError> errors;

  ApiEnvelope({required this.success, this.data, this.message, this.errors = const []});

  factory ApiEnvelope.fromJson(Map<String, dynamic> json) => ApiEnvelope(
        success: json['success'] == true,
        data: json['data'],
        message: json['message'] as String?,
        errors: (json['errors'] as List<dynamic>? ?? [])
            .map((e) => ApiFieldError.fromJson(e as Map<String, dynamic>))
            .toList(),
      );

  Map<String, String> get fieldErrorMap => {
        for (final e in errors) e.field: e.message,
      };
}

class ApiFieldError {
  final String field;
  final String message;
  ApiFieldError(this.field, this.message);

  factory ApiFieldError.fromJson(Map<String, dynamic> json) =>
      ApiFieldError((json['field'] ?? '').toString(), (json['message'] ?? '').toString());
}
