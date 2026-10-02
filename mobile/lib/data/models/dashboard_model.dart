/// Mirrors the server DashboardDto and its nested records.
class DashboardModel {
  final String projectId;
  final String projectName;
  final int totalTasks;
  final int openTasks;
  final int doneTasks;
  final int overdueTasks;
  final List<CountBucket> byStatus;
  final List<CountBucket> byPriority;
  final List<CountBucket> byType;
  final List<WorkloadRow> workload;
  final List<EpicProgressRow> epicProgress;
  final ActiveSprintSummary? activeSprint;

  DashboardModel({
    required this.projectId,
    required this.projectName,
    required this.totalTasks,
    required this.openTasks,
    required this.doneTasks,
    required this.overdueTasks,
    required this.byStatus,
    required this.byPriority,
    required this.byType,
    required this.workload,
    required this.epicProgress,
    this.activeSprint,
  });

  factory DashboardModel.fromJson(Map<String, dynamic> j) => DashboardModel(
        projectId: j['projectId'] as String,
        projectName: (j['projectName'] ?? '') as String,
        totalTasks: (j['totalTasks'] ?? 0) as int,
        openTasks: (j['openTasks'] ?? 0) as int,
        doneTasks: (j['doneTasks'] ?? 0) as int,
        overdueTasks: (j['overdueTasks'] ?? 0) as int,
        byStatus: CountBucket.listFrom(j['byStatus']),
        byPriority: CountBucket.listFrom(j['byPriority']),
        byType: CountBucket.listFrom(j['byType']),
        workload: (j['workload'] as List<dynamic>? ?? [])
            .map((e) => WorkloadRow.fromJson(e as Map<String, dynamic>))
            .toList(),
        epicProgress: (j['epicProgress'] as List<dynamic>? ?? [])
            .map((e) => EpicProgressRow.fromJson(e as Map<String, dynamic>))
            .toList(),
        activeSprint: j['activeSprint'] == null
            ? null
            : ActiveSprintSummary.fromJson(j['activeSprint'] as Map<String, dynamic>),
      );
}

class CountBucket {
  final String key;
  final int count;
  CountBucket(this.key, this.count);

  factory CountBucket.fromJson(Map<String, dynamic> j) =>
      CountBucket((j['key'] ?? '') as String, (j['count'] ?? 0) as int);

  static List<CountBucket> listFrom(dynamic list) =>
      (list as List<dynamic>? ?? []).map((e) => CountBucket.fromJson(e as Map<String, dynamic>)).toList();
}

class WorkloadRow {
  final String? userId;
  final String userName;
  final int openTaskCount;
  final int storyPoints;
  WorkloadRow({this.userId, required this.userName, required this.openTaskCount, required this.storyPoints});

  factory WorkloadRow.fromJson(Map<String, dynamic> j) => WorkloadRow(
        userId: j['userId'] as String?,
        userName: (j['userName'] ?? 'Unassigned') as String,
        openTaskCount: (j['openTaskCount'] ?? 0) as int,
        storyPoints: (j['storyPoints'] ?? 0) as int,
      );
}

class EpicProgressRow {
  final String epicId;
  final String name;
  final int totalTasks;
  final int doneTasks;
  EpicProgressRow({required this.epicId, required this.name, required this.totalTasks, required this.doneTasks});

  double get fraction => totalTasks == 0 ? 0 : doneTasks / totalTasks;

  factory EpicProgressRow.fromJson(Map<String, dynamic> j) => EpicProgressRow(
        epicId: j['epicId'] as String,
        name: (j['name'] ?? '') as String,
        totalTasks: (j['totalTasks'] ?? 0) as int,
        doneTasks: (j['doneTasks'] ?? 0) as int,
      );
}

class ActiveSprintSummary {
  final String id;
  final String name;
  final int taskCount;
  final int doneTaskCount;
  final int committedStoryPoints;
  final int completedStoryPoints;
  ActiveSprintSummary({
    required this.id,
    required this.name,
    required this.taskCount,
    required this.doneTaskCount,
    required this.committedStoryPoints,
    required this.completedStoryPoints,
  });

  factory ActiveSprintSummary.fromJson(Map<String, dynamic> j) => ActiveSprintSummary(
        id: j['id'] as String,
        name: (j['name'] ?? '') as String,
        taskCount: (j['taskCount'] ?? 0) as int,
        doneTaskCount: (j['doneTaskCount'] ?? 0) as int,
        committedStoryPoints: (j['committedStoryPoints'] ?? 0) as int,
        completedStoryPoints: (j['completedStoryPoints'] ?? 0) as int,
      );
}
