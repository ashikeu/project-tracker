/// Typed failures surfaced from the data layer to controllers, so the UI can
/// react differently to "no network" vs "server said no" vs "bad input".
sealed class Failure {
  final String message;
  const Failure(this.message);

  @override
  String toString() => message;
}

/// No connectivity, or the request timed out / the host was unreachable.
class NetworkFailure extends Failure {
  const NetworkFailure([super.message = 'You appear to be offline.']);
}

/// The server returned a non-2xx with a message (400/401/403/404/500).
class ServerFailure extends Failure {
  final int? statusCode;
  const ServerFailure(super.message, {this.statusCode});
}

/// 422 validation — field-level errors from FluentValidation.
class ValidationFailure extends Failure {
  final Map<String, String> fieldErrors;
  const ValidationFailure(super.message, {this.fieldErrors = const {}});
}

/// Credentials rejected / token expired and could not be refreshed.
class AuthFailure extends Failure {
  const AuthFailure([super.message = 'Your session has expired. Please sign in again.']);
}

/// Anything the client caused locally (decode error, DB error).
class UnexpectedFailure extends Failure {
  const UnexpectedFailure([super.message = 'Something went wrong.']);
}
