/// Mirrors the server ProjectDto (the fields the client actually uses).
class ProjectModel {
  final String id;
  final String key;
  final String name;
  final String? description;
  final String status;
  final String priority;
  final bool isArchived;

  ProjectModel({
    required this.id,
    required this.key,
    required this.name,
    this.description,
    this.status = 'Active',
    this.priority = 'Medium',
    this.isArchived = false,
  });

  factory ProjectModel.fromJson(Map<String, dynamic> j) => ProjectModel(
        id: j['id'] as String,
        key: (j['key'] ?? '') as String,
        name: (j['name'] ?? '') as String,
        description: j['description'] as String?,
        status: (j['status'] ?? 'Active') as String,
        priority: (j['priority'] ?? 'Medium') as String,
        isArchived: j['isArchived'] == true,
      );
}

/// Mirrors the server BoardColumnDto.
class BoardColumnModel {
  final String id;
  final String projectId;
  final String name;
  final String color;
  final int order;
  final int? wipLimit;
  final String statusMapping;
  final bool isCompletionColumn;

  BoardColumnModel({
    required this.id,
    required this.projectId,
    required this.name,
    this.color = '#6B7280',
    this.order = 0,
    this.wipLimit,
    this.statusMapping = 'Todo',
    this.isCompletionColumn = false,
  });

  factory BoardColumnModel.fromJson(Map<String, dynamic> j) => BoardColumnModel(
        id: j['id'] as String,
        projectId: j['projectId'] as String,
        name: (j['name'] ?? '') as String,
        color: (j['color'] ?? '#6B7280') as String,
        order: (j['order'] ?? 0) as int,
        wipLimit: j['wipLimit'] as int?,
        statusMapping: (j['statusMapping'] ?? 'Todo') as String,
        isCompletionColumn: j['isCompletionColumn'] == true,
      );
}
