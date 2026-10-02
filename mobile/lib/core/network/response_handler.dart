import 'package:dio/dio.dart';

import '../errors/failure.dart';
import 'api_envelope.dart';

/// Turns a raw Dio [Response] carrying the standard envelope into either the
/// unwrapped `data`, or a thrown typed [Failure]. Centralizes status→Failure
/// mapping so every remote datasource handles errors identically.
dynamic unwrap(Response<dynamic> response) {
  final status = response.statusCode ?? 0;
  final body = response.data;
  final envelope = body is Map<String, dynamic> ? ApiEnvelope.fromJson(body) : null;

  if (status >= 200 && status < 300 && (envelope?.success ?? false)) {
    return envelope!.data;
  }

  final message = envelope?.message ?? 'Request failed ($status).';
  switch (status) {
    case 401:
      throw AuthFailure(message);
    case 403:
      throw ServerFailure('You do not have permission to do that.', statusCode: 403);
    case 422:
      throw ValidationFailure(
        message.isEmpty ? 'Please check the highlighted fields.' : message,
        fieldErrors: envelope?.fieldErrorMap ?? const {},
      );
    default:
      throw ServerFailure(message, statusCode: status);
  }
}

/// Maps transport-level Dio errors (no envelope) to Failures.
Failure mapDioError(Object error) {
  if (error is Failure) return error;
  if (error is DioException) {
    switch (error.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
      case DioExceptionType.connectionError:
        return const NetworkFailure();
      case DioExceptionType.badResponse:
        // A 5xx that slipped past validateStatus, or a non-envelope body.
        final code = error.response?.statusCode;
        return ServerFailure('Server error ($code).', statusCode: code);
      default:
        if (error.error is Failure) return error.error as Failure;
        return const NetworkFailure();
    }
  }
  return const UnexpectedFailure();
}
