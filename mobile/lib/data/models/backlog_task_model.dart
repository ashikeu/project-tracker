/// A lightweight read model for the product backlog list. The backlog endpoint
/// returns the full server TaskDto; this keeps only the fields the backlog UI
/// renders, so the client isn't coupled to every TaskDto field.
class BacklogTaskModel {
  final String id;
  final String taskKey;
  final String title;
  final String status;
  final String priority;
  final String taskType;
  final int? storyPoints;
  final String? assigneeName;
  final double position;

  BacklogTaskModel({
    required this.id,
    required this.taskKey,
    required this.title,
    required this.status,
    required this.priority,
    required this.taskType,
    this.storyPoints,
    this.assigneeName,
    required this.position,
  });

  factory BacklogTaskModel.fromJson(Map<String, dynamic> j) => BacklogTaskModel(
        id: j['id'] as String,
        taskKey: (j['taskKey'] ?? '') as String,
        title: (j['title'] ?? '') as String,
        status: (j['status'] ?? '') as String,
        priority: (j['priority'] ?? '') as String,
        taskType: (j['taskType'] ?? '') as String,
        storyPoints: j['storyPoints'] as int?,
        assigneeName: j['assigneeName'] as String?,
        position: (j['position'] as num?)?.toDouble() ?? 0,
      );
}
