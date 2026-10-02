/// Mirrors the server SprintDto. Sprints are server-owned (not offline-synced),
/// so this is a read model plus the fields the create/update forms post back.
/// Date fields are the server's `DateOnly` wire form ("yyyy-MM-dd"), kept as
/// strings here and parsed only where a widget needs a real [DateTime].
class SprintModel {
  final String id;
  final String projectId;
  final String name;
  final String? goal;
  final String status; // Planning | Active | Completed | Cancelled
  final String? startDate;
  final String? endDate;
  final DateTime? completedAt;
  final int taskCount;
  final int completedTaskCount;
  final int committedStoryPoints;
  final int completedStoryPoints;
  final DateTime createdAt;

  SprintModel({
    required this.id,
    required this.projectId,
    required this.name,
    this.goal,
    required this.status,
    this.startDate,
    this.endDate,
    this.completedAt,
    required this.taskCount,
    required this.completedTaskCount,
    required this.committedStoryPoints,
    required this.completedStoryPoints,
    required this.createdAt,
  });

  bool get isActive => status == 'Active';
  bool get isPlanning => status == 'Planning';
  bool get isCompleted => status == 'Completed';
  bool get isCancelled => status == 'Cancelled';

  /// Fraction of tasks done (0..1), for a progress bar.
  double get progress => taskCount == 0 ? 0 : completedTaskCount / taskCount;

  factory SprintModel.fromJson(Map<String, dynamic> j) => SprintModel(
        id: j['id'] as String,
        projectId: j['projectId'] as String,
        name: (j['name'] ?? '') as String,
        goal: j['goal'] as String?,
        status: (j['status'] ?? 'Planning') as String,
        startDate: j['startDate'] as String?,
        endDate: j['endDate'] as String?,
        completedAt: j['completedAt'] == null
            ? null
            : DateTime.tryParse(j['completedAt'] as String),
        taskCount: (j['taskCount'] ?? 0) as int,
        completedTaskCount: (j['completedTaskCount'] ?? 0) as int,
        committedStoryPoints: (j['committedStoryPoints'] ?? 0) as int,
        completedStoryPoints: (j['completedStoryPoints'] ?? 0) as int,
        createdAt: DateTime.tryParse((j['createdAt'] ?? '') as String) ??
            DateTime.fromMillisecondsSinceEpoch(0),
      );
}
