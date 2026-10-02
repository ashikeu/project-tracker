/// Client mirror of the server's TaskSyncPayload — the wire shape used by both
/// /sync/pull (incoming) and /sync/push (outgoing). Enum fields are ints; dates
/// are DateOnly ("yyyy-MM-dd"); rowVersion is the base64 concurrency token.
class TaskPayload {
  final String id;
  final String taskKey;
  final String projectId;
  final String? epicId;
  final String? sprintId;
  final String? parentTaskId;
  final String title;
  final String? description;
  final int taskType;
  final int status;
  final int priority;
  final String? assigneeUserId;
  final String? reporterUserId;
  final String? teamId;
  final String? boardColumnId;
  final int? storyPoints;
  final double? estimatedHours;
  final double? remainingHours;
  final double loggedHours;
  final DateTime? startDate;
  final DateTime? dueDate;
  final double position;
  final bool isDeleted;
  final String rowVersion;
  final DateTime? updatedAt;

  TaskPayload({
    required this.id,
    this.taskKey = '',
    required this.projectId,
    this.epicId,
    this.sprintId,
    this.parentTaskId,
    required this.title,
    this.description,
    this.taskType = 0,
    this.status = 0,
    this.priority = 2,
    this.assigneeUserId,
    this.reporterUserId,
    this.teamId,
    this.boardColumnId,
    this.storyPoints,
    this.estimatedHours,
    this.remainingHours,
    this.loggedHours = 0,
    this.startDate,
    this.dueDate,
    this.position = 0,
    this.isDeleted = false,
    this.rowVersion = '',
    this.updatedAt,
  });

  /// Tolerant of both camelCase (our push payload) and PascalCase: the server
  /// serializes sync payloads with the default JsonSerializer (PascalCase keys),
  /// while the REST envelope is camelCase. We key everything off lowercased names.
  factory TaskPayload.fromJson(Map<String, dynamic> raw) {
    final j = {for (final e in raw.entries) e.key.toLowerCase(): e.value};
    return TaskPayload(
      id: j['id'] as String,
      taskKey: (j['taskkey'] ?? '') as String,
      projectId: j['projectid'] as String,
      epicId: j['epicid'] as String?,
      sprintId: j['sprintid'] as String?,
      parentTaskId: j['parenttaskid'] as String?,
      title: (j['title'] ?? '') as String,
      description: j['description'] as String?,
      taskType: (j['tasktype'] ?? 0) as int,
      status: (j['status'] ?? 0) as int,
      priority: (j['priority'] ?? 2) as int,
      assigneeUserId: j['assigneeuserid'] as String?,
      reporterUserId: j['reporteruserid'] as String?,
      teamId: j['teamid'] as String?,
      boardColumnId: j['boardcolumnid'] as String?,
      storyPoints: (j['storypoints'] as num?)?.toInt(),
      estimatedHours: _toDouble(j['estimatedhours']),
      remainingHours: _toDouble(j['remaininghours']),
      loggedHours: _toDouble(j['loggedhours']) ?? 0,
      startDate: _toDate(j['startdate']),
      dueDate: _toDate(j['duedate']),
      position: _toDouble(j['position']) ?? 0,
      isDeleted: j['isdeleted'] == true,
      rowVersion: (j['rowversion'] ?? '') as String,
      updatedAt: j['updatedat'] == null ? null : DateTime.parse(j['updatedat'] as String),
    );
  }

  /// What the client sends up in `payloadJson`. The server ignores server-owned
  /// fields (taskKey/rowVersion/reporter) on create, but sending them is harmless
  /// and keeps the shape symmetric.
  Map<String, dynamic> toJson() => {
        'id': id,
        'taskKey': taskKey,
        'projectId': projectId,
        'epicId': epicId,
        'sprintId': sprintId,
        'parentTaskId': parentTaskId,
        'title': title,
        'description': description,
        'taskType': taskType,
        'status': status,
        'priority': priority,
        'assigneeUserId': assigneeUserId,
        // Omit when null: the server's ReporterUserId is a non-nullable Guid
        // (an explicit null fails to deserialize). On create the server assigns
        // it from the authenticated user regardless.
        if (reporterUserId != null) 'reporterUserId': reporterUserId,
        'teamId': teamId,
        'boardColumnId': boardColumnId,
        'storyPoints': storyPoints,
        'estimatedHours': estimatedHours,
        'remainingHours': remainingHours,
        'loggedHours': loggedHours,
        'startDate': _dateOnly(startDate),
        'dueDate': _dateOnly(dueDate),
        'position': position,
        'isDeleted': isDeleted,
        'rowVersion': rowVersion,
      };

  static double? _toDouble(dynamic v) {
    if (v == null) return null;
    if (v is num) return v.toDouble();
    return double.tryParse(v.toString());
  }

  /// Server DateOnly arrives as "2026-10-01"; be tolerant of a full timestamp too.
  static DateTime? _toDate(dynamic v) {
    if (v == null) return null;
    final s = v.toString();
    return DateTime.tryParse(s.length > 10 ? s : '${s}T00:00:00');
  }

  static String? _dateOnly(DateTime? d) {
    if (d == null) return null;
    final m = d.month.toString().padLeft(2, '0');
    final day = d.day.toString().padLeft(2, '0');
    return '${d.year}-$m-$day';
  }
}
