/// One outbound operation in a /sync/push batch. [payloadJson] is a *string* of
/// JSON (the server deserializes it itself), matching SyncOperationInput.
class SyncOperationInput {
  final String operationId;
  final String entityType;
  final String entityId;
  final String changeType; // created | updated | deleted
  final String? payloadJson;
  final String? baseRowVersion;

  SyncOperationInput({
    required this.operationId,
    this.entityType = 'Task',
    required this.entityId,
    required this.changeType,
    this.payloadJson,
    this.baseRowVersion,
  });

  Map<String, dynamic> toJson() => {
        'operationId': operationId,
        'entityType': entityType,
        'entityId': entityId,
        'changeType': changeType,
        'payloadJson': payloadJson,
        'baseRowVersion': baseRowVersion,
      };
}

/// Per-op result from /sync/push: Applied | Conflict | Rejected.
class SyncOperationResult {
  final String operationId;
  final String status;
  final String? message;
  final String? taskKey;
  final String? rowVersion;
  final String? serverPayloadJson;

  SyncOperationResult({
    required this.operationId,
    required this.status,
    this.message,
    this.taskKey,
    this.rowVersion,
    this.serverPayloadJson,
  });

  bool get isApplied => status == 'Applied';
  bool get isConflict => status == 'Conflict';
  bool get isRejected => status == 'Rejected';

  factory SyncOperationResult.fromJson(Map<String, dynamic> j) => SyncOperationResult(
        operationId: j['operationId'] as String,
        status: (j['status'] ?? '') as String,
        message: j['message'] as String?,
        taskKey: j['taskKey'] as String?,
        rowVersion: j['rowVersion'] as String?,
        serverPayloadJson: j['serverPayloadJson'] as String?,
      );
}

/// One row from /sync/pull. A null [payloadJson] is a tombstone (delete).
class SyncChangeDto {
  final int serverVersion;
  final String entityType;
  final String entityId;
  final String changeType;
  final String? payloadJson;

  SyncChangeDto({
    required this.serverVersion,
    required this.entityType,
    required this.entityId,
    required this.changeType,
    this.payloadJson,
  });

  bool get isTombstone => payloadJson == null;

  factory SyncChangeDto.fromJson(Map<String, dynamic> j) => SyncChangeDto(
        serverVersion: (j['serverVersion'] as num).toInt(),
        entityType: (j['entityType'] ?? '') as String,
        entityId: j['entityId'] as String,
        changeType: (j['changeType'] ?? '') as String,
        payloadJson: j['payloadJson'] as String?,
      );
}

class SyncPullResult {
  final List<SyncChangeDto> changes;
  final int nextVersion;
  final bool hasMore;

  SyncPullResult({required this.changes, required this.nextVersion, required this.hasMore});

  factory SyncPullResult.fromJson(Map<String, dynamic> j) => SyncPullResult(
        changes: (j['changes'] as List<dynamic>? ?? [])
            .map((e) => SyncChangeDto.fromJson(e as Map<String, dynamic>))
            .toList(),
        nextVersion: (j['nextVersion'] as num?)?.toInt() ?? 0,
        hasMore: j['hasMore'] == true,
      );
}
