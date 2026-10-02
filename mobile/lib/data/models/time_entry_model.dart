/// Mirrors the server TimeEntryDto. [date] is the server's `DateOnly`
/// ("yyyy-MM-dd"); a running timer has [isRunningTimer] true and no finalized
/// [durationMinutes] yet (it shows 0 until stopped).
class TimeEntryModel {
  final String id;
  final String taskId;
  final String userId;
  final String userName;
  final String date;
  final int durationMinutes;
  final bool isBillable;
  final String? description;
  final bool isRunningTimer;
  final DateTime? startedAt;
  final DateTime createdAt;

  TimeEntryModel({
    required this.id,
    required this.taskId,
    required this.userId,
    required this.userName,
    required this.date,
    required this.durationMinutes,
    required this.isBillable,
    this.description,
    required this.isRunningTimer,
    this.startedAt,
    required this.createdAt,
  });

  /// "1h 30m" / "45m" style label from the finalized minutes.
  String get durationLabel {
    final h = durationMinutes ~/ 60;
    final m = durationMinutes % 60;
    if (h > 0 && m > 0) return '${h}h ${m}m';
    if (h > 0) return '${h}h';
    return '${m}m';
  }

  factory TimeEntryModel.fromJson(Map<String, dynamic> j) => TimeEntryModel(
        id: j['id'] as String,
        taskId: j['taskId'] as String,
        userId: (j['userId'] ?? '') as String,
        userName: (j['userName'] ?? '') as String,
        date: (j['date'] ?? '') as String,
        durationMinutes: (j['durationMinutes'] as num?)?.toInt() ?? 0,
        isBillable: (j['isBillable'] ?? false) as bool,
        description: j['description'] as String?,
        isRunningTimer: (j['isRunningTimer'] ?? false) as bool,
        startedAt: j['startedAt'] == null
            ? null
            : DateTime.tryParse(j['startedAt'] as String),
        createdAt: DateTime.tryParse((j['createdAt'] ?? '') as String) ??
            DateTime.fromMillisecondsSinceEpoch(0),
      );
}

/// Mirrors the server TaskTimeSummaryDto: the rolled-up estimate/logged/remaining
/// hours for a task.
class TaskTimeSummaryModel {
  final String taskId;
  final double? estimatedHours;
  final double loggedHours;
  final double? remainingHours;

  TaskTimeSummaryModel({
    required this.taskId,
    this.estimatedHours,
    required this.loggedHours,
    this.remainingHours,
  });

  factory TaskTimeSummaryModel.fromJson(Map<String, dynamic> j) =>
      TaskTimeSummaryModel(
        taskId: j['taskId'] as String,
        estimatedHours: (j['estimatedHours'] as num?)?.toDouble(),
        loggedHours: (j['loggedHours'] as num?)?.toDouble() ?? 0,
        remainingHours: (j['remainingHours'] as num?)?.toDouble(),
      );
}
