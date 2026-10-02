// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'app_database.dart';

// ignore_for_file: type=lint
class $LocalTasksTable extends LocalTasks
    with TableInfo<$LocalTasksTable, LocalTask> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $LocalTasksTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _idMeta = const VerificationMeta('id');
  @override
  late final GeneratedColumn<String> id = GeneratedColumn<String>(
    'id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _taskKeyMeta = const VerificationMeta(
    'taskKey',
  );
  @override
  late final GeneratedColumn<String> taskKey = GeneratedColumn<String>(
    'task_key',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant(''),
  );
  static const VerificationMeta _projectIdMeta = const VerificationMeta(
    'projectId',
  );
  @override
  late final GeneratedColumn<String> projectId = GeneratedColumn<String>(
    'project_id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _epicIdMeta = const VerificationMeta('epicId');
  @override
  late final GeneratedColumn<String> epicId = GeneratedColumn<String>(
    'epic_id',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _sprintIdMeta = const VerificationMeta(
    'sprintId',
  );
  @override
  late final GeneratedColumn<String> sprintId = GeneratedColumn<String>(
    'sprint_id',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _parentTaskIdMeta = const VerificationMeta(
    'parentTaskId',
  );
  @override
  late final GeneratedColumn<String> parentTaskId = GeneratedColumn<String>(
    'parent_task_id',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _titleMeta = const VerificationMeta('title');
  @override
  late final GeneratedColumn<String> title = GeneratedColumn<String>(
    'title',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _descriptionMeta = const VerificationMeta(
    'description',
  );
  @override
  late final GeneratedColumn<String> description = GeneratedColumn<String>(
    'description',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _taskTypeMeta = const VerificationMeta(
    'taskType',
  );
  @override
  late final GeneratedColumn<int> taskType = GeneratedColumn<int>(
    'task_type',
    aliasedName,
    false,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
    defaultValue: const Constant(0),
  );
  static const VerificationMeta _statusMeta = const VerificationMeta('status');
  @override
  late final GeneratedColumn<int> status = GeneratedColumn<int>(
    'status',
    aliasedName,
    false,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
    defaultValue: const Constant(0),
  );
  static const VerificationMeta _priorityMeta = const VerificationMeta(
    'priority',
  );
  @override
  late final GeneratedColumn<int> priority = GeneratedColumn<int>(
    'priority',
    aliasedName,
    false,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
    defaultValue: const Constant(2),
  );
  static const VerificationMeta _assigneeUserIdMeta = const VerificationMeta(
    'assigneeUserId',
  );
  @override
  late final GeneratedColumn<String> assigneeUserId = GeneratedColumn<String>(
    'assignee_user_id',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _reporterUserIdMeta = const VerificationMeta(
    'reporterUserId',
  );
  @override
  late final GeneratedColumn<String> reporterUserId = GeneratedColumn<String>(
    'reporter_user_id',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _teamIdMeta = const VerificationMeta('teamId');
  @override
  late final GeneratedColumn<String> teamId = GeneratedColumn<String>(
    'team_id',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _boardColumnIdMeta = const VerificationMeta(
    'boardColumnId',
  );
  @override
  late final GeneratedColumn<String> boardColumnId = GeneratedColumn<String>(
    'board_column_id',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _storyPointsMeta = const VerificationMeta(
    'storyPoints',
  );
  @override
  late final GeneratedColumn<int> storyPoints = GeneratedColumn<int>(
    'story_points',
    aliasedName,
    true,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _estimatedHoursMeta = const VerificationMeta(
    'estimatedHours',
  );
  @override
  late final GeneratedColumn<double> estimatedHours = GeneratedColumn<double>(
    'estimated_hours',
    aliasedName,
    true,
    type: DriftSqlType.double,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _remainingHoursMeta = const VerificationMeta(
    'remainingHours',
  );
  @override
  late final GeneratedColumn<double> remainingHours = GeneratedColumn<double>(
    'remaining_hours',
    aliasedName,
    true,
    type: DriftSqlType.double,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _loggedHoursMeta = const VerificationMeta(
    'loggedHours',
  );
  @override
  late final GeneratedColumn<double> loggedHours = GeneratedColumn<double>(
    'logged_hours',
    aliasedName,
    false,
    type: DriftSqlType.double,
    requiredDuringInsert: false,
    defaultValue: const Constant(0),
  );
  static const VerificationMeta _startDateMeta = const VerificationMeta(
    'startDate',
  );
  @override
  late final GeneratedColumn<DateTime> startDate = GeneratedColumn<DateTime>(
    'start_date',
    aliasedName,
    true,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _dueDateMeta = const VerificationMeta(
    'dueDate',
  );
  @override
  late final GeneratedColumn<DateTime> dueDate = GeneratedColumn<DateTime>(
    'due_date',
    aliasedName,
    true,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _positionMeta = const VerificationMeta(
    'position',
  );
  @override
  late final GeneratedColumn<double> position = GeneratedColumn<double>(
    'position',
    aliasedName,
    false,
    type: DriftSqlType.double,
    requiredDuringInsert: false,
    defaultValue: const Constant(0),
  );
  static const VerificationMeta _isDeletedMeta = const VerificationMeta(
    'isDeleted',
  );
  @override
  late final GeneratedColumn<bool> isDeleted = GeneratedColumn<bool>(
    'is_deleted',
    aliasedName,
    false,
    type: DriftSqlType.bool,
    requiredDuringInsert: false,
    defaultConstraints: GeneratedColumn.constraintIsAlways(
      'CHECK ("is_deleted" IN (0, 1))',
    ),
    defaultValue: const Constant(false),
  );
  static const VerificationMeta _rowVersionMeta = const VerificationMeta(
    'rowVersion',
  );
  @override
  late final GeneratedColumn<String> rowVersion = GeneratedColumn<String>(
    'row_version',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant(''),
  );
  static const VerificationMeta _updatedAtMeta = const VerificationMeta(
    'updatedAt',
  );
  @override
  late final GeneratedColumn<DateTime> updatedAt = GeneratedColumn<DateTime>(
    'updated_at',
    aliasedName,
    true,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _syncStateMeta = const VerificationMeta(
    'syncState',
  );
  @override
  late final GeneratedColumn<String> syncState = GeneratedColumn<String>(
    'sync_state',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant('synced'),
  );
  static const VerificationMeta _needsReviewMeta = const VerificationMeta(
    'needsReview',
  );
  @override
  late final GeneratedColumn<bool> needsReview = GeneratedColumn<bool>(
    'needs_review',
    aliasedName,
    false,
    type: DriftSqlType.bool,
    requiredDuringInsert: false,
    defaultConstraints: GeneratedColumn.constraintIsAlways(
      'CHECK ("needs_review" IN (0, 1))',
    ),
    defaultValue: const Constant(false),
  );
  @override
  List<GeneratedColumn> get $columns => [
    id,
    taskKey,
    projectId,
    epicId,
    sprintId,
    parentTaskId,
    title,
    description,
    taskType,
    status,
    priority,
    assigneeUserId,
    reporterUserId,
    teamId,
    boardColumnId,
    storyPoints,
    estimatedHours,
    remainingHours,
    loggedHours,
    startDate,
    dueDate,
    position,
    isDeleted,
    rowVersion,
    updatedAt,
    syncState,
    needsReview,
  ];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'local_tasks';
  @override
  VerificationContext validateIntegrity(
    Insertable<LocalTask> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('id')) {
      context.handle(_idMeta, id.isAcceptableOrUnknown(data['id']!, _idMeta));
    } else if (isInserting) {
      context.missing(_idMeta);
    }
    if (data.containsKey('task_key')) {
      context.handle(
        _taskKeyMeta,
        taskKey.isAcceptableOrUnknown(data['task_key']!, _taskKeyMeta),
      );
    }
    if (data.containsKey('project_id')) {
      context.handle(
        _projectIdMeta,
        projectId.isAcceptableOrUnknown(data['project_id']!, _projectIdMeta),
      );
    } else if (isInserting) {
      context.missing(_projectIdMeta);
    }
    if (data.containsKey('epic_id')) {
      context.handle(
        _epicIdMeta,
        epicId.isAcceptableOrUnknown(data['epic_id']!, _epicIdMeta),
      );
    }
    if (data.containsKey('sprint_id')) {
      context.handle(
        _sprintIdMeta,
        sprintId.isAcceptableOrUnknown(data['sprint_id']!, _sprintIdMeta),
      );
    }
    if (data.containsKey('parent_task_id')) {
      context.handle(
        _parentTaskIdMeta,
        parentTaskId.isAcceptableOrUnknown(
          data['parent_task_id']!,
          _parentTaskIdMeta,
        ),
      );
    }
    if (data.containsKey('title')) {
      context.handle(
        _titleMeta,
        title.isAcceptableOrUnknown(data['title']!, _titleMeta),
      );
    } else if (isInserting) {
      context.missing(_titleMeta);
    }
    if (data.containsKey('description')) {
      context.handle(
        _descriptionMeta,
        description.isAcceptableOrUnknown(
          data['description']!,
          _descriptionMeta,
        ),
      );
    }
    if (data.containsKey('task_type')) {
      context.handle(
        _taskTypeMeta,
        taskType.isAcceptableOrUnknown(data['task_type']!, _taskTypeMeta),
      );
    }
    if (data.containsKey('status')) {
      context.handle(
        _statusMeta,
        status.isAcceptableOrUnknown(data['status']!, _statusMeta),
      );
    }
    if (data.containsKey('priority')) {
      context.handle(
        _priorityMeta,
        priority.isAcceptableOrUnknown(data['priority']!, _priorityMeta),
      );
    }
    if (data.containsKey('assignee_user_id')) {
      context.handle(
        _assigneeUserIdMeta,
        assigneeUserId.isAcceptableOrUnknown(
          data['assignee_user_id']!,
          _assigneeUserIdMeta,
        ),
      );
    }
    if (data.containsKey('reporter_user_id')) {
      context.handle(
        _reporterUserIdMeta,
        reporterUserId.isAcceptableOrUnknown(
          data['reporter_user_id']!,
          _reporterUserIdMeta,
        ),
      );
    }
    if (data.containsKey('team_id')) {
      context.handle(
        _teamIdMeta,
        teamId.isAcceptableOrUnknown(data['team_id']!, _teamIdMeta),
      );
    }
    if (data.containsKey('board_column_id')) {
      context.handle(
        _boardColumnIdMeta,
        boardColumnId.isAcceptableOrUnknown(
          data['board_column_id']!,
          _boardColumnIdMeta,
        ),
      );
    }
    if (data.containsKey('story_points')) {
      context.handle(
        _storyPointsMeta,
        storyPoints.isAcceptableOrUnknown(
          data['story_points']!,
          _storyPointsMeta,
        ),
      );
    }
    if (data.containsKey('estimated_hours')) {
      context.handle(
        _estimatedHoursMeta,
        estimatedHours.isAcceptableOrUnknown(
          data['estimated_hours']!,
          _estimatedHoursMeta,
        ),
      );
    }
    if (data.containsKey('remaining_hours')) {
      context.handle(
        _remainingHoursMeta,
        remainingHours.isAcceptableOrUnknown(
          data['remaining_hours']!,
          _remainingHoursMeta,
        ),
      );
    }
    if (data.containsKey('logged_hours')) {
      context.handle(
        _loggedHoursMeta,
        loggedHours.isAcceptableOrUnknown(
          data['logged_hours']!,
          _loggedHoursMeta,
        ),
      );
    }
    if (data.containsKey('start_date')) {
      context.handle(
        _startDateMeta,
        startDate.isAcceptableOrUnknown(data['start_date']!, _startDateMeta),
      );
    }
    if (data.containsKey('due_date')) {
      context.handle(
        _dueDateMeta,
        dueDate.isAcceptableOrUnknown(data['due_date']!, _dueDateMeta),
      );
    }
    if (data.containsKey('position')) {
      context.handle(
        _positionMeta,
        position.isAcceptableOrUnknown(data['position']!, _positionMeta),
      );
    }
    if (data.containsKey('is_deleted')) {
      context.handle(
        _isDeletedMeta,
        isDeleted.isAcceptableOrUnknown(data['is_deleted']!, _isDeletedMeta),
      );
    }
    if (data.containsKey('row_version')) {
      context.handle(
        _rowVersionMeta,
        rowVersion.isAcceptableOrUnknown(data['row_version']!, _rowVersionMeta),
      );
    }
    if (data.containsKey('updated_at')) {
      context.handle(
        _updatedAtMeta,
        updatedAt.isAcceptableOrUnknown(data['updated_at']!, _updatedAtMeta),
      );
    }
    if (data.containsKey('sync_state')) {
      context.handle(
        _syncStateMeta,
        syncState.isAcceptableOrUnknown(data['sync_state']!, _syncStateMeta),
      );
    }
    if (data.containsKey('needs_review')) {
      context.handle(
        _needsReviewMeta,
        needsReview.isAcceptableOrUnknown(
          data['needs_review']!,
          _needsReviewMeta,
        ),
      );
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {id};
  @override
  LocalTask map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return LocalTask(
      id: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}id'],
      )!,
      taskKey: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}task_key'],
      )!,
      projectId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}project_id'],
      )!,
      epicId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}epic_id'],
      ),
      sprintId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}sprint_id'],
      ),
      parentTaskId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}parent_task_id'],
      ),
      title: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}title'],
      )!,
      description: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}description'],
      ),
      taskType: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}task_type'],
      )!,
      status: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}status'],
      )!,
      priority: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}priority'],
      )!,
      assigneeUserId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}assignee_user_id'],
      ),
      reporterUserId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}reporter_user_id'],
      ),
      teamId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}team_id'],
      ),
      boardColumnId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}board_column_id'],
      ),
      storyPoints: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}story_points'],
      ),
      estimatedHours: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}estimated_hours'],
      ),
      remainingHours: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}remaining_hours'],
      ),
      loggedHours: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}logged_hours'],
      )!,
      startDate: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}start_date'],
      ),
      dueDate: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}due_date'],
      ),
      position: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}position'],
      )!,
      isDeleted: attachedDatabase.typeMapping.read(
        DriftSqlType.bool,
        data['${effectivePrefix}is_deleted'],
      )!,
      rowVersion: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}row_version'],
      )!,
      updatedAt: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}updated_at'],
      ),
      syncState: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}sync_state'],
      )!,
      needsReview: attachedDatabase.typeMapping.read(
        DriftSqlType.bool,
        data['${effectivePrefix}needs_review'],
      )!,
    );
  }

  @override
  $LocalTasksTable createAlias(String alias) {
    return $LocalTasksTable(attachedDatabase, alias);
  }
}

class LocalTask extends DataClass implements Insertable<LocalTask> {
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

  /// base64 of the server RowVersion; empty until the row has been synced once.
  final String rowVersion;
  final DateTime? updatedAt;

  /// Client sync bookkeeping: 'synced' | 'pendingCreate' | 'pendingUpdate' | 'pendingDelete'.
  final String syncState;

  /// True when a server conflict diverged from local edits and the user must reconcile.
  final bool needsReview;
  const LocalTask({
    required this.id,
    required this.taskKey,
    required this.projectId,
    this.epicId,
    this.sprintId,
    this.parentTaskId,
    required this.title,
    this.description,
    required this.taskType,
    required this.status,
    required this.priority,
    this.assigneeUserId,
    this.reporterUserId,
    this.teamId,
    this.boardColumnId,
    this.storyPoints,
    this.estimatedHours,
    this.remainingHours,
    required this.loggedHours,
    this.startDate,
    this.dueDate,
    required this.position,
    required this.isDeleted,
    required this.rowVersion,
    this.updatedAt,
    required this.syncState,
    required this.needsReview,
  });
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['id'] = Variable<String>(id);
    map['task_key'] = Variable<String>(taskKey);
    map['project_id'] = Variable<String>(projectId);
    if (!nullToAbsent || epicId != null) {
      map['epic_id'] = Variable<String>(epicId);
    }
    if (!nullToAbsent || sprintId != null) {
      map['sprint_id'] = Variable<String>(sprintId);
    }
    if (!nullToAbsent || parentTaskId != null) {
      map['parent_task_id'] = Variable<String>(parentTaskId);
    }
    map['title'] = Variable<String>(title);
    if (!nullToAbsent || description != null) {
      map['description'] = Variable<String>(description);
    }
    map['task_type'] = Variable<int>(taskType);
    map['status'] = Variable<int>(status);
    map['priority'] = Variable<int>(priority);
    if (!nullToAbsent || assigneeUserId != null) {
      map['assignee_user_id'] = Variable<String>(assigneeUserId);
    }
    if (!nullToAbsent || reporterUserId != null) {
      map['reporter_user_id'] = Variable<String>(reporterUserId);
    }
    if (!nullToAbsent || teamId != null) {
      map['team_id'] = Variable<String>(teamId);
    }
    if (!nullToAbsent || boardColumnId != null) {
      map['board_column_id'] = Variable<String>(boardColumnId);
    }
    if (!nullToAbsent || storyPoints != null) {
      map['story_points'] = Variable<int>(storyPoints);
    }
    if (!nullToAbsent || estimatedHours != null) {
      map['estimated_hours'] = Variable<double>(estimatedHours);
    }
    if (!nullToAbsent || remainingHours != null) {
      map['remaining_hours'] = Variable<double>(remainingHours);
    }
    map['logged_hours'] = Variable<double>(loggedHours);
    if (!nullToAbsent || startDate != null) {
      map['start_date'] = Variable<DateTime>(startDate);
    }
    if (!nullToAbsent || dueDate != null) {
      map['due_date'] = Variable<DateTime>(dueDate);
    }
    map['position'] = Variable<double>(position);
    map['is_deleted'] = Variable<bool>(isDeleted);
    map['row_version'] = Variable<String>(rowVersion);
    if (!nullToAbsent || updatedAt != null) {
      map['updated_at'] = Variable<DateTime>(updatedAt);
    }
    map['sync_state'] = Variable<String>(syncState);
    map['needs_review'] = Variable<bool>(needsReview);
    return map;
  }

  LocalTasksCompanion toCompanion(bool nullToAbsent) {
    return LocalTasksCompanion(
      id: Value(id),
      taskKey: Value(taskKey),
      projectId: Value(projectId),
      epicId: epicId == null && nullToAbsent
          ? const Value.absent()
          : Value(epicId),
      sprintId: sprintId == null && nullToAbsent
          ? const Value.absent()
          : Value(sprintId),
      parentTaskId: parentTaskId == null && nullToAbsent
          ? const Value.absent()
          : Value(parentTaskId),
      title: Value(title),
      description: description == null && nullToAbsent
          ? const Value.absent()
          : Value(description),
      taskType: Value(taskType),
      status: Value(status),
      priority: Value(priority),
      assigneeUserId: assigneeUserId == null && nullToAbsent
          ? const Value.absent()
          : Value(assigneeUserId),
      reporterUserId: reporterUserId == null && nullToAbsent
          ? const Value.absent()
          : Value(reporterUserId),
      teamId: teamId == null && nullToAbsent
          ? const Value.absent()
          : Value(teamId),
      boardColumnId: boardColumnId == null && nullToAbsent
          ? const Value.absent()
          : Value(boardColumnId),
      storyPoints: storyPoints == null && nullToAbsent
          ? const Value.absent()
          : Value(storyPoints),
      estimatedHours: estimatedHours == null && nullToAbsent
          ? const Value.absent()
          : Value(estimatedHours),
      remainingHours: remainingHours == null && nullToAbsent
          ? const Value.absent()
          : Value(remainingHours),
      loggedHours: Value(loggedHours),
      startDate: startDate == null && nullToAbsent
          ? const Value.absent()
          : Value(startDate),
      dueDate: dueDate == null && nullToAbsent
          ? const Value.absent()
          : Value(dueDate),
      position: Value(position),
      isDeleted: Value(isDeleted),
      rowVersion: Value(rowVersion),
      updatedAt: updatedAt == null && nullToAbsent
          ? const Value.absent()
          : Value(updatedAt),
      syncState: Value(syncState),
      needsReview: Value(needsReview),
    );
  }

  factory LocalTask.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return LocalTask(
      id: serializer.fromJson<String>(json['id']),
      taskKey: serializer.fromJson<String>(json['taskKey']),
      projectId: serializer.fromJson<String>(json['projectId']),
      epicId: serializer.fromJson<String?>(json['epicId']),
      sprintId: serializer.fromJson<String?>(json['sprintId']),
      parentTaskId: serializer.fromJson<String?>(json['parentTaskId']),
      title: serializer.fromJson<String>(json['title']),
      description: serializer.fromJson<String?>(json['description']),
      taskType: serializer.fromJson<int>(json['taskType']),
      status: serializer.fromJson<int>(json['status']),
      priority: serializer.fromJson<int>(json['priority']),
      assigneeUserId: serializer.fromJson<String?>(json['assigneeUserId']),
      reporterUserId: serializer.fromJson<String?>(json['reporterUserId']),
      teamId: serializer.fromJson<String?>(json['teamId']),
      boardColumnId: serializer.fromJson<String?>(json['boardColumnId']),
      storyPoints: serializer.fromJson<int?>(json['storyPoints']),
      estimatedHours: serializer.fromJson<double?>(json['estimatedHours']),
      remainingHours: serializer.fromJson<double?>(json['remainingHours']),
      loggedHours: serializer.fromJson<double>(json['loggedHours']),
      startDate: serializer.fromJson<DateTime?>(json['startDate']),
      dueDate: serializer.fromJson<DateTime?>(json['dueDate']),
      position: serializer.fromJson<double>(json['position']),
      isDeleted: serializer.fromJson<bool>(json['isDeleted']),
      rowVersion: serializer.fromJson<String>(json['rowVersion']),
      updatedAt: serializer.fromJson<DateTime?>(json['updatedAt']),
      syncState: serializer.fromJson<String>(json['syncState']),
      needsReview: serializer.fromJson<bool>(json['needsReview']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'id': serializer.toJson<String>(id),
      'taskKey': serializer.toJson<String>(taskKey),
      'projectId': serializer.toJson<String>(projectId),
      'epicId': serializer.toJson<String?>(epicId),
      'sprintId': serializer.toJson<String?>(sprintId),
      'parentTaskId': serializer.toJson<String?>(parentTaskId),
      'title': serializer.toJson<String>(title),
      'description': serializer.toJson<String?>(description),
      'taskType': serializer.toJson<int>(taskType),
      'status': serializer.toJson<int>(status),
      'priority': serializer.toJson<int>(priority),
      'assigneeUserId': serializer.toJson<String?>(assigneeUserId),
      'reporterUserId': serializer.toJson<String?>(reporterUserId),
      'teamId': serializer.toJson<String?>(teamId),
      'boardColumnId': serializer.toJson<String?>(boardColumnId),
      'storyPoints': serializer.toJson<int?>(storyPoints),
      'estimatedHours': serializer.toJson<double?>(estimatedHours),
      'remainingHours': serializer.toJson<double?>(remainingHours),
      'loggedHours': serializer.toJson<double>(loggedHours),
      'startDate': serializer.toJson<DateTime?>(startDate),
      'dueDate': serializer.toJson<DateTime?>(dueDate),
      'position': serializer.toJson<double>(position),
      'isDeleted': serializer.toJson<bool>(isDeleted),
      'rowVersion': serializer.toJson<String>(rowVersion),
      'updatedAt': serializer.toJson<DateTime?>(updatedAt),
      'syncState': serializer.toJson<String>(syncState),
      'needsReview': serializer.toJson<bool>(needsReview),
    };
  }

  LocalTask copyWith({
    String? id,
    String? taskKey,
    String? projectId,
    Value<String?> epicId = const Value.absent(),
    Value<String?> sprintId = const Value.absent(),
    Value<String?> parentTaskId = const Value.absent(),
    String? title,
    Value<String?> description = const Value.absent(),
    int? taskType,
    int? status,
    int? priority,
    Value<String?> assigneeUserId = const Value.absent(),
    Value<String?> reporterUserId = const Value.absent(),
    Value<String?> teamId = const Value.absent(),
    Value<String?> boardColumnId = const Value.absent(),
    Value<int?> storyPoints = const Value.absent(),
    Value<double?> estimatedHours = const Value.absent(),
    Value<double?> remainingHours = const Value.absent(),
    double? loggedHours,
    Value<DateTime?> startDate = const Value.absent(),
    Value<DateTime?> dueDate = const Value.absent(),
    double? position,
    bool? isDeleted,
    String? rowVersion,
    Value<DateTime?> updatedAt = const Value.absent(),
    String? syncState,
    bool? needsReview,
  }) => LocalTask(
    id: id ?? this.id,
    taskKey: taskKey ?? this.taskKey,
    projectId: projectId ?? this.projectId,
    epicId: epicId.present ? epicId.value : this.epicId,
    sprintId: sprintId.present ? sprintId.value : this.sprintId,
    parentTaskId: parentTaskId.present ? parentTaskId.value : this.parentTaskId,
    title: title ?? this.title,
    description: description.present ? description.value : this.description,
    taskType: taskType ?? this.taskType,
    status: status ?? this.status,
    priority: priority ?? this.priority,
    assigneeUserId: assigneeUserId.present
        ? assigneeUserId.value
        : this.assigneeUserId,
    reporterUserId: reporterUserId.present
        ? reporterUserId.value
        : this.reporterUserId,
    teamId: teamId.present ? teamId.value : this.teamId,
    boardColumnId: boardColumnId.present
        ? boardColumnId.value
        : this.boardColumnId,
    storyPoints: storyPoints.present ? storyPoints.value : this.storyPoints,
    estimatedHours: estimatedHours.present
        ? estimatedHours.value
        : this.estimatedHours,
    remainingHours: remainingHours.present
        ? remainingHours.value
        : this.remainingHours,
    loggedHours: loggedHours ?? this.loggedHours,
    startDate: startDate.present ? startDate.value : this.startDate,
    dueDate: dueDate.present ? dueDate.value : this.dueDate,
    position: position ?? this.position,
    isDeleted: isDeleted ?? this.isDeleted,
    rowVersion: rowVersion ?? this.rowVersion,
    updatedAt: updatedAt.present ? updatedAt.value : this.updatedAt,
    syncState: syncState ?? this.syncState,
    needsReview: needsReview ?? this.needsReview,
  );
  LocalTask copyWithCompanion(LocalTasksCompanion data) {
    return LocalTask(
      id: data.id.present ? data.id.value : this.id,
      taskKey: data.taskKey.present ? data.taskKey.value : this.taskKey,
      projectId: data.projectId.present ? data.projectId.value : this.projectId,
      epicId: data.epicId.present ? data.epicId.value : this.epicId,
      sprintId: data.sprintId.present ? data.sprintId.value : this.sprintId,
      parentTaskId: data.parentTaskId.present
          ? data.parentTaskId.value
          : this.parentTaskId,
      title: data.title.present ? data.title.value : this.title,
      description: data.description.present
          ? data.description.value
          : this.description,
      taskType: data.taskType.present ? data.taskType.value : this.taskType,
      status: data.status.present ? data.status.value : this.status,
      priority: data.priority.present ? data.priority.value : this.priority,
      assigneeUserId: data.assigneeUserId.present
          ? data.assigneeUserId.value
          : this.assigneeUserId,
      reporterUserId: data.reporterUserId.present
          ? data.reporterUserId.value
          : this.reporterUserId,
      teamId: data.teamId.present ? data.teamId.value : this.teamId,
      boardColumnId: data.boardColumnId.present
          ? data.boardColumnId.value
          : this.boardColumnId,
      storyPoints: data.storyPoints.present
          ? data.storyPoints.value
          : this.storyPoints,
      estimatedHours: data.estimatedHours.present
          ? data.estimatedHours.value
          : this.estimatedHours,
      remainingHours: data.remainingHours.present
          ? data.remainingHours.value
          : this.remainingHours,
      loggedHours: data.loggedHours.present
          ? data.loggedHours.value
          : this.loggedHours,
      startDate: data.startDate.present ? data.startDate.value : this.startDate,
      dueDate: data.dueDate.present ? data.dueDate.value : this.dueDate,
      position: data.position.present ? data.position.value : this.position,
      isDeleted: data.isDeleted.present ? data.isDeleted.value : this.isDeleted,
      rowVersion: data.rowVersion.present
          ? data.rowVersion.value
          : this.rowVersion,
      updatedAt: data.updatedAt.present ? data.updatedAt.value : this.updatedAt,
      syncState: data.syncState.present ? data.syncState.value : this.syncState,
      needsReview: data.needsReview.present
          ? data.needsReview.value
          : this.needsReview,
    );
  }

  @override
  String toString() {
    return (StringBuffer('LocalTask(')
          ..write('id: $id, ')
          ..write('taskKey: $taskKey, ')
          ..write('projectId: $projectId, ')
          ..write('epicId: $epicId, ')
          ..write('sprintId: $sprintId, ')
          ..write('parentTaskId: $parentTaskId, ')
          ..write('title: $title, ')
          ..write('description: $description, ')
          ..write('taskType: $taskType, ')
          ..write('status: $status, ')
          ..write('priority: $priority, ')
          ..write('assigneeUserId: $assigneeUserId, ')
          ..write('reporterUserId: $reporterUserId, ')
          ..write('teamId: $teamId, ')
          ..write('boardColumnId: $boardColumnId, ')
          ..write('storyPoints: $storyPoints, ')
          ..write('estimatedHours: $estimatedHours, ')
          ..write('remainingHours: $remainingHours, ')
          ..write('loggedHours: $loggedHours, ')
          ..write('startDate: $startDate, ')
          ..write('dueDate: $dueDate, ')
          ..write('position: $position, ')
          ..write('isDeleted: $isDeleted, ')
          ..write('rowVersion: $rowVersion, ')
          ..write('updatedAt: $updatedAt, ')
          ..write('syncState: $syncState, ')
          ..write('needsReview: $needsReview')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode => Object.hashAll([
    id,
    taskKey,
    projectId,
    epicId,
    sprintId,
    parentTaskId,
    title,
    description,
    taskType,
    status,
    priority,
    assigneeUserId,
    reporterUserId,
    teamId,
    boardColumnId,
    storyPoints,
    estimatedHours,
    remainingHours,
    loggedHours,
    startDate,
    dueDate,
    position,
    isDeleted,
    rowVersion,
    updatedAt,
    syncState,
    needsReview,
  ]);
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is LocalTask &&
          other.id == this.id &&
          other.taskKey == this.taskKey &&
          other.projectId == this.projectId &&
          other.epicId == this.epicId &&
          other.sprintId == this.sprintId &&
          other.parentTaskId == this.parentTaskId &&
          other.title == this.title &&
          other.description == this.description &&
          other.taskType == this.taskType &&
          other.status == this.status &&
          other.priority == this.priority &&
          other.assigneeUserId == this.assigneeUserId &&
          other.reporterUserId == this.reporterUserId &&
          other.teamId == this.teamId &&
          other.boardColumnId == this.boardColumnId &&
          other.storyPoints == this.storyPoints &&
          other.estimatedHours == this.estimatedHours &&
          other.remainingHours == this.remainingHours &&
          other.loggedHours == this.loggedHours &&
          other.startDate == this.startDate &&
          other.dueDate == this.dueDate &&
          other.position == this.position &&
          other.isDeleted == this.isDeleted &&
          other.rowVersion == this.rowVersion &&
          other.updatedAt == this.updatedAt &&
          other.syncState == this.syncState &&
          other.needsReview == this.needsReview);
}

class LocalTasksCompanion extends UpdateCompanion<LocalTask> {
  final Value<String> id;
  final Value<String> taskKey;
  final Value<String> projectId;
  final Value<String?> epicId;
  final Value<String?> sprintId;
  final Value<String?> parentTaskId;
  final Value<String> title;
  final Value<String?> description;
  final Value<int> taskType;
  final Value<int> status;
  final Value<int> priority;
  final Value<String?> assigneeUserId;
  final Value<String?> reporterUserId;
  final Value<String?> teamId;
  final Value<String?> boardColumnId;
  final Value<int?> storyPoints;
  final Value<double?> estimatedHours;
  final Value<double?> remainingHours;
  final Value<double> loggedHours;
  final Value<DateTime?> startDate;
  final Value<DateTime?> dueDate;
  final Value<double> position;
  final Value<bool> isDeleted;
  final Value<String> rowVersion;
  final Value<DateTime?> updatedAt;
  final Value<String> syncState;
  final Value<bool> needsReview;
  final Value<int> rowid;
  const LocalTasksCompanion({
    this.id = const Value.absent(),
    this.taskKey = const Value.absent(),
    this.projectId = const Value.absent(),
    this.epicId = const Value.absent(),
    this.sprintId = const Value.absent(),
    this.parentTaskId = const Value.absent(),
    this.title = const Value.absent(),
    this.description = const Value.absent(),
    this.taskType = const Value.absent(),
    this.status = const Value.absent(),
    this.priority = const Value.absent(),
    this.assigneeUserId = const Value.absent(),
    this.reporterUserId = const Value.absent(),
    this.teamId = const Value.absent(),
    this.boardColumnId = const Value.absent(),
    this.storyPoints = const Value.absent(),
    this.estimatedHours = const Value.absent(),
    this.remainingHours = const Value.absent(),
    this.loggedHours = const Value.absent(),
    this.startDate = const Value.absent(),
    this.dueDate = const Value.absent(),
    this.position = const Value.absent(),
    this.isDeleted = const Value.absent(),
    this.rowVersion = const Value.absent(),
    this.updatedAt = const Value.absent(),
    this.syncState = const Value.absent(),
    this.needsReview = const Value.absent(),
    this.rowid = const Value.absent(),
  });
  LocalTasksCompanion.insert({
    required String id,
    this.taskKey = const Value.absent(),
    required String projectId,
    this.epicId = const Value.absent(),
    this.sprintId = const Value.absent(),
    this.parentTaskId = const Value.absent(),
    required String title,
    this.description = const Value.absent(),
    this.taskType = const Value.absent(),
    this.status = const Value.absent(),
    this.priority = const Value.absent(),
    this.assigneeUserId = const Value.absent(),
    this.reporterUserId = const Value.absent(),
    this.teamId = const Value.absent(),
    this.boardColumnId = const Value.absent(),
    this.storyPoints = const Value.absent(),
    this.estimatedHours = const Value.absent(),
    this.remainingHours = const Value.absent(),
    this.loggedHours = const Value.absent(),
    this.startDate = const Value.absent(),
    this.dueDate = const Value.absent(),
    this.position = const Value.absent(),
    this.isDeleted = const Value.absent(),
    this.rowVersion = const Value.absent(),
    this.updatedAt = const Value.absent(),
    this.syncState = const Value.absent(),
    this.needsReview = const Value.absent(),
    this.rowid = const Value.absent(),
  }) : id = Value(id),
       projectId = Value(projectId),
       title = Value(title);
  static Insertable<LocalTask> custom({
    Expression<String>? id,
    Expression<String>? taskKey,
    Expression<String>? projectId,
    Expression<String>? epicId,
    Expression<String>? sprintId,
    Expression<String>? parentTaskId,
    Expression<String>? title,
    Expression<String>? description,
    Expression<int>? taskType,
    Expression<int>? status,
    Expression<int>? priority,
    Expression<String>? assigneeUserId,
    Expression<String>? reporterUserId,
    Expression<String>? teamId,
    Expression<String>? boardColumnId,
    Expression<int>? storyPoints,
    Expression<double>? estimatedHours,
    Expression<double>? remainingHours,
    Expression<double>? loggedHours,
    Expression<DateTime>? startDate,
    Expression<DateTime>? dueDate,
    Expression<double>? position,
    Expression<bool>? isDeleted,
    Expression<String>? rowVersion,
    Expression<DateTime>? updatedAt,
    Expression<String>? syncState,
    Expression<bool>? needsReview,
    Expression<int>? rowid,
  }) {
    return RawValuesInsertable({
      if (id != null) 'id': id,
      if (taskKey != null) 'task_key': taskKey,
      if (projectId != null) 'project_id': projectId,
      if (epicId != null) 'epic_id': epicId,
      if (sprintId != null) 'sprint_id': sprintId,
      if (parentTaskId != null) 'parent_task_id': parentTaskId,
      if (title != null) 'title': title,
      if (description != null) 'description': description,
      if (taskType != null) 'task_type': taskType,
      if (status != null) 'status': status,
      if (priority != null) 'priority': priority,
      if (assigneeUserId != null) 'assignee_user_id': assigneeUserId,
      if (reporterUserId != null) 'reporter_user_id': reporterUserId,
      if (teamId != null) 'team_id': teamId,
      if (boardColumnId != null) 'board_column_id': boardColumnId,
      if (storyPoints != null) 'story_points': storyPoints,
      if (estimatedHours != null) 'estimated_hours': estimatedHours,
      if (remainingHours != null) 'remaining_hours': remainingHours,
      if (loggedHours != null) 'logged_hours': loggedHours,
      if (startDate != null) 'start_date': startDate,
      if (dueDate != null) 'due_date': dueDate,
      if (position != null) 'position': position,
      if (isDeleted != null) 'is_deleted': isDeleted,
      if (rowVersion != null) 'row_version': rowVersion,
      if (updatedAt != null) 'updated_at': updatedAt,
      if (syncState != null) 'sync_state': syncState,
      if (needsReview != null) 'needs_review': needsReview,
      if (rowid != null) 'rowid': rowid,
    });
  }

  LocalTasksCompanion copyWith({
    Value<String>? id,
    Value<String>? taskKey,
    Value<String>? projectId,
    Value<String?>? epicId,
    Value<String?>? sprintId,
    Value<String?>? parentTaskId,
    Value<String>? title,
    Value<String?>? description,
    Value<int>? taskType,
    Value<int>? status,
    Value<int>? priority,
    Value<String?>? assigneeUserId,
    Value<String?>? reporterUserId,
    Value<String?>? teamId,
    Value<String?>? boardColumnId,
    Value<int?>? storyPoints,
    Value<double?>? estimatedHours,
    Value<double?>? remainingHours,
    Value<double>? loggedHours,
    Value<DateTime?>? startDate,
    Value<DateTime?>? dueDate,
    Value<double>? position,
    Value<bool>? isDeleted,
    Value<String>? rowVersion,
    Value<DateTime?>? updatedAt,
    Value<String>? syncState,
    Value<bool>? needsReview,
    Value<int>? rowid,
  }) {
    return LocalTasksCompanion(
      id: id ?? this.id,
      taskKey: taskKey ?? this.taskKey,
      projectId: projectId ?? this.projectId,
      epicId: epicId ?? this.epicId,
      sprintId: sprintId ?? this.sprintId,
      parentTaskId: parentTaskId ?? this.parentTaskId,
      title: title ?? this.title,
      description: description ?? this.description,
      taskType: taskType ?? this.taskType,
      status: status ?? this.status,
      priority: priority ?? this.priority,
      assigneeUserId: assigneeUserId ?? this.assigneeUserId,
      reporterUserId: reporterUserId ?? this.reporterUserId,
      teamId: teamId ?? this.teamId,
      boardColumnId: boardColumnId ?? this.boardColumnId,
      storyPoints: storyPoints ?? this.storyPoints,
      estimatedHours: estimatedHours ?? this.estimatedHours,
      remainingHours: remainingHours ?? this.remainingHours,
      loggedHours: loggedHours ?? this.loggedHours,
      startDate: startDate ?? this.startDate,
      dueDate: dueDate ?? this.dueDate,
      position: position ?? this.position,
      isDeleted: isDeleted ?? this.isDeleted,
      rowVersion: rowVersion ?? this.rowVersion,
      updatedAt: updatedAt ?? this.updatedAt,
      syncState: syncState ?? this.syncState,
      needsReview: needsReview ?? this.needsReview,
      rowid: rowid ?? this.rowid,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (id.present) {
      map['id'] = Variable<String>(id.value);
    }
    if (taskKey.present) {
      map['task_key'] = Variable<String>(taskKey.value);
    }
    if (projectId.present) {
      map['project_id'] = Variable<String>(projectId.value);
    }
    if (epicId.present) {
      map['epic_id'] = Variable<String>(epicId.value);
    }
    if (sprintId.present) {
      map['sprint_id'] = Variable<String>(sprintId.value);
    }
    if (parentTaskId.present) {
      map['parent_task_id'] = Variable<String>(parentTaskId.value);
    }
    if (title.present) {
      map['title'] = Variable<String>(title.value);
    }
    if (description.present) {
      map['description'] = Variable<String>(description.value);
    }
    if (taskType.present) {
      map['task_type'] = Variable<int>(taskType.value);
    }
    if (status.present) {
      map['status'] = Variable<int>(status.value);
    }
    if (priority.present) {
      map['priority'] = Variable<int>(priority.value);
    }
    if (assigneeUserId.present) {
      map['assignee_user_id'] = Variable<String>(assigneeUserId.value);
    }
    if (reporterUserId.present) {
      map['reporter_user_id'] = Variable<String>(reporterUserId.value);
    }
    if (teamId.present) {
      map['team_id'] = Variable<String>(teamId.value);
    }
    if (boardColumnId.present) {
      map['board_column_id'] = Variable<String>(boardColumnId.value);
    }
    if (storyPoints.present) {
      map['story_points'] = Variable<int>(storyPoints.value);
    }
    if (estimatedHours.present) {
      map['estimated_hours'] = Variable<double>(estimatedHours.value);
    }
    if (remainingHours.present) {
      map['remaining_hours'] = Variable<double>(remainingHours.value);
    }
    if (loggedHours.present) {
      map['logged_hours'] = Variable<double>(loggedHours.value);
    }
    if (startDate.present) {
      map['start_date'] = Variable<DateTime>(startDate.value);
    }
    if (dueDate.present) {
      map['due_date'] = Variable<DateTime>(dueDate.value);
    }
    if (position.present) {
      map['position'] = Variable<double>(position.value);
    }
    if (isDeleted.present) {
      map['is_deleted'] = Variable<bool>(isDeleted.value);
    }
    if (rowVersion.present) {
      map['row_version'] = Variable<String>(rowVersion.value);
    }
    if (updatedAt.present) {
      map['updated_at'] = Variable<DateTime>(updatedAt.value);
    }
    if (syncState.present) {
      map['sync_state'] = Variable<String>(syncState.value);
    }
    if (needsReview.present) {
      map['needs_review'] = Variable<bool>(needsReview.value);
    }
    if (rowid.present) {
      map['rowid'] = Variable<int>(rowid.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('LocalTasksCompanion(')
          ..write('id: $id, ')
          ..write('taskKey: $taskKey, ')
          ..write('projectId: $projectId, ')
          ..write('epicId: $epicId, ')
          ..write('sprintId: $sprintId, ')
          ..write('parentTaskId: $parentTaskId, ')
          ..write('title: $title, ')
          ..write('description: $description, ')
          ..write('taskType: $taskType, ')
          ..write('status: $status, ')
          ..write('priority: $priority, ')
          ..write('assigneeUserId: $assigneeUserId, ')
          ..write('reporterUserId: $reporterUserId, ')
          ..write('teamId: $teamId, ')
          ..write('boardColumnId: $boardColumnId, ')
          ..write('storyPoints: $storyPoints, ')
          ..write('estimatedHours: $estimatedHours, ')
          ..write('remainingHours: $remainingHours, ')
          ..write('loggedHours: $loggedHours, ')
          ..write('startDate: $startDate, ')
          ..write('dueDate: $dueDate, ')
          ..write('position: $position, ')
          ..write('isDeleted: $isDeleted, ')
          ..write('rowVersion: $rowVersion, ')
          ..write('updatedAt: $updatedAt, ')
          ..write('syncState: $syncState, ')
          ..write('needsReview: $needsReview, ')
          ..write('rowid: $rowid')
          ..write(')'))
        .toString();
  }
}

class $SyncQueueItemsTable extends SyncQueueItems
    with TableInfo<$SyncQueueItemsTable, SyncQueueItem> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $SyncQueueItemsTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _operationIdMeta = const VerificationMeta(
    'operationId',
  );
  @override
  late final GeneratedColumn<String> operationId = GeneratedColumn<String>(
    'operation_id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _entityTypeMeta = const VerificationMeta(
    'entityType',
  );
  @override
  late final GeneratedColumn<String> entityType = GeneratedColumn<String>(
    'entity_type',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant('Task'),
  );
  static const VerificationMeta _entityIdMeta = const VerificationMeta(
    'entityId',
  );
  @override
  late final GeneratedColumn<String> entityId = GeneratedColumn<String>(
    'entity_id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _changeTypeMeta = const VerificationMeta(
    'changeType',
  );
  @override
  late final GeneratedColumn<String> changeType = GeneratedColumn<String>(
    'change_type',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _payloadJsonMeta = const VerificationMeta(
    'payloadJson',
  );
  @override
  late final GeneratedColumn<String> payloadJson = GeneratedColumn<String>(
    'payload_json',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _baseRowVersionMeta = const VerificationMeta(
    'baseRowVersion',
  );
  @override
  late final GeneratedColumn<String> baseRowVersion = GeneratedColumn<String>(
    'base_row_version',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _statusMeta = const VerificationMeta('status');
  @override
  late final GeneratedColumn<String> status = GeneratedColumn<String>(
    'status',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant('pending'),
  );
  static const VerificationMeta _attemptCountMeta = const VerificationMeta(
    'attemptCount',
  );
  @override
  late final GeneratedColumn<int> attemptCount = GeneratedColumn<int>(
    'attempt_count',
    aliasedName,
    false,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
    defaultValue: const Constant(0),
  );
  static const VerificationMeta _nextAttemptAtMeta = const VerificationMeta(
    'nextAttemptAt',
  );
  @override
  late final GeneratedColumn<DateTime> nextAttemptAt =
      GeneratedColumn<DateTime>(
        'next_attempt_at',
        aliasedName,
        true,
        type: DriftSqlType.dateTime,
        requiredDuringInsert: false,
      );
  static const VerificationMeta _lastErrorMeta = const VerificationMeta(
    'lastError',
  );
  @override
  late final GeneratedColumn<String> lastError = GeneratedColumn<String>(
    'last_error',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _createdAtMeta = const VerificationMeta(
    'createdAt',
  );
  @override
  late final GeneratedColumn<DateTime> createdAt = GeneratedColumn<DateTime>(
    'created_at',
    aliasedName,
    false,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: true,
  );
  @override
  List<GeneratedColumn> get $columns => [
    operationId,
    entityType,
    entityId,
    changeType,
    payloadJson,
    baseRowVersion,
    status,
    attemptCount,
    nextAttemptAt,
    lastError,
    createdAt,
  ];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'sync_queue_items';
  @override
  VerificationContext validateIntegrity(
    Insertable<SyncQueueItem> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('operation_id')) {
      context.handle(
        _operationIdMeta,
        operationId.isAcceptableOrUnknown(
          data['operation_id']!,
          _operationIdMeta,
        ),
      );
    } else if (isInserting) {
      context.missing(_operationIdMeta);
    }
    if (data.containsKey('entity_type')) {
      context.handle(
        _entityTypeMeta,
        entityType.isAcceptableOrUnknown(data['entity_type']!, _entityTypeMeta),
      );
    }
    if (data.containsKey('entity_id')) {
      context.handle(
        _entityIdMeta,
        entityId.isAcceptableOrUnknown(data['entity_id']!, _entityIdMeta),
      );
    } else if (isInserting) {
      context.missing(_entityIdMeta);
    }
    if (data.containsKey('change_type')) {
      context.handle(
        _changeTypeMeta,
        changeType.isAcceptableOrUnknown(data['change_type']!, _changeTypeMeta),
      );
    } else if (isInserting) {
      context.missing(_changeTypeMeta);
    }
    if (data.containsKey('payload_json')) {
      context.handle(
        _payloadJsonMeta,
        payloadJson.isAcceptableOrUnknown(
          data['payload_json']!,
          _payloadJsonMeta,
        ),
      );
    }
    if (data.containsKey('base_row_version')) {
      context.handle(
        _baseRowVersionMeta,
        baseRowVersion.isAcceptableOrUnknown(
          data['base_row_version']!,
          _baseRowVersionMeta,
        ),
      );
    }
    if (data.containsKey('status')) {
      context.handle(
        _statusMeta,
        status.isAcceptableOrUnknown(data['status']!, _statusMeta),
      );
    }
    if (data.containsKey('attempt_count')) {
      context.handle(
        _attemptCountMeta,
        attemptCount.isAcceptableOrUnknown(
          data['attempt_count']!,
          _attemptCountMeta,
        ),
      );
    }
    if (data.containsKey('next_attempt_at')) {
      context.handle(
        _nextAttemptAtMeta,
        nextAttemptAt.isAcceptableOrUnknown(
          data['next_attempt_at']!,
          _nextAttemptAtMeta,
        ),
      );
    }
    if (data.containsKey('last_error')) {
      context.handle(
        _lastErrorMeta,
        lastError.isAcceptableOrUnknown(data['last_error']!, _lastErrorMeta),
      );
    }
    if (data.containsKey('created_at')) {
      context.handle(
        _createdAtMeta,
        createdAt.isAcceptableOrUnknown(data['created_at']!, _createdAtMeta),
      );
    } else if (isInserting) {
      context.missing(_createdAtMeta);
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {operationId};
  @override
  SyncQueueItem map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return SyncQueueItem(
      operationId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}operation_id'],
      )!,
      entityType: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}entity_type'],
      )!,
      entityId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}entity_id'],
      )!,
      changeType: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}change_type'],
      )!,
      payloadJson: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}payload_json'],
      ),
      baseRowVersion: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}base_row_version'],
      ),
      status: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}status'],
      )!,
      attemptCount: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}attempt_count'],
      )!,
      nextAttemptAt: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}next_attempt_at'],
      ),
      lastError: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}last_error'],
      ),
      createdAt: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}created_at'],
      )!,
    );
  }

  @override
  $SyncQueueItemsTable createAlias(String alias) {
    return $SyncQueueItemsTable(attachedDatabase, alias);
  }
}

class SyncQueueItem extends DataClass implements Insertable<SyncQueueItem> {
  final String operationId;
  final String entityType;
  final String entityId;
  final String changeType;
  final String? payloadJson;
  final String? baseRowVersion;

  /// 'pending' | 'failed'. A 'failed' item exceeded max retries and awaits the user.
  final String status;
  final int attemptCount;
  final DateTime? nextAttemptAt;
  final String? lastError;
  final DateTime createdAt;
  const SyncQueueItem({
    required this.operationId,
    required this.entityType,
    required this.entityId,
    required this.changeType,
    this.payloadJson,
    this.baseRowVersion,
    required this.status,
    required this.attemptCount,
    this.nextAttemptAt,
    this.lastError,
    required this.createdAt,
  });
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['operation_id'] = Variable<String>(operationId);
    map['entity_type'] = Variable<String>(entityType);
    map['entity_id'] = Variable<String>(entityId);
    map['change_type'] = Variable<String>(changeType);
    if (!nullToAbsent || payloadJson != null) {
      map['payload_json'] = Variable<String>(payloadJson);
    }
    if (!nullToAbsent || baseRowVersion != null) {
      map['base_row_version'] = Variable<String>(baseRowVersion);
    }
    map['status'] = Variable<String>(status);
    map['attempt_count'] = Variable<int>(attemptCount);
    if (!nullToAbsent || nextAttemptAt != null) {
      map['next_attempt_at'] = Variable<DateTime>(nextAttemptAt);
    }
    if (!nullToAbsent || lastError != null) {
      map['last_error'] = Variable<String>(lastError);
    }
    map['created_at'] = Variable<DateTime>(createdAt);
    return map;
  }

  SyncQueueItemsCompanion toCompanion(bool nullToAbsent) {
    return SyncQueueItemsCompanion(
      operationId: Value(operationId),
      entityType: Value(entityType),
      entityId: Value(entityId),
      changeType: Value(changeType),
      payloadJson: payloadJson == null && nullToAbsent
          ? const Value.absent()
          : Value(payloadJson),
      baseRowVersion: baseRowVersion == null && nullToAbsent
          ? const Value.absent()
          : Value(baseRowVersion),
      status: Value(status),
      attemptCount: Value(attemptCount),
      nextAttemptAt: nextAttemptAt == null && nullToAbsent
          ? const Value.absent()
          : Value(nextAttemptAt),
      lastError: lastError == null && nullToAbsent
          ? const Value.absent()
          : Value(lastError),
      createdAt: Value(createdAt),
    );
  }

  factory SyncQueueItem.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return SyncQueueItem(
      operationId: serializer.fromJson<String>(json['operationId']),
      entityType: serializer.fromJson<String>(json['entityType']),
      entityId: serializer.fromJson<String>(json['entityId']),
      changeType: serializer.fromJson<String>(json['changeType']),
      payloadJson: serializer.fromJson<String?>(json['payloadJson']),
      baseRowVersion: serializer.fromJson<String?>(json['baseRowVersion']),
      status: serializer.fromJson<String>(json['status']),
      attemptCount: serializer.fromJson<int>(json['attemptCount']),
      nextAttemptAt: serializer.fromJson<DateTime?>(json['nextAttemptAt']),
      lastError: serializer.fromJson<String?>(json['lastError']),
      createdAt: serializer.fromJson<DateTime>(json['createdAt']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'operationId': serializer.toJson<String>(operationId),
      'entityType': serializer.toJson<String>(entityType),
      'entityId': serializer.toJson<String>(entityId),
      'changeType': serializer.toJson<String>(changeType),
      'payloadJson': serializer.toJson<String?>(payloadJson),
      'baseRowVersion': serializer.toJson<String?>(baseRowVersion),
      'status': serializer.toJson<String>(status),
      'attemptCount': serializer.toJson<int>(attemptCount),
      'nextAttemptAt': serializer.toJson<DateTime?>(nextAttemptAt),
      'lastError': serializer.toJson<String?>(lastError),
      'createdAt': serializer.toJson<DateTime>(createdAt),
    };
  }

  SyncQueueItem copyWith({
    String? operationId,
    String? entityType,
    String? entityId,
    String? changeType,
    Value<String?> payloadJson = const Value.absent(),
    Value<String?> baseRowVersion = const Value.absent(),
    String? status,
    int? attemptCount,
    Value<DateTime?> nextAttemptAt = const Value.absent(),
    Value<String?> lastError = const Value.absent(),
    DateTime? createdAt,
  }) => SyncQueueItem(
    operationId: operationId ?? this.operationId,
    entityType: entityType ?? this.entityType,
    entityId: entityId ?? this.entityId,
    changeType: changeType ?? this.changeType,
    payloadJson: payloadJson.present ? payloadJson.value : this.payloadJson,
    baseRowVersion: baseRowVersion.present
        ? baseRowVersion.value
        : this.baseRowVersion,
    status: status ?? this.status,
    attemptCount: attemptCount ?? this.attemptCount,
    nextAttemptAt: nextAttemptAt.present
        ? nextAttemptAt.value
        : this.nextAttemptAt,
    lastError: lastError.present ? lastError.value : this.lastError,
    createdAt: createdAt ?? this.createdAt,
  );
  SyncQueueItem copyWithCompanion(SyncQueueItemsCompanion data) {
    return SyncQueueItem(
      operationId: data.operationId.present
          ? data.operationId.value
          : this.operationId,
      entityType: data.entityType.present
          ? data.entityType.value
          : this.entityType,
      entityId: data.entityId.present ? data.entityId.value : this.entityId,
      changeType: data.changeType.present
          ? data.changeType.value
          : this.changeType,
      payloadJson: data.payloadJson.present
          ? data.payloadJson.value
          : this.payloadJson,
      baseRowVersion: data.baseRowVersion.present
          ? data.baseRowVersion.value
          : this.baseRowVersion,
      status: data.status.present ? data.status.value : this.status,
      attemptCount: data.attemptCount.present
          ? data.attemptCount.value
          : this.attemptCount,
      nextAttemptAt: data.nextAttemptAt.present
          ? data.nextAttemptAt.value
          : this.nextAttemptAt,
      lastError: data.lastError.present ? data.lastError.value : this.lastError,
      createdAt: data.createdAt.present ? data.createdAt.value : this.createdAt,
    );
  }

  @override
  String toString() {
    return (StringBuffer('SyncQueueItem(')
          ..write('operationId: $operationId, ')
          ..write('entityType: $entityType, ')
          ..write('entityId: $entityId, ')
          ..write('changeType: $changeType, ')
          ..write('payloadJson: $payloadJson, ')
          ..write('baseRowVersion: $baseRowVersion, ')
          ..write('status: $status, ')
          ..write('attemptCount: $attemptCount, ')
          ..write('nextAttemptAt: $nextAttemptAt, ')
          ..write('lastError: $lastError, ')
          ..write('createdAt: $createdAt')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode => Object.hash(
    operationId,
    entityType,
    entityId,
    changeType,
    payloadJson,
    baseRowVersion,
    status,
    attemptCount,
    nextAttemptAt,
    lastError,
    createdAt,
  );
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is SyncQueueItem &&
          other.operationId == this.operationId &&
          other.entityType == this.entityType &&
          other.entityId == this.entityId &&
          other.changeType == this.changeType &&
          other.payloadJson == this.payloadJson &&
          other.baseRowVersion == this.baseRowVersion &&
          other.status == this.status &&
          other.attemptCount == this.attemptCount &&
          other.nextAttemptAt == this.nextAttemptAt &&
          other.lastError == this.lastError &&
          other.createdAt == this.createdAt);
}

class SyncQueueItemsCompanion extends UpdateCompanion<SyncQueueItem> {
  final Value<String> operationId;
  final Value<String> entityType;
  final Value<String> entityId;
  final Value<String> changeType;
  final Value<String?> payloadJson;
  final Value<String?> baseRowVersion;
  final Value<String> status;
  final Value<int> attemptCount;
  final Value<DateTime?> nextAttemptAt;
  final Value<String?> lastError;
  final Value<DateTime> createdAt;
  final Value<int> rowid;
  const SyncQueueItemsCompanion({
    this.operationId = const Value.absent(),
    this.entityType = const Value.absent(),
    this.entityId = const Value.absent(),
    this.changeType = const Value.absent(),
    this.payloadJson = const Value.absent(),
    this.baseRowVersion = const Value.absent(),
    this.status = const Value.absent(),
    this.attemptCount = const Value.absent(),
    this.nextAttemptAt = const Value.absent(),
    this.lastError = const Value.absent(),
    this.createdAt = const Value.absent(),
    this.rowid = const Value.absent(),
  });
  SyncQueueItemsCompanion.insert({
    required String operationId,
    this.entityType = const Value.absent(),
    required String entityId,
    required String changeType,
    this.payloadJson = const Value.absent(),
    this.baseRowVersion = const Value.absent(),
    this.status = const Value.absent(),
    this.attemptCount = const Value.absent(),
    this.nextAttemptAt = const Value.absent(),
    this.lastError = const Value.absent(),
    required DateTime createdAt,
    this.rowid = const Value.absent(),
  }) : operationId = Value(operationId),
       entityId = Value(entityId),
       changeType = Value(changeType),
       createdAt = Value(createdAt);
  static Insertable<SyncQueueItem> custom({
    Expression<String>? operationId,
    Expression<String>? entityType,
    Expression<String>? entityId,
    Expression<String>? changeType,
    Expression<String>? payloadJson,
    Expression<String>? baseRowVersion,
    Expression<String>? status,
    Expression<int>? attemptCount,
    Expression<DateTime>? nextAttemptAt,
    Expression<String>? lastError,
    Expression<DateTime>? createdAt,
    Expression<int>? rowid,
  }) {
    return RawValuesInsertable({
      if (operationId != null) 'operation_id': operationId,
      if (entityType != null) 'entity_type': entityType,
      if (entityId != null) 'entity_id': entityId,
      if (changeType != null) 'change_type': changeType,
      if (payloadJson != null) 'payload_json': payloadJson,
      if (baseRowVersion != null) 'base_row_version': baseRowVersion,
      if (status != null) 'status': status,
      if (attemptCount != null) 'attempt_count': attemptCount,
      if (nextAttemptAt != null) 'next_attempt_at': nextAttemptAt,
      if (lastError != null) 'last_error': lastError,
      if (createdAt != null) 'created_at': createdAt,
      if (rowid != null) 'rowid': rowid,
    });
  }

  SyncQueueItemsCompanion copyWith({
    Value<String>? operationId,
    Value<String>? entityType,
    Value<String>? entityId,
    Value<String>? changeType,
    Value<String?>? payloadJson,
    Value<String?>? baseRowVersion,
    Value<String>? status,
    Value<int>? attemptCount,
    Value<DateTime?>? nextAttemptAt,
    Value<String?>? lastError,
    Value<DateTime>? createdAt,
    Value<int>? rowid,
  }) {
    return SyncQueueItemsCompanion(
      operationId: operationId ?? this.operationId,
      entityType: entityType ?? this.entityType,
      entityId: entityId ?? this.entityId,
      changeType: changeType ?? this.changeType,
      payloadJson: payloadJson ?? this.payloadJson,
      baseRowVersion: baseRowVersion ?? this.baseRowVersion,
      status: status ?? this.status,
      attemptCount: attemptCount ?? this.attemptCount,
      nextAttemptAt: nextAttemptAt ?? this.nextAttemptAt,
      lastError: lastError ?? this.lastError,
      createdAt: createdAt ?? this.createdAt,
      rowid: rowid ?? this.rowid,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (operationId.present) {
      map['operation_id'] = Variable<String>(operationId.value);
    }
    if (entityType.present) {
      map['entity_type'] = Variable<String>(entityType.value);
    }
    if (entityId.present) {
      map['entity_id'] = Variable<String>(entityId.value);
    }
    if (changeType.present) {
      map['change_type'] = Variable<String>(changeType.value);
    }
    if (payloadJson.present) {
      map['payload_json'] = Variable<String>(payloadJson.value);
    }
    if (baseRowVersion.present) {
      map['base_row_version'] = Variable<String>(baseRowVersion.value);
    }
    if (status.present) {
      map['status'] = Variable<String>(status.value);
    }
    if (attemptCount.present) {
      map['attempt_count'] = Variable<int>(attemptCount.value);
    }
    if (nextAttemptAt.present) {
      map['next_attempt_at'] = Variable<DateTime>(nextAttemptAt.value);
    }
    if (lastError.present) {
      map['last_error'] = Variable<String>(lastError.value);
    }
    if (createdAt.present) {
      map['created_at'] = Variable<DateTime>(createdAt.value);
    }
    if (rowid.present) {
      map['rowid'] = Variable<int>(rowid.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('SyncQueueItemsCompanion(')
          ..write('operationId: $operationId, ')
          ..write('entityType: $entityType, ')
          ..write('entityId: $entityId, ')
          ..write('changeType: $changeType, ')
          ..write('payloadJson: $payloadJson, ')
          ..write('baseRowVersion: $baseRowVersion, ')
          ..write('status: $status, ')
          ..write('attemptCount: $attemptCount, ')
          ..write('nextAttemptAt: $nextAttemptAt, ')
          ..write('lastError: $lastError, ')
          ..write('createdAt: $createdAt, ')
          ..write('rowid: $rowid')
          ..write(')'))
        .toString();
  }
}

class $ConflictItemsTable extends ConflictItems
    with TableInfo<$ConflictItemsTable, ConflictItem> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $ConflictItemsTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _idMeta = const VerificationMeta('id');
  @override
  late final GeneratedColumn<int> id = GeneratedColumn<int>(
    'id',
    aliasedName,
    false,
    hasAutoIncrement: true,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
    defaultConstraints: GeneratedColumn.constraintIsAlways(
      'PRIMARY KEY AUTOINCREMENT',
    ),
  );
  static const VerificationMeta _entityTypeMeta = const VerificationMeta(
    'entityType',
  );
  @override
  late final GeneratedColumn<String> entityType = GeneratedColumn<String>(
    'entity_type',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant('Task'),
  );
  static const VerificationMeta _entityIdMeta = const VerificationMeta(
    'entityId',
  );
  @override
  late final GeneratedColumn<String> entityId = GeneratedColumn<String>(
    'entity_id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _localPayloadJsonMeta = const VerificationMeta(
    'localPayloadJson',
  );
  @override
  late final GeneratedColumn<String> localPayloadJson = GeneratedColumn<String>(
    'local_payload_json',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _serverPayloadJsonMeta = const VerificationMeta(
    'serverPayloadJson',
  );
  @override
  late final GeneratedColumn<String> serverPayloadJson =
      GeneratedColumn<String>(
        'server_payload_json',
        aliasedName,
        false,
        type: DriftSqlType.string,
        requiredDuringInsert: true,
      );
  static const VerificationMeta _createdAtMeta = const VerificationMeta(
    'createdAt',
  );
  @override
  late final GeneratedColumn<DateTime> createdAt = GeneratedColumn<DateTime>(
    'created_at',
    aliasedName,
    false,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: true,
  );
  @override
  List<GeneratedColumn> get $columns => [
    id,
    entityType,
    entityId,
    localPayloadJson,
    serverPayloadJson,
    createdAt,
  ];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'conflict_items';
  @override
  VerificationContext validateIntegrity(
    Insertable<ConflictItem> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('id')) {
      context.handle(_idMeta, id.isAcceptableOrUnknown(data['id']!, _idMeta));
    }
    if (data.containsKey('entity_type')) {
      context.handle(
        _entityTypeMeta,
        entityType.isAcceptableOrUnknown(data['entity_type']!, _entityTypeMeta),
      );
    }
    if (data.containsKey('entity_id')) {
      context.handle(
        _entityIdMeta,
        entityId.isAcceptableOrUnknown(data['entity_id']!, _entityIdMeta),
      );
    } else if (isInserting) {
      context.missing(_entityIdMeta);
    }
    if (data.containsKey('local_payload_json')) {
      context.handle(
        _localPayloadJsonMeta,
        localPayloadJson.isAcceptableOrUnknown(
          data['local_payload_json']!,
          _localPayloadJsonMeta,
        ),
      );
    } else if (isInserting) {
      context.missing(_localPayloadJsonMeta);
    }
    if (data.containsKey('server_payload_json')) {
      context.handle(
        _serverPayloadJsonMeta,
        serverPayloadJson.isAcceptableOrUnknown(
          data['server_payload_json']!,
          _serverPayloadJsonMeta,
        ),
      );
    } else if (isInserting) {
      context.missing(_serverPayloadJsonMeta);
    }
    if (data.containsKey('created_at')) {
      context.handle(
        _createdAtMeta,
        createdAt.isAcceptableOrUnknown(data['created_at']!, _createdAtMeta),
      );
    } else if (isInserting) {
      context.missing(_createdAtMeta);
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {id};
  @override
  ConflictItem map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return ConflictItem(
      id: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}id'],
      )!,
      entityType: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}entity_type'],
      )!,
      entityId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}entity_id'],
      )!,
      localPayloadJson: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}local_payload_json'],
      )!,
      serverPayloadJson: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}server_payload_json'],
      )!,
      createdAt: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}created_at'],
      )!,
    );
  }

  @override
  $ConflictItemsTable createAlias(String alias) {
    return $ConflictItemsTable(attachedDatabase, alias);
  }
}

class ConflictItem extends DataClass implements Insertable<ConflictItem> {
  final int id;
  final String entityType;
  final String entityId;
  final String localPayloadJson;
  final String serverPayloadJson;
  final DateTime createdAt;
  const ConflictItem({
    required this.id,
    required this.entityType,
    required this.entityId,
    required this.localPayloadJson,
    required this.serverPayloadJson,
    required this.createdAt,
  });
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['id'] = Variable<int>(id);
    map['entity_type'] = Variable<String>(entityType);
    map['entity_id'] = Variable<String>(entityId);
    map['local_payload_json'] = Variable<String>(localPayloadJson);
    map['server_payload_json'] = Variable<String>(serverPayloadJson);
    map['created_at'] = Variable<DateTime>(createdAt);
    return map;
  }

  ConflictItemsCompanion toCompanion(bool nullToAbsent) {
    return ConflictItemsCompanion(
      id: Value(id),
      entityType: Value(entityType),
      entityId: Value(entityId),
      localPayloadJson: Value(localPayloadJson),
      serverPayloadJson: Value(serverPayloadJson),
      createdAt: Value(createdAt),
    );
  }

  factory ConflictItem.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return ConflictItem(
      id: serializer.fromJson<int>(json['id']),
      entityType: serializer.fromJson<String>(json['entityType']),
      entityId: serializer.fromJson<String>(json['entityId']),
      localPayloadJson: serializer.fromJson<String>(json['localPayloadJson']),
      serverPayloadJson: serializer.fromJson<String>(json['serverPayloadJson']),
      createdAt: serializer.fromJson<DateTime>(json['createdAt']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'id': serializer.toJson<int>(id),
      'entityType': serializer.toJson<String>(entityType),
      'entityId': serializer.toJson<String>(entityId),
      'localPayloadJson': serializer.toJson<String>(localPayloadJson),
      'serverPayloadJson': serializer.toJson<String>(serverPayloadJson),
      'createdAt': serializer.toJson<DateTime>(createdAt),
    };
  }

  ConflictItem copyWith({
    int? id,
    String? entityType,
    String? entityId,
    String? localPayloadJson,
    String? serverPayloadJson,
    DateTime? createdAt,
  }) => ConflictItem(
    id: id ?? this.id,
    entityType: entityType ?? this.entityType,
    entityId: entityId ?? this.entityId,
    localPayloadJson: localPayloadJson ?? this.localPayloadJson,
    serverPayloadJson: serverPayloadJson ?? this.serverPayloadJson,
    createdAt: createdAt ?? this.createdAt,
  );
  ConflictItem copyWithCompanion(ConflictItemsCompanion data) {
    return ConflictItem(
      id: data.id.present ? data.id.value : this.id,
      entityType: data.entityType.present
          ? data.entityType.value
          : this.entityType,
      entityId: data.entityId.present ? data.entityId.value : this.entityId,
      localPayloadJson: data.localPayloadJson.present
          ? data.localPayloadJson.value
          : this.localPayloadJson,
      serverPayloadJson: data.serverPayloadJson.present
          ? data.serverPayloadJson.value
          : this.serverPayloadJson,
      createdAt: data.createdAt.present ? data.createdAt.value : this.createdAt,
    );
  }

  @override
  String toString() {
    return (StringBuffer('ConflictItem(')
          ..write('id: $id, ')
          ..write('entityType: $entityType, ')
          ..write('entityId: $entityId, ')
          ..write('localPayloadJson: $localPayloadJson, ')
          ..write('serverPayloadJson: $serverPayloadJson, ')
          ..write('createdAt: $createdAt')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode => Object.hash(
    id,
    entityType,
    entityId,
    localPayloadJson,
    serverPayloadJson,
    createdAt,
  );
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is ConflictItem &&
          other.id == this.id &&
          other.entityType == this.entityType &&
          other.entityId == this.entityId &&
          other.localPayloadJson == this.localPayloadJson &&
          other.serverPayloadJson == this.serverPayloadJson &&
          other.createdAt == this.createdAt);
}

class ConflictItemsCompanion extends UpdateCompanion<ConflictItem> {
  final Value<int> id;
  final Value<String> entityType;
  final Value<String> entityId;
  final Value<String> localPayloadJson;
  final Value<String> serverPayloadJson;
  final Value<DateTime> createdAt;
  const ConflictItemsCompanion({
    this.id = const Value.absent(),
    this.entityType = const Value.absent(),
    this.entityId = const Value.absent(),
    this.localPayloadJson = const Value.absent(),
    this.serverPayloadJson = const Value.absent(),
    this.createdAt = const Value.absent(),
  });
  ConflictItemsCompanion.insert({
    this.id = const Value.absent(),
    this.entityType = const Value.absent(),
    required String entityId,
    required String localPayloadJson,
    required String serverPayloadJson,
    required DateTime createdAt,
  }) : entityId = Value(entityId),
       localPayloadJson = Value(localPayloadJson),
       serverPayloadJson = Value(serverPayloadJson),
       createdAt = Value(createdAt);
  static Insertable<ConflictItem> custom({
    Expression<int>? id,
    Expression<String>? entityType,
    Expression<String>? entityId,
    Expression<String>? localPayloadJson,
    Expression<String>? serverPayloadJson,
    Expression<DateTime>? createdAt,
  }) {
    return RawValuesInsertable({
      if (id != null) 'id': id,
      if (entityType != null) 'entity_type': entityType,
      if (entityId != null) 'entity_id': entityId,
      if (localPayloadJson != null) 'local_payload_json': localPayloadJson,
      if (serverPayloadJson != null) 'server_payload_json': serverPayloadJson,
      if (createdAt != null) 'created_at': createdAt,
    });
  }

  ConflictItemsCompanion copyWith({
    Value<int>? id,
    Value<String>? entityType,
    Value<String>? entityId,
    Value<String>? localPayloadJson,
    Value<String>? serverPayloadJson,
    Value<DateTime>? createdAt,
  }) {
    return ConflictItemsCompanion(
      id: id ?? this.id,
      entityType: entityType ?? this.entityType,
      entityId: entityId ?? this.entityId,
      localPayloadJson: localPayloadJson ?? this.localPayloadJson,
      serverPayloadJson: serverPayloadJson ?? this.serverPayloadJson,
      createdAt: createdAt ?? this.createdAt,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (id.present) {
      map['id'] = Variable<int>(id.value);
    }
    if (entityType.present) {
      map['entity_type'] = Variable<String>(entityType.value);
    }
    if (entityId.present) {
      map['entity_id'] = Variable<String>(entityId.value);
    }
    if (localPayloadJson.present) {
      map['local_payload_json'] = Variable<String>(localPayloadJson.value);
    }
    if (serverPayloadJson.present) {
      map['server_payload_json'] = Variable<String>(serverPayloadJson.value);
    }
    if (createdAt.present) {
      map['created_at'] = Variable<DateTime>(createdAt.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('ConflictItemsCompanion(')
          ..write('id: $id, ')
          ..write('entityType: $entityType, ')
          ..write('entityId: $entityId, ')
          ..write('localPayloadJson: $localPayloadJson, ')
          ..write('serverPayloadJson: $serverPayloadJson, ')
          ..write('createdAt: $createdAt')
          ..write(')'))
        .toString();
  }
}

class $LocalProjectsTable extends LocalProjects
    with TableInfo<$LocalProjectsTable, LocalProject> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $LocalProjectsTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _idMeta = const VerificationMeta('id');
  @override
  late final GeneratedColumn<String> id = GeneratedColumn<String>(
    'id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _keyMeta = const VerificationMeta('key');
  @override
  late final GeneratedColumn<String> key = GeneratedColumn<String>(
    'key',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _nameMeta = const VerificationMeta('name');
  @override
  late final GeneratedColumn<String> name = GeneratedColumn<String>(
    'name',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _descriptionMeta = const VerificationMeta(
    'description',
  );
  @override
  late final GeneratedColumn<String> description = GeneratedColumn<String>(
    'description',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _statusMeta = const VerificationMeta('status');
  @override
  late final GeneratedColumn<String> status = GeneratedColumn<String>(
    'status',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant('Active'),
  );
  static const VerificationMeta _priorityMeta = const VerificationMeta(
    'priority',
  );
  @override
  late final GeneratedColumn<String> priority = GeneratedColumn<String>(
    'priority',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant('Medium'),
  );
  static const VerificationMeta _isArchivedMeta = const VerificationMeta(
    'isArchived',
  );
  @override
  late final GeneratedColumn<bool> isArchived = GeneratedColumn<bool>(
    'is_archived',
    aliasedName,
    false,
    type: DriftSqlType.bool,
    requiredDuringInsert: false,
    defaultConstraints: GeneratedColumn.constraintIsAlways(
      'CHECK ("is_archived" IN (0, 1))',
    ),
    defaultValue: const Constant(false),
  );
  @override
  List<GeneratedColumn> get $columns => [
    id,
    key,
    name,
    description,
    status,
    priority,
    isArchived,
  ];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'local_projects';
  @override
  VerificationContext validateIntegrity(
    Insertable<LocalProject> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('id')) {
      context.handle(_idMeta, id.isAcceptableOrUnknown(data['id']!, _idMeta));
    } else if (isInserting) {
      context.missing(_idMeta);
    }
    if (data.containsKey('key')) {
      context.handle(
        _keyMeta,
        key.isAcceptableOrUnknown(data['key']!, _keyMeta),
      );
    } else if (isInserting) {
      context.missing(_keyMeta);
    }
    if (data.containsKey('name')) {
      context.handle(
        _nameMeta,
        name.isAcceptableOrUnknown(data['name']!, _nameMeta),
      );
    } else if (isInserting) {
      context.missing(_nameMeta);
    }
    if (data.containsKey('description')) {
      context.handle(
        _descriptionMeta,
        description.isAcceptableOrUnknown(
          data['description']!,
          _descriptionMeta,
        ),
      );
    }
    if (data.containsKey('status')) {
      context.handle(
        _statusMeta,
        status.isAcceptableOrUnknown(data['status']!, _statusMeta),
      );
    }
    if (data.containsKey('priority')) {
      context.handle(
        _priorityMeta,
        priority.isAcceptableOrUnknown(data['priority']!, _priorityMeta),
      );
    }
    if (data.containsKey('is_archived')) {
      context.handle(
        _isArchivedMeta,
        isArchived.isAcceptableOrUnknown(data['is_archived']!, _isArchivedMeta),
      );
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {id};
  @override
  LocalProject map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return LocalProject(
      id: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}id'],
      )!,
      key: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}key'],
      )!,
      name: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}name'],
      )!,
      description: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}description'],
      ),
      status: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}status'],
      )!,
      priority: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}priority'],
      )!,
      isArchived: attachedDatabase.typeMapping.read(
        DriftSqlType.bool,
        data['${effectivePrefix}is_archived'],
      )!,
    );
  }

  @override
  $LocalProjectsTable createAlias(String alias) {
    return $LocalProjectsTable(attachedDatabase, alias);
  }
}

class LocalProject extends DataClass implements Insertable<LocalProject> {
  final String id;
  final String key;
  final String name;
  final String? description;
  final String status;
  final String priority;
  final bool isArchived;
  const LocalProject({
    required this.id,
    required this.key,
    required this.name,
    this.description,
    required this.status,
    required this.priority,
    required this.isArchived,
  });
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['id'] = Variable<String>(id);
    map['key'] = Variable<String>(key);
    map['name'] = Variable<String>(name);
    if (!nullToAbsent || description != null) {
      map['description'] = Variable<String>(description);
    }
    map['status'] = Variable<String>(status);
    map['priority'] = Variable<String>(priority);
    map['is_archived'] = Variable<bool>(isArchived);
    return map;
  }

  LocalProjectsCompanion toCompanion(bool nullToAbsent) {
    return LocalProjectsCompanion(
      id: Value(id),
      key: Value(key),
      name: Value(name),
      description: description == null && nullToAbsent
          ? const Value.absent()
          : Value(description),
      status: Value(status),
      priority: Value(priority),
      isArchived: Value(isArchived),
    );
  }

  factory LocalProject.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return LocalProject(
      id: serializer.fromJson<String>(json['id']),
      key: serializer.fromJson<String>(json['key']),
      name: serializer.fromJson<String>(json['name']),
      description: serializer.fromJson<String?>(json['description']),
      status: serializer.fromJson<String>(json['status']),
      priority: serializer.fromJson<String>(json['priority']),
      isArchived: serializer.fromJson<bool>(json['isArchived']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'id': serializer.toJson<String>(id),
      'key': serializer.toJson<String>(key),
      'name': serializer.toJson<String>(name),
      'description': serializer.toJson<String?>(description),
      'status': serializer.toJson<String>(status),
      'priority': serializer.toJson<String>(priority),
      'isArchived': serializer.toJson<bool>(isArchived),
    };
  }

  LocalProject copyWith({
    String? id,
    String? key,
    String? name,
    Value<String?> description = const Value.absent(),
    String? status,
    String? priority,
    bool? isArchived,
  }) => LocalProject(
    id: id ?? this.id,
    key: key ?? this.key,
    name: name ?? this.name,
    description: description.present ? description.value : this.description,
    status: status ?? this.status,
    priority: priority ?? this.priority,
    isArchived: isArchived ?? this.isArchived,
  );
  LocalProject copyWithCompanion(LocalProjectsCompanion data) {
    return LocalProject(
      id: data.id.present ? data.id.value : this.id,
      key: data.key.present ? data.key.value : this.key,
      name: data.name.present ? data.name.value : this.name,
      description: data.description.present
          ? data.description.value
          : this.description,
      status: data.status.present ? data.status.value : this.status,
      priority: data.priority.present ? data.priority.value : this.priority,
      isArchived: data.isArchived.present
          ? data.isArchived.value
          : this.isArchived,
    );
  }

  @override
  String toString() {
    return (StringBuffer('LocalProject(')
          ..write('id: $id, ')
          ..write('key: $key, ')
          ..write('name: $name, ')
          ..write('description: $description, ')
          ..write('status: $status, ')
          ..write('priority: $priority, ')
          ..write('isArchived: $isArchived')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode =>
      Object.hash(id, key, name, description, status, priority, isArchived);
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is LocalProject &&
          other.id == this.id &&
          other.key == this.key &&
          other.name == this.name &&
          other.description == this.description &&
          other.status == this.status &&
          other.priority == this.priority &&
          other.isArchived == this.isArchived);
}

class LocalProjectsCompanion extends UpdateCompanion<LocalProject> {
  final Value<String> id;
  final Value<String> key;
  final Value<String> name;
  final Value<String?> description;
  final Value<String> status;
  final Value<String> priority;
  final Value<bool> isArchived;
  final Value<int> rowid;
  const LocalProjectsCompanion({
    this.id = const Value.absent(),
    this.key = const Value.absent(),
    this.name = const Value.absent(),
    this.description = const Value.absent(),
    this.status = const Value.absent(),
    this.priority = const Value.absent(),
    this.isArchived = const Value.absent(),
    this.rowid = const Value.absent(),
  });
  LocalProjectsCompanion.insert({
    required String id,
    required String key,
    required String name,
    this.description = const Value.absent(),
    this.status = const Value.absent(),
    this.priority = const Value.absent(),
    this.isArchived = const Value.absent(),
    this.rowid = const Value.absent(),
  }) : id = Value(id),
       key = Value(key),
       name = Value(name);
  static Insertable<LocalProject> custom({
    Expression<String>? id,
    Expression<String>? key,
    Expression<String>? name,
    Expression<String>? description,
    Expression<String>? status,
    Expression<String>? priority,
    Expression<bool>? isArchived,
    Expression<int>? rowid,
  }) {
    return RawValuesInsertable({
      if (id != null) 'id': id,
      if (key != null) 'key': key,
      if (name != null) 'name': name,
      if (description != null) 'description': description,
      if (status != null) 'status': status,
      if (priority != null) 'priority': priority,
      if (isArchived != null) 'is_archived': isArchived,
      if (rowid != null) 'rowid': rowid,
    });
  }

  LocalProjectsCompanion copyWith({
    Value<String>? id,
    Value<String>? key,
    Value<String>? name,
    Value<String?>? description,
    Value<String>? status,
    Value<String>? priority,
    Value<bool>? isArchived,
    Value<int>? rowid,
  }) {
    return LocalProjectsCompanion(
      id: id ?? this.id,
      key: key ?? this.key,
      name: name ?? this.name,
      description: description ?? this.description,
      status: status ?? this.status,
      priority: priority ?? this.priority,
      isArchived: isArchived ?? this.isArchived,
      rowid: rowid ?? this.rowid,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (id.present) {
      map['id'] = Variable<String>(id.value);
    }
    if (key.present) {
      map['key'] = Variable<String>(key.value);
    }
    if (name.present) {
      map['name'] = Variable<String>(name.value);
    }
    if (description.present) {
      map['description'] = Variable<String>(description.value);
    }
    if (status.present) {
      map['status'] = Variable<String>(status.value);
    }
    if (priority.present) {
      map['priority'] = Variable<String>(priority.value);
    }
    if (isArchived.present) {
      map['is_archived'] = Variable<bool>(isArchived.value);
    }
    if (rowid.present) {
      map['rowid'] = Variable<int>(rowid.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('LocalProjectsCompanion(')
          ..write('id: $id, ')
          ..write('key: $key, ')
          ..write('name: $name, ')
          ..write('description: $description, ')
          ..write('status: $status, ')
          ..write('priority: $priority, ')
          ..write('isArchived: $isArchived, ')
          ..write('rowid: $rowid')
          ..write(')'))
        .toString();
  }
}

class $LocalBoardColumnsTable extends LocalBoardColumns
    with TableInfo<$LocalBoardColumnsTable, LocalBoardColumn> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $LocalBoardColumnsTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _idMeta = const VerificationMeta('id');
  @override
  late final GeneratedColumn<String> id = GeneratedColumn<String>(
    'id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _projectIdMeta = const VerificationMeta(
    'projectId',
  );
  @override
  late final GeneratedColumn<String> projectId = GeneratedColumn<String>(
    'project_id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _nameMeta = const VerificationMeta('name');
  @override
  late final GeneratedColumn<String> name = GeneratedColumn<String>(
    'name',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _colorMeta = const VerificationMeta('color');
  @override
  late final GeneratedColumn<String> color = GeneratedColumn<String>(
    'color',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant('#6B7280'),
  );
  static const VerificationMeta _orderMeta = const VerificationMeta('order');
  @override
  late final GeneratedColumn<int> order = GeneratedColumn<int>(
    'order',
    aliasedName,
    false,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
    defaultValue: const Constant(0),
  );
  static const VerificationMeta _wipLimitMeta = const VerificationMeta(
    'wipLimit',
  );
  @override
  late final GeneratedColumn<int> wipLimit = GeneratedColumn<int>(
    'wip_limit',
    aliasedName,
    true,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _statusMappingMeta = const VerificationMeta(
    'statusMapping',
  );
  @override
  late final GeneratedColumn<String> statusMapping = GeneratedColumn<String>(
    'status_mapping',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant('Todo'),
  );
  static const VerificationMeta _isCompletionColumnMeta =
      const VerificationMeta('isCompletionColumn');
  @override
  late final GeneratedColumn<bool> isCompletionColumn = GeneratedColumn<bool>(
    'is_completion_column',
    aliasedName,
    false,
    type: DriftSqlType.bool,
    requiredDuringInsert: false,
    defaultConstraints: GeneratedColumn.constraintIsAlways(
      'CHECK ("is_completion_column" IN (0, 1))',
    ),
    defaultValue: const Constant(false),
  );
  @override
  List<GeneratedColumn> get $columns => [
    id,
    projectId,
    name,
    color,
    order,
    wipLimit,
    statusMapping,
    isCompletionColumn,
  ];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'local_board_columns';
  @override
  VerificationContext validateIntegrity(
    Insertable<LocalBoardColumn> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('id')) {
      context.handle(_idMeta, id.isAcceptableOrUnknown(data['id']!, _idMeta));
    } else if (isInserting) {
      context.missing(_idMeta);
    }
    if (data.containsKey('project_id')) {
      context.handle(
        _projectIdMeta,
        projectId.isAcceptableOrUnknown(data['project_id']!, _projectIdMeta),
      );
    } else if (isInserting) {
      context.missing(_projectIdMeta);
    }
    if (data.containsKey('name')) {
      context.handle(
        _nameMeta,
        name.isAcceptableOrUnknown(data['name']!, _nameMeta),
      );
    } else if (isInserting) {
      context.missing(_nameMeta);
    }
    if (data.containsKey('color')) {
      context.handle(
        _colorMeta,
        color.isAcceptableOrUnknown(data['color']!, _colorMeta),
      );
    }
    if (data.containsKey('order')) {
      context.handle(
        _orderMeta,
        order.isAcceptableOrUnknown(data['order']!, _orderMeta),
      );
    }
    if (data.containsKey('wip_limit')) {
      context.handle(
        _wipLimitMeta,
        wipLimit.isAcceptableOrUnknown(data['wip_limit']!, _wipLimitMeta),
      );
    }
    if (data.containsKey('status_mapping')) {
      context.handle(
        _statusMappingMeta,
        statusMapping.isAcceptableOrUnknown(
          data['status_mapping']!,
          _statusMappingMeta,
        ),
      );
    }
    if (data.containsKey('is_completion_column')) {
      context.handle(
        _isCompletionColumnMeta,
        isCompletionColumn.isAcceptableOrUnknown(
          data['is_completion_column']!,
          _isCompletionColumnMeta,
        ),
      );
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {id};
  @override
  LocalBoardColumn map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return LocalBoardColumn(
      id: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}id'],
      )!,
      projectId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}project_id'],
      )!,
      name: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}name'],
      )!,
      color: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}color'],
      )!,
      order: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}order'],
      )!,
      wipLimit: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}wip_limit'],
      ),
      statusMapping: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}status_mapping'],
      )!,
      isCompletionColumn: attachedDatabase.typeMapping.read(
        DriftSqlType.bool,
        data['${effectivePrefix}is_completion_column'],
      )!,
    );
  }

  @override
  $LocalBoardColumnsTable createAlias(String alias) {
    return $LocalBoardColumnsTable(attachedDatabase, alias);
  }
}

class LocalBoardColumn extends DataClass
    implements Insertable<LocalBoardColumn> {
  final String id;
  final String projectId;
  final String name;
  final String color;
  final int order;
  final int? wipLimit;
  final String statusMapping;
  final bool isCompletionColumn;
  const LocalBoardColumn({
    required this.id,
    required this.projectId,
    required this.name,
    required this.color,
    required this.order,
    this.wipLimit,
    required this.statusMapping,
    required this.isCompletionColumn,
  });
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['id'] = Variable<String>(id);
    map['project_id'] = Variable<String>(projectId);
    map['name'] = Variable<String>(name);
    map['color'] = Variable<String>(color);
    map['order'] = Variable<int>(order);
    if (!nullToAbsent || wipLimit != null) {
      map['wip_limit'] = Variable<int>(wipLimit);
    }
    map['status_mapping'] = Variable<String>(statusMapping);
    map['is_completion_column'] = Variable<bool>(isCompletionColumn);
    return map;
  }

  LocalBoardColumnsCompanion toCompanion(bool nullToAbsent) {
    return LocalBoardColumnsCompanion(
      id: Value(id),
      projectId: Value(projectId),
      name: Value(name),
      color: Value(color),
      order: Value(order),
      wipLimit: wipLimit == null && nullToAbsent
          ? const Value.absent()
          : Value(wipLimit),
      statusMapping: Value(statusMapping),
      isCompletionColumn: Value(isCompletionColumn),
    );
  }

  factory LocalBoardColumn.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return LocalBoardColumn(
      id: serializer.fromJson<String>(json['id']),
      projectId: serializer.fromJson<String>(json['projectId']),
      name: serializer.fromJson<String>(json['name']),
      color: serializer.fromJson<String>(json['color']),
      order: serializer.fromJson<int>(json['order']),
      wipLimit: serializer.fromJson<int?>(json['wipLimit']),
      statusMapping: serializer.fromJson<String>(json['statusMapping']),
      isCompletionColumn: serializer.fromJson<bool>(json['isCompletionColumn']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'id': serializer.toJson<String>(id),
      'projectId': serializer.toJson<String>(projectId),
      'name': serializer.toJson<String>(name),
      'color': serializer.toJson<String>(color),
      'order': serializer.toJson<int>(order),
      'wipLimit': serializer.toJson<int?>(wipLimit),
      'statusMapping': serializer.toJson<String>(statusMapping),
      'isCompletionColumn': serializer.toJson<bool>(isCompletionColumn),
    };
  }

  LocalBoardColumn copyWith({
    String? id,
    String? projectId,
    String? name,
    String? color,
    int? order,
    Value<int?> wipLimit = const Value.absent(),
    String? statusMapping,
    bool? isCompletionColumn,
  }) => LocalBoardColumn(
    id: id ?? this.id,
    projectId: projectId ?? this.projectId,
    name: name ?? this.name,
    color: color ?? this.color,
    order: order ?? this.order,
    wipLimit: wipLimit.present ? wipLimit.value : this.wipLimit,
    statusMapping: statusMapping ?? this.statusMapping,
    isCompletionColumn: isCompletionColumn ?? this.isCompletionColumn,
  );
  LocalBoardColumn copyWithCompanion(LocalBoardColumnsCompanion data) {
    return LocalBoardColumn(
      id: data.id.present ? data.id.value : this.id,
      projectId: data.projectId.present ? data.projectId.value : this.projectId,
      name: data.name.present ? data.name.value : this.name,
      color: data.color.present ? data.color.value : this.color,
      order: data.order.present ? data.order.value : this.order,
      wipLimit: data.wipLimit.present ? data.wipLimit.value : this.wipLimit,
      statusMapping: data.statusMapping.present
          ? data.statusMapping.value
          : this.statusMapping,
      isCompletionColumn: data.isCompletionColumn.present
          ? data.isCompletionColumn.value
          : this.isCompletionColumn,
    );
  }

  @override
  String toString() {
    return (StringBuffer('LocalBoardColumn(')
          ..write('id: $id, ')
          ..write('projectId: $projectId, ')
          ..write('name: $name, ')
          ..write('color: $color, ')
          ..write('order: $order, ')
          ..write('wipLimit: $wipLimit, ')
          ..write('statusMapping: $statusMapping, ')
          ..write('isCompletionColumn: $isCompletionColumn')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode => Object.hash(
    id,
    projectId,
    name,
    color,
    order,
    wipLimit,
    statusMapping,
    isCompletionColumn,
  );
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is LocalBoardColumn &&
          other.id == this.id &&
          other.projectId == this.projectId &&
          other.name == this.name &&
          other.color == this.color &&
          other.order == this.order &&
          other.wipLimit == this.wipLimit &&
          other.statusMapping == this.statusMapping &&
          other.isCompletionColumn == this.isCompletionColumn);
}

class LocalBoardColumnsCompanion extends UpdateCompanion<LocalBoardColumn> {
  final Value<String> id;
  final Value<String> projectId;
  final Value<String> name;
  final Value<String> color;
  final Value<int> order;
  final Value<int?> wipLimit;
  final Value<String> statusMapping;
  final Value<bool> isCompletionColumn;
  final Value<int> rowid;
  const LocalBoardColumnsCompanion({
    this.id = const Value.absent(),
    this.projectId = const Value.absent(),
    this.name = const Value.absent(),
    this.color = const Value.absent(),
    this.order = const Value.absent(),
    this.wipLimit = const Value.absent(),
    this.statusMapping = const Value.absent(),
    this.isCompletionColumn = const Value.absent(),
    this.rowid = const Value.absent(),
  });
  LocalBoardColumnsCompanion.insert({
    required String id,
    required String projectId,
    required String name,
    this.color = const Value.absent(),
    this.order = const Value.absent(),
    this.wipLimit = const Value.absent(),
    this.statusMapping = const Value.absent(),
    this.isCompletionColumn = const Value.absent(),
    this.rowid = const Value.absent(),
  }) : id = Value(id),
       projectId = Value(projectId),
       name = Value(name);
  static Insertable<LocalBoardColumn> custom({
    Expression<String>? id,
    Expression<String>? projectId,
    Expression<String>? name,
    Expression<String>? color,
    Expression<int>? order,
    Expression<int>? wipLimit,
    Expression<String>? statusMapping,
    Expression<bool>? isCompletionColumn,
    Expression<int>? rowid,
  }) {
    return RawValuesInsertable({
      if (id != null) 'id': id,
      if (projectId != null) 'project_id': projectId,
      if (name != null) 'name': name,
      if (color != null) 'color': color,
      if (order != null) 'order': order,
      if (wipLimit != null) 'wip_limit': wipLimit,
      if (statusMapping != null) 'status_mapping': statusMapping,
      if (isCompletionColumn != null)
        'is_completion_column': isCompletionColumn,
      if (rowid != null) 'rowid': rowid,
    });
  }

  LocalBoardColumnsCompanion copyWith({
    Value<String>? id,
    Value<String>? projectId,
    Value<String>? name,
    Value<String>? color,
    Value<int>? order,
    Value<int?>? wipLimit,
    Value<String>? statusMapping,
    Value<bool>? isCompletionColumn,
    Value<int>? rowid,
  }) {
    return LocalBoardColumnsCompanion(
      id: id ?? this.id,
      projectId: projectId ?? this.projectId,
      name: name ?? this.name,
      color: color ?? this.color,
      order: order ?? this.order,
      wipLimit: wipLimit ?? this.wipLimit,
      statusMapping: statusMapping ?? this.statusMapping,
      isCompletionColumn: isCompletionColumn ?? this.isCompletionColumn,
      rowid: rowid ?? this.rowid,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (id.present) {
      map['id'] = Variable<String>(id.value);
    }
    if (projectId.present) {
      map['project_id'] = Variable<String>(projectId.value);
    }
    if (name.present) {
      map['name'] = Variable<String>(name.value);
    }
    if (color.present) {
      map['color'] = Variable<String>(color.value);
    }
    if (order.present) {
      map['order'] = Variable<int>(order.value);
    }
    if (wipLimit.present) {
      map['wip_limit'] = Variable<int>(wipLimit.value);
    }
    if (statusMapping.present) {
      map['status_mapping'] = Variable<String>(statusMapping.value);
    }
    if (isCompletionColumn.present) {
      map['is_completion_column'] = Variable<bool>(isCompletionColumn.value);
    }
    if (rowid.present) {
      map['rowid'] = Variable<int>(rowid.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('LocalBoardColumnsCompanion(')
          ..write('id: $id, ')
          ..write('projectId: $projectId, ')
          ..write('name: $name, ')
          ..write('color: $color, ')
          ..write('order: $order, ')
          ..write('wipLimit: $wipLimit, ')
          ..write('statusMapping: $statusMapping, ')
          ..write('isCompletionColumn: $isCompletionColumn, ')
          ..write('rowid: $rowid')
          ..write(')'))
        .toString();
  }
}

class $LocalUsersTable extends LocalUsers
    with TableInfo<$LocalUsersTable, LocalUser> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $LocalUsersTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _idMeta = const VerificationMeta('id');
  @override
  late final GeneratedColumn<String> id = GeneratedColumn<String>(
    'id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _emailMeta = const VerificationMeta('email');
  @override
  late final GeneratedColumn<String> email = GeneratedColumn<String>(
    'email',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant(''),
  );
  static const VerificationMeta _firstNameMeta = const VerificationMeta(
    'firstName',
  );
  @override
  late final GeneratedColumn<String> firstName = GeneratedColumn<String>(
    'first_name',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant(''),
  );
  static const VerificationMeta _lastNameMeta = const VerificationMeta(
    'lastName',
  );
  @override
  late final GeneratedColumn<String> lastName = GeneratedColumn<String>(
    'last_name',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant(''),
  );
  @override
  List<GeneratedColumn> get $columns => [id, email, firstName, lastName];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'local_users';
  @override
  VerificationContext validateIntegrity(
    Insertable<LocalUser> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('id')) {
      context.handle(_idMeta, id.isAcceptableOrUnknown(data['id']!, _idMeta));
    } else if (isInserting) {
      context.missing(_idMeta);
    }
    if (data.containsKey('email')) {
      context.handle(
        _emailMeta,
        email.isAcceptableOrUnknown(data['email']!, _emailMeta),
      );
    }
    if (data.containsKey('first_name')) {
      context.handle(
        _firstNameMeta,
        firstName.isAcceptableOrUnknown(data['first_name']!, _firstNameMeta),
      );
    }
    if (data.containsKey('last_name')) {
      context.handle(
        _lastNameMeta,
        lastName.isAcceptableOrUnknown(data['last_name']!, _lastNameMeta),
      );
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {id};
  @override
  LocalUser map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return LocalUser(
      id: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}id'],
      )!,
      email: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}email'],
      )!,
      firstName: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}first_name'],
      )!,
      lastName: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}last_name'],
      )!,
    );
  }

  @override
  $LocalUsersTable createAlias(String alias) {
    return $LocalUsersTable(attachedDatabase, alias);
  }
}

class LocalUser extends DataClass implements Insertable<LocalUser> {
  final String id;
  final String email;
  final String firstName;
  final String lastName;
  const LocalUser({
    required this.id,
    required this.email,
    required this.firstName,
    required this.lastName,
  });
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['id'] = Variable<String>(id);
    map['email'] = Variable<String>(email);
    map['first_name'] = Variable<String>(firstName);
    map['last_name'] = Variable<String>(lastName);
    return map;
  }

  LocalUsersCompanion toCompanion(bool nullToAbsent) {
    return LocalUsersCompanion(
      id: Value(id),
      email: Value(email),
      firstName: Value(firstName),
      lastName: Value(lastName),
    );
  }

  factory LocalUser.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return LocalUser(
      id: serializer.fromJson<String>(json['id']),
      email: serializer.fromJson<String>(json['email']),
      firstName: serializer.fromJson<String>(json['firstName']),
      lastName: serializer.fromJson<String>(json['lastName']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'id': serializer.toJson<String>(id),
      'email': serializer.toJson<String>(email),
      'firstName': serializer.toJson<String>(firstName),
      'lastName': serializer.toJson<String>(lastName),
    };
  }

  LocalUser copyWith({
    String? id,
    String? email,
    String? firstName,
    String? lastName,
  }) => LocalUser(
    id: id ?? this.id,
    email: email ?? this.email,
    firstName: firstName ?? this.firstName,
    lastName: lastName ?? this.lastName,
  );
  LocalUser copyWithCompanion(LocalUsersCompanion data) {
    return LocalUser(
      id: data.id.present ? data.id.value : this.id,
      email: data.email.present ? data.email.value : this.email,
      firstName: data.firstName.present ? data.firstName.value : this.firstName,
      lastName: data.lastName.present ? data.lastName.value : this.lastName,
    );
  }

  @override
  String toString() {
    return (StringBuffer('LocalUser(')
          ..write('id: $id, ')
          ..write('email: $email, ')
          ..write('firstName: $firstName, ')
          ..write('lastName: $lastName')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode => Object.hash(id, email, firstName, lastName);
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is LocalUser &&
          other.id == this.id &&
          other.email == this.email &&
          other.firstName == this.firstName &&
          other.lastName == this.lastName);
}

class LocalUsersCompanion extends UpdateCompanion<LocalUser> {
  final Value<String> id;
  final Value<String> email;
  final Value<String> firstName;
  final Value<String> lastName;
  final Value<int> rowid;
  const LocalUsersCompanion({
    this.id = const Value.absent(),
    this.email = const Value.absent(),
    this.firstName = const Value.absent(),
    this.lastName = const Value.absent(),
    this.rowid = const Value.absent(),
  });
  LocalUsersCompanion.insert({
    required String id,
    this.email = const Value.absent(),
    this.firstName = const Value.absent(),
    this.lastName = const Value.absent(),
    this.rowid = const Value.absent(),
  }) : id = Value(id);
  static Insertable<LocalUser> custom({
    Expression<String>? id,
    Expression<String>? email,
    Expression<String>? firstName,
    Expression<String>? lastName,
    Expression<int>? rowid,
  }) {
    return RawValuesInsertable({
      if (id != null) 'id': id,
      if (email != null) 'email': email,
      if (firstName != null) 'first_name': firstName,
      if (lastName != null) 'last_name': lastName,
      if (rowid != null) 'rowid': rowid,
    });
  }

  LocalUsersCompanion copyWith({
    Value<String>? id,
    Value<String>? email,
    Value<String>? firstName,
    Value<String>? lastName,
    Value<int>? rowid,
  }) {
    return LocalUsersCompanion(
      id: id ?? this.id,
      email: email ?? this.email,
      firstName: firstName ?? this.firstName,
      lastName: lastName ?? this.lastName,
      rowid: rowid ?? this.rowid,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (id.present) {
      map['id'] = Variable<String>(id.value);
    }
    if (email.present) {
      map['email'] = Variable<String>(email.value);
    }
    if (firstName.present) {
      map['first_name'] = Variable<String>(firstName.value);
    }
    if (lastName.present) {
      map['last_name'] = Variable<String>(lastName.value);
    }
    if (rowid.present) {
      map['rowid'] = Variable<int>(rowid.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('LocalUsersCompanion(')
          ..write('id: $id, ')
          ..write('email: $email, ')
          ..write('firstName: $firstName, ')
          ..write('lastName: $lastName, ')
          ..write('rowid: $rowid')
          ..write(')'))
        .toString();
  }
}

class $SyncMetaTable extends SyncMeta
    with TableInfo<$SyncMetaTable, SyncMetaData> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $SyncMetaTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _keyMeta = const VerificationMeta('key');
  @override
  late final GeneratedColumn<String> key = GeneratedColumn<String>(
    'key',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _valueMeta = const VerificationMeta('value');
  @override
  late final GeneratedColumn<String> value = GeneratedColumn<String>(
    'value',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  @override
  List<GeneratedColumn> get $columns => [key, value];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'sync_meta';
  @override
  VerificationContext validateIntegrity(
    Insertable<SyncMetaData> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('key')) {
      context.handle(
        _keyMeta,
        key.isAcceptableOrUnknown(data['key']!, _keyMeta),
      );
    } else if (isInserting) {
      context.missing(_keyMeta);
    }
    if (data.containsKey('value')) {
      context.handle(
        _valueMeta,
        value.isAcceptableOrUnknown(data['value']!, _valueMeta),
      );
    } else if (isInserting) {
      context.missing(_valueMeta);
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {key};
  @override
  SyncMetaData map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return SyncMetaData(
      key: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}key'],
      )!,
      value: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}value'],
      )!,
    );
  }

  @override
  $SyncMetaTable createAlias(String alias) {
    return $SyncMetaTable(attachedDatabase, alias);
  }
}

class SyncMetaData extends DataClass implements Insertable<SyncMetaData> {
  final String key;
  final String value;
  const SyncMetaData({required this.key, required this.value});
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['key'] = Variable<String>(key);
    map['value'] = Variable<String>(value);
    return map;
  }

  SyncMetaCompanion toCompanion(bool nullToAbsent) {
    return SyncMetaCompanion(key: Value(key), value: Value(value));
  }

  factory SyncMetaData.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return SyncMetaData(
      key: serializer.fromJson<String>(json['key']),
      value: serializer.fromJson<String>(json['value']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'key': serializer.toJson<String>(key),
      'value': serializer.toJson<String>(value),
    };
  }

  SyncMetaData copyWith({String? key, String? value}) =>
      SyncMetaData(key: key ?? this.key, value: value ?? this.value);
  SyncMetaData copyWithCompanion(SyncMetaCompanion data) {
    return SyncMetaData(
      key: data.key.present ? data.key.value : this.key,
      value: data.value.present ? data.value.value : this.value,
    );
  }

  @override
  String toString() {
    return (StringBuffer('SyncMetaData(')
          ..write('key: $key, ')
          ..write('value: $value')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode => Object.hash(key, value);
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is SyncMetaData &&
          other.key == this.key &&
          other.value == this.value);
}

class SyncMetaCompanion extends UpdateCompanion<SyncMetaData> {
  final Value<String> key;
  final Value<String> value;
  final Value<int> rowid;
  const SyncMetaCompanion({
    this.key = const Value.absent(),
    this.value = const Value.absent(),
    this.rowid = const Value.absent(),
  });
  SyncMetaCompanion.insert({
    required String key,
    required String value,
    this.rowid = const Value.absent(),
  }) : key = Value(key),
       value = Value(value);
  static Insertable<SyncMetaData> custom({
    Expression<String>? key,
    Expression<String>? value,
    Expression<int>? rowid,
  }) {
    return RawValuesInsertable({
      if (key != null) 'key': key,
      if (value != null) 'value': value,
      if (rowid != null) 'rowid': rowid,
    });
  }

  SyncMetaCompanion copyWith({
    Value<String>? key,
    Value<String>? value,
    Value<int>? rowid,
  }) {
    return SyncMetaCompanion(
      key: key ?? this.key,
      value: value ?? this.value,
      rowid: rowid ?? this.rowid,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (key.present) {
      map['key'] = Variable<String>(key.value);
    }
    if (value.present) {
      map['value'] = Variable<String>(value.value);
    }
    if (rowid.present) {
      map['rowid'] = Variable<int>(rowid.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('SyncMetaCompanion(')
          ..write('key: $key, ')
          ..write('value: $value, ')
          ..write('rowid: $rowid')
          ..write(')'))
        .toString();
  }
}

abstract class _$AppDatabase extends GeneratedDatabase {
  _$AppDatabase(QueryExecutor e) : super(e);
  $AppDatabaseManager get managers => $AppDatabaseManager(this);
  late final $LocalTasksTable localTasks = $LocalTasksTable(this);
  late final $SyncQueueItemsTable syncQueueItems = $SyncQueueItemsTable(this);
  late final $ConflictItemsTable conflictItems = $ConflictItemsTable(this);
  late final $LocalProjectsTable localProjects = $LocalProjectsTable(this);
  late final $LocalBoardColumnsTable localBoardColumns =
      $LocalBoardColumnsTable(this);
  late final $LocalUsersTable localUsers = $LocalUsersTable(this);
  late final $SyncMetaTable syncMeta = $SyncMetaTable(this);
  @override
  Iterable<TableInfo<Table, Object?>> get allTables =>
      allSchemaEntities.whereType<TableInfo<Table, Object?>>();
  @override
  List<DatabaseSchemaEntity> get allSchemaEntities => [
    localTasks,
    syncQueueItems,
    conflictItems,
    localProjects,
    localBoardColumns,
    localUsers,
    syncMeta,
  ];
}

typedef $$LocalTasksTableCreateCompanionBuilder = LocalTasksCompanion Function({
  required String id,
  Value<String> taskKey,
  required String projectId,
  Value<String?> epicId,
  Value<String?> sprintId,
  Value<String?> parentTaskId,
  required String title,
  Value<String?> description,
  Value<int> taskType,
  Value<int> status,
  Value<int> priority,
  Value<String?> assigneeUserId,
  Value<String?> reporterUserId,
  Value<String?> teamId,
  Value<String?> boardColumnId,
  Value<int?> storyPoints,
  Value<double?> estimatedHours,
  Value<double?> remainingHours,
  Value<double> loggedHours,
  Value<DateTime?> startDate,
  Value<DateTime?> dueDate,
  Value<double> position,
  Value<bool> isDeleted,
  Value<String> rowVersion,
  Value<DateTime?> updatedAt,
  Value<String> syncState,
  Value<bool> needsReview,
  Value<int> rowid,
});
typedef $$LocalTasksTableUpdateCompanionBuilder = LocalTasksCompanion Function({
  Value<String> id,
  Value<String> taskKey,
  Value<String> projectId,
  Value<String?> epicId,
  Value<String?> sprintId,
  Value<String?> parentTaskId,
  Value<String> title,
  Value<String?> description,
  Value<int> taskType,
  Value<int> status,
  Value<int> priority,
  Value<String?> assigneeUserId,
  Value<String?> reporterUserId,
  Value<String?> teamId,
  Value<String?> boardColumnId,
  Value<int?> storyPoints,
  Value<double?> estimatedHours,
  Value<double?> remainingHours,
  Value<double> loggedHours,
  Value<DateTime?> startDate,
  Value<DateTime?> dueDate,
  Value<double> position,
  Value<bool> isDeleted,
  Value<String> rowVersion,
  Value<DateTime?> updatedAt,
  Value<String> syncState,
  Value<bool> needsReview,
  Value<int> rowid,
});

class $$LocalTasksTableFilterComposer
    extends Composer<_$AppDatabase, $LocalTasksTable> {
  $$LocalTasksTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<String> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get taskKey => $composableBuilder(
    column: $table.taskKey,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get projectId => $composableBuilder(
    column: $table.projectId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get epicId => $composableBuilder(
    column: $table.epicId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get sprintId => $composableBuilder(
    column: $table.sprintId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get parentTaskId => $composableBuilder(
    column: $table.parentTaskId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get title => $composableBuilder(
    column: $table.title,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get description => $composableBuilder(
    column: $table.description,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<int> get taskType => $composableBuilder(
    column: $table.taskType,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<int> get status => $composableBuilder(
    column: $table.status,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<int> get priority => $composableBuilder(
    column: $table.priority,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get assigneeUserId => $composableBuilder(
    column: $table.assigneeUserId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get reporterUserId => $composableBuilder(
    column: $table.reporterUserId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get teamId => $composableBuilder(
    column: $table.teamId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get boardColumnId => $composableBuilder(
    column: $table.boardColumnId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<int> get storyPoints => $composableBuilder(
    column: $table.storyPoints,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get estimatedHours => $composableBuilder(
    column: $table.estimatedHours,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get remainingHours => $composableBuilder(
    column: $table.remainingHours,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get loggedHours => $composableBuilder(
    column: $table.loggedHours,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get startDate => $composableBuilder(
    column: $table.startDate,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get dueDate => $composableBuilder(
    column: $table.dueDate,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get position => $composableBuilder(
    column: $table.position,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<bool> get isDeleted => $composableBuilder(
    column: $table.isDeleted,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get rowVersion => $composableBuilder(
    column: $table.rowVersion,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get updatedAt => $composableBuilder(
    column: $table.updatedAt,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get syncState => $composableBuilder(
    column: $table.syncState,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<bool> get needsReview => $composableBuilder(
    column: $table.needsReview,
    builder: (column) => ColumnFilters(column),
  );
}

class $$LocalTasksTableOrderingComposer
    extends Composer<_$AppDatabase, $LocalTasksTable> {
  $$LocalTasksTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<String> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get taskKey => $composableBuilder(
    column: $table.taskKey,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get projectId => $composableBuilder(
    column: $table.projectId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get epicId => $composableBuilder(
    column: $table.epicId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get sprintId => $composableBuilder(
    column: $table.sprintId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get parentTaskId => $composableBuilder(
    column: $table.parentTaskId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get title => $composableBuilder(
    column: $table.title,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get description => $composableBuilder(
    column: $table.description,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<int> get taskType => $composableBuilder(
    column: $table.taskType,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<int> get status => $composableBuilder(
    column: $table.status,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<int> get priority => $composableBuilder(
    column: $table.priority,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get assigneeUserId => $composableBuilder(
    column: $table.assigneeUserId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get reporterUserId => $composableBuilder(
    column: $table.reporterUserId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get teamId => $composableBuilder(
    column: $table.teamId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get boardColumnId => $composableBuilder(
    column: $table.boardColumnId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<int> get storyPoints => $composableBuilder(
    column: $table.storyPoints,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get estimatedHours => $composableBuilder(
    column: $table.estimatedHours,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get remainingHours => $composableBuilder(
    column: $table.remainingHours,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get loggedHours => $composableBuilder(
    column: $table.loggedHours,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get startDate => $composableBuilder(
    column: $table.startDate,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get dueDate => $composableBuilder(
    column: $table.dueDate,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get position => $composableBuilder(
    column: $table.position,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<bool> get isDeleted => $composableBuilder(
    column: $table.isDeleted,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get rowVersion => $composableBuilder(
    column: $table.rowVersion,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get updatedAt => $composableBuilder(
    column: $table.updatedAt,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get syncState => $composableBuilder(
    column: $table.syncState,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<bool> get needsReview => $composableBuilder(
    column: $table.needsReview,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$LocalTasksTableAnnotationComposer
    extends Composer<_$AppDatabase, $LocalTasksTable> {
  $$LocalTasksTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<String> get id =>
      $composableBuilder(column: $table.id, builder: (column) => column);

  GeneratedColumn<String> get taskKey =>
      $composableBuilder(column: $table.taskKey, builder: (column) => column);

  GeneratedColumn<String> get projectId =>
      $composableBuilder(column: $table.projectId, builder: (column) => column);

  GeneratedColumn<String> get epicId =>
      $composableBuilder(column: $table.epicId, builder: (column) => column);

  GeneratedColumn<String> get sprintId =>
      $composableBuilder(column: $table.sprintId, builder: (column) => column);

  GeneratedColumn<String> get parentTaskId => $composableBuilder(
    column: $table.parentTaskId,
    builder: (column) => column,
  );

  GeneratedColumn<String> get title =>
      $composableBuilder(column: $table.title, builder: (column) => column);

  GeneratedColumn<String> get description => $composableBuilder(
    column: $table.description,
    builder: (column) => column,
  );

  GeneratedColumn<int> get taskType =>
      $composableBuilder(column: $table.taskType, builder: (column) => column);

  GeneratedColumn<int> get status =>
      $composableBuilder(column: $table.status, builder: (column) => column);

  GeneratedColumn<int> get priority =>
      $composableBuilder(column: $table.priority, builder: (column) => column);

  GeneratedColumn<String> get assigneeUserId => $composableBuilder(
    column: $table.assigneeUserId,
    builder: (column) => column,
  );

  GeneratedColumn<String> get reporterUserId => $composableBuilder(
    column: $table.reporterUserId,
    builder: (column) => column,
  );

  GeneratedColumn<String> get teamId =>
      $composableBuilder(column: $table.teamId, builder: (column) => column);

  GeneratedColumn<String> get boardColumnId => $composableBuilder(
    column: $table.boardColumnId,
    builder: (column) => column,
  );

  GeneratedColumn<int> get storyPoints => $composableBuilder(
    column: $table.storyPoints,
    builder: (column) => column,
  );

  GeneratedColumn<double> get estimatedHours => $composableBuilder(
    column: $table.estimatedHours,
    builder: (column) => column,
  );

  GeneratedColumn<double> get remainingHours => $composableBuilder(
    column: $table.remainingHours,
    builder: (column) => column,
  );

  GeneratedColumn<double> get loggedHours => $composableBuilder(
    column: $table.loggedHours,
    builder: (column) => column,
  );

  GeneratedColumn<DateTime> get startDate =>
      $composableBuilder(column: $table.startDate, builder: (column) => column);

  GeneratedColumn<DateTime> get dueDate =>
      $composableBuilder(column: $table.dueDate, builder: (column) => column);

  GeneratedColumn<double> get position =>
      $composableBuilder(column: $table.position, builder: (column) => column);

  GeneratedColumn<bool> get isDeleted =>
      $composableBuilder(column: $table.isDeleted, builder: (column) => column);

  GeneratedColumn<String> get rowVersion => $composableBuilder(
    column: $table.rowVersion,
    builder: (column) => column,
  );

  GeneratedColumn<DateTime> get updatedAt =>
      $composableBuilder(column: $table.updatedAt, builder: (column) => column);

  GeneratedColumn<String> get syncState =>
      $composableBuilder(column: $table.syncState, builder: (column) => column);

  GeneratedColumn<bool> get needsReview => $composableBuilder(
    column: $table.needsReview,
    builder: (column) => column,
  );
}

class $$LocalTasksTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $LocalTasksTable,
          LocalTask,
          $$LocalTasksTableFilterComposer,
          $$LocalTasksTableOrderingComposer,
          $$LocalTasksTableAnnotationComposer,
          $$LocalTasksTableCreateCompanionBuilder,
          $$LocalTasksTableUpdateCompanionBuilder,
          (
            LocalTask,
            BaseReferences<_$AppDatabase, $LocalTasksTable, LocalTask>,
          ),
          LocalTask,
          PrefetchHooks Function()
        > {
  $$LocalTasksTableTableManager(_$AppDatabase db, $LocalTasksTable table)
    : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$LocalTasksTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$LocalTasksTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$LocalTasksTableAnnotationComposer($db: db, $table: table),
          updateCompanionCallback:
              ({
                Value<String> id = const Value.absent(),
                Value<String> taskKey = const Value.absent(),
                Value<String> projectId = const Value.absent(),
                Value<String?> epicId = const Value.absent(),
                Value<String?> sprintId = const Value.absent(),
                Value<String?> parentTaskId = const Value.absent(),
                Value<String> title = const Value.absent(),
                Value<String?> description = const Value.absent(),
                Value<int> taskType = const Value.absent(),
                Value<int> status = const Value.absent(),
                Value<int> priority = const Value.absent(),
                Value<String?> assigneeUserId = const Value.absent(),
                Value<String?> reporterUserId = const Value.absent(),
                Value<String?> teamId = const Value.absent(),
                Value<String?> boardColumnId = const Value.absent(),
                Value<int?> storyPoints = const Value.absent(),
                Value<double?> estimatedHours = const Value.absent(),
                Value<double?> remainingHours = const Value.absent(),
                Value<double> loggedHours = const Value.absent(),
                Value<DateTime?> startDate = const Value.absent(),
                Value<DateTime?> dueDate = const Value.absent(),
                Value<double> position = const Value.absent(),
                Value<bool> isDeleted = const Value.absent(),
                Value<String> rowVersion = const Value.absent(),
                Value<DateTime?> updatedAt = const Value.absent(),
                Value<String> syncState = const Value.absent(),
                Value<bool> needsReview = const Value.absent(),
                Value<int> rowid = const Value.absent(),
              }) => LocalTasksCompanion(
                id: id,
                taskKey: taskKey,
                projectId: projectId,
                epicId: epicId,
                sprintId: sprintId,
                parentTaskId: parentTaskId,
                title: title,
                description: description,
                taskType: taskType,
                status: status,
                priority: priority,
                assigneeUserId: assigneeUserId,
                reporterUserId: reporterUserId,
                teamId: teamId,
                boardColumnId: boardColumnId,
                storyPoints: storyPoints,
                estimatedHours: estimatedHours,
                remainingHours: remainingHours,
                loggedHours: loggedHours,
                startDate: startDate,
                dueDate: dueDate,
                position: position,
                isDeleted: isDeleted,
                rowVersion: rowVersion,
                updatedAt: updatedAt,
                syncState: syncState,
                needsReview: needsReview,
                rowid: rowid,
              ),
          createCompanionCallback:
              ({
                required String id,
                Value<String> taskKey = const Value.absent(),
                required String projectId,
                Value<String?> epicId = const Value.absent(),
                Value<String?> sprintId = const Value.absent(),
                Value<String?> parentTaskId = const Value.absent(),
                required String title,
                Value<String?> description = const Value.absent(),
                Value<int> taskType = const Value.absent(),
                Value<int> status = const Value.absent(),
                Value<int> priority = const Value.absent(),
                Value<String?> assigneeUserId = const Value.absent(),
                Value<String?> reporterUserId = const Value.absent(),
                Value<String?> teamId = const Value.absent(),
                Value<String?> boardColumnId = const Value.absent(),
                Value<int?> storyPoints = const Value.absent(),
                Value<double?> estimatedHours = const Value.absent(),
                Value<double?> remainingHours = const Value.absent(),
                Value<double> loggedHours = const Value.absent(),
                Value<DateTime?> startDate = const Value.absent(),
                Value<DateTime?> dueDate = const Value.absent(),
                Value<double> position = const Value.absent(),
                Value<bool> isDeleted = const Value.absent(),
                Value<String> rowVersion = const Value.absent(),
                Value<DateTime?> updatedAt = const Value.absent(),
                Value<String> syncState = const Value.absent(),
                Value<bool> needsReview = const Value.absent(),
                Value<int> rowid = const Value.absent(),
              }) => LocalTasksCompanion.insert(
                id: id,
                taskKey: taskKey,
                projectId: projectId,
                epicId: epicId,
                sprintId: sprintId,
                parentTaskId: parentTaskId,
                title: title,
                description: description,
                taskType: taskType,
                status: status,
                priority: priority,
                assigneeUserId: assigneeUserId,
                reporterUserId: reporterUserId,
                teamId: teamId,
                boardColumnId: boardColumnId,
                storyPoints: storyPoints,
                estimatedHours: estimatedHours,
                remainingHours: remainingHours,
                loggedHours: loggedHours,
                startDate: startDate,
                dueDate: dueDate,
                position: position,
                isDeleted: isDeleted,
                rowVersion: rowVersion,
                updatedAt: updatedAt,
                syncState: syncState,
                needsReview: needsReview,
                rowid: rowid,
              ),
          withReferenceMapper: (p0) => p0
              .map(
                (e) => (
                  e.readTable<$LocalTasksTable, LocalTask>(table),
                  BaseReferences<_$AppDatabase, $LocalTasksTable, LocalTask>(
                    db,
                    table,
                    e,
                  ),
                ),
              )
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$LocalTasksTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $LocalTasksTable,
      LocalTask,
      $$LocalTasksTableFilterComposer,
      $$LocalTasksTableOrderingComposer,
      $$LocalTasksTableAnnotationComposer,
      $$LocalTasksTableCreateCompanionBuilder,
      $$LocalTasksTableUpdateCompanionBuilder,
      (LocalTask, BaseReferences<_$AppDatabase, $LocalTasksTable, LocalTask>),
      LocalTask,
      PrefetchHooks Function()
    >;
typedef $$SyncQueueItemsTableCreateCompanionBuilder =
    SyncQueueItemsCompanion Function({
      required String operationId,
      Value<String> entityType,
      required String entityId,
      required String changeType,
      Value<String?> payloadJson,
      Value<String?> baseRowVersion,
      Value<String> status,
      Value<int> attemptCount,
      Value<DateTime?> nextAttemptAt,
      Value<String?> lastError,
      required DateTime createdAt,
      Value<int> rowid,
    });
typedef $$SyncQueueItemsTableUpdateCompanionBuilder =
    SyncQueueItemsCompanion Function({
      Value<String> operationId,
      Value<String> entityType,
      Value<String> entityId,
      Value<String> changeType,
      Value<String?> payloadJson,
      Value<String?> baseRowVersion,
      Value<String> status,
      Value<int> attemptCount,
      Value<DateTime?> nextAttemptAt,
      Value<String?> lastError,
      Value<DateTime> createdAt,
      Value<int> rowid,
    });

class $$SyncQueueItemsTableFilterComposer
    extends Composer<_$AppDatabase, $SyncQueueItemsTable> {
  $$SyncQueueItemsTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<String> get operationId => $composableBuilder(
    column: $table.operationId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get entityType => $composableBuilder(
    column: $table.entityType,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get entityId => $composableBuilder(
    column: $table.entityId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get changeType => $composableBuilder(
    column: $table.changeType,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get payloadJson => $composableBuilder(
    column: $table.payloadJson,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get baseRowVersion => $composableBuilder(
    column: $table.baseRowVersion,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get status => $composableBuilder(
    column: $table.status,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<int> get attemptCount => $composableBuilder(
    column: $table.attemptCount,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get nextAttemptAt => $composableBuilder(
    column: $table.nextAttemptAt,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get lastError => $composableBuilder(
    column: $table.lastError,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get createdAt => $composableBuilder(
    column: $table.createdAt,
    builder: (column) => ColumnFilters(column),
  );
}

class $$SyncQueueItemsTableOrderingComposer
    extends Composer<_$AppDatabase, $SyncQueueItemsTable> {
  $$SyncQueueItemsTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<String> get operationId => $composableBuilder(
    column: $table.operationId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get entityType => $composableBuilder(
    column: $table.entityType,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get entityId => $composableBuilder(
    column: $table.entityId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get changeType => $composableBuilder(
    column: $table.changeType,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get payloadJson => $composableBuilder(
    column: $table.payloadJson,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get baseRowVersion => $composableBuilder(
    column: $table.baseRowVersion,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get status => $composableBuilder(
    column: $table.status,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<int> get attemptCount => $composableBuilder(
    column: $table.attemptCount,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get nextAttemptAt => $composableBuilder(
    column: $table.nextAttemptAt,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get lastError => $composableBuilder(
    column: $table.lastError,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get createdAt => $composableBuilder(
    column: $table.createdAt,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$SyncQueueItemsTableAnnotationComposer
    extends Composer<_$AppDatabase, $SyncQueueItemsTable> {
  $$SyncQueueItemsTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<String> get operationId => $composableBuilder(
    column: $table.operationId,
    builder: (column) => column,
  );

  GeneratedColumn<String> get entityType => $composableBuilder(
    column: $table.entityType,
    builder: (column) => column,
  );

  GeneratedColumn<String> get entityId =>
      $composableBuilder(column: $table.entityId, builder: (column) => column);

  GeneratedColumn<String> get changeType => $composableBuilder(
    column: $table.changeType,
    builder: (column) => column,
  );

  GeneratedColumn<String> get payloadJson => $composableBuilder(
    column: $table.payloadJson,
    builder: (column) => column,
  );

  GeneratedColumn<String> get baseRowVersion => $composableBuilder(
    column: $table.baseRowVersion,
    builder: (column) => column,
  );

  GeneratedColumn<String> get status =>
      $composableBuilder(column: $table.status, builder: (column) => column);

  GeneratedColumn<int> get attemptCount => $composableBuilder(
    column: $table.attemptCount,
    builder: (column) => column,
  );

  GeneratedColumn<DateTime> get nextAttemptAt => $composableBuilder(
    column: $table.nextAttemptAt,
    builder: (column) => column,
  );

  GeneratedColumn<String> get lastError =>
      $composableBuilder(column: $table.lastError, builder: (column) => column);

  GeneratedColumn<DateTime> get createdAt =>
      $composableBuilder(column: $table.createdAt, builder: (column) => column);
}

class $$SyncQueueItemsTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $SyncQueueItemsTable,
          SyncQueueItem,
          $$SyncQueueItemsTableFilterComposer,
          $$SyncQueueItemsTableOrderingComposer,
          $$SyncQueueItemsTableAnnotationComposer,
          $$SyncQueueItemsTableCreateCompanionBuilder,
          $$SyncQueueItemsTableUpdateCompanionBuilder,
          (
            SyncQueueItem,
            BaseReferences<_$AppDatabase, $SyncQueueItemsTable, SyncQueueItem>,
          ),
          SyncQueueItem,
          PrefetchHooks Function()
        > {
  $$SyncQueueItemsTableTableManager(
    _$AppDatabase db,
    $SyncQueueItemsTable table,
  ) : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$SyncQueueItemsTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$SyncQueueItemsTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$SyncQueueItemsTableAnnotationComposer($db: db, $table: table),
          updateCompanionCallback:
              ({
                Value<String> operationId = const Value.absent(),
                Value<String> entityType = const Value.absent(),
                Value<String> entityId = const Value.absent(),
                Value<String> changeType = const Value.absent(),
                Value<String?> payloadJson = const Value.absent(),
                Value<String?> baseRowVersion = const Value.absent(),
                Value<String> status = const Value.absent(),
                Value<int> attemptCount = const Value.absent(),
                Value<DateTime?> nextAttemptAt = const Value.absent(),
                Value<String?> lastError = const Value.absent(),
                Value<DateTime> createdAt = const Value.absent(),
                Value<int> rowid = const Value.absent(),
              }) => SyncQueueItemsCompanion(
                operationId: operationId,
                entityType: entityType,
                entityId: entityId,
                changeType: changeType,
                payloadJson: payloadJson,
                baseRowVersion: baseRowVersion,
                status: status,
                attemptCount: attemptCount,
                nextAttemptAt: nextAttemptAt,
                lastError: lastError,
                createdAt: createdAt,
                rowid: rowid,
              ),
          createCompanionCallback:
              ({
                required String operationId,
                Value<String> entityType = const Value.absent(),
                required String entityId,
                required String changeType,
                Value<String?> payloadJson = const Value.absent(),
                Value<String?> baseRowVersion = const Value.absent(),
                Value<String> status = const Value.absent(),
                Value<int> attemptCount = const Value.absent(),
                Value<DateTime?> nextAttemptAt = const Value.absent(),
                Value<String?> lastError = const Value.absent(),
                required DateTime createdAt,
                Value<int> rowid = const Value.absent(),
              }) => SyncQueueItemsCompanion.insert(
                operationId: operationId,
                entityType: entityType,
                entityId: entityId,
                changeType: changeType,
                payloadJson: payloadJson,
                baseRowVersion: baseRowVersion,
                status: status,
                attemptCount: attemptCount,
                nextAttemptAt: nextAttemptAt,
                lastError: lastError,
                createdAt: createdAt,
                rowid: rowid,
              ),
          withReferenceMapper: (p0) => p0
              .map(
                (e) => (
                  e.readTable<$SyncQueueItemsTable, SyncQueueItem>(table),
                  BaseReferences<
                    _$AppDatabase,
                    $SyncQueueItemsTable,
                    SyncQueueItem
                  >(db, table, e),
                ),
              )
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$SyncQueueItemsTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $SyncQueueItemsTable,
      SyncQueueItem,
      $$SyncQueueItemsTableFilterComposer,
      $$SyncQueueItemsTableOrderingComposer,
      $$SyncQueueItemsTableAnnotationComposer,
      $$SyncQueueItemsTableCreateCompanionBuilder,
      $$SyncQueueItemsTableUpdateCompanionBuilder,
      (
        SyncQueueItem,
        BaseReferences<_$AppDatabase, $SyncQueueItemsTable, SyncQueueItem>,
      ),
      SyncQueueItem,
      PrefetchHooks Function()
    >;
typedef $$ConflictItemsTableCreateCompanionBuilder =
    ConflictItemsCompanion Function({
      Value<int> id,
      Value<String> entityType,
      required String entityId,
      required String localPayloadJson,
      required String serverPayloadJson,
      required DateTime createdAt,
    });
typedef $$ConflictItemsTableUpdateCompanionBuilder =
    ConflictItemsCompanion Function({
      Value<int> id,
      Value<String> entityType,
      Value<String> entityId,
      Value<String> localPayloadJson,
      Value<String> serverPayloadJson,
      Value<DateTime> createdAt,
    });

class $$ConflictItemsTableFilterComposer
    extends Composer<_$AppDatabase, $ConflictItemsTable> {
  $$ConflictItemsTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<int> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get entityType => $composableBuilder(
    column: $table.entityType,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get entityId => $composableBuilder(
    column: $table.entityId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get localPayloadJson => $composableBuilder(
    column: $table.localPayloadJson,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get serverPayloadJson => $composableBuilder(
    column: $table.serverPayloadJson,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get createdAt => $composableBuilder(
    column: $table.createdAt,
    builder: (column) => ColumnFilters(column),
  );
}

class $$ConflictItemsTableOrderingComposer
    extends Composer<_$AppDatabase, $ConflictItemsTable> {
  $$ConflictItemsTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<int> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get entityType => $composableBuilder(
    column: $table.entityType,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get entityId => $composableBuilder(
    column: $table.entityId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get localPayloadJson => $composableBuilder(
    column: $table.localPayloadJson,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get serverPayloadJson => $composableBuilder(
    column: $table.serverPayloadJson,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get createdAt => $composableBuilder(
    column: $table.createdAt,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$ConflictItemsTableAnnotationComposer
    extends Composer<_$AppDatabase, $ConflictItemsTable> {
  $$ConflictItemsTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<int> get id =>
      $composableBuilder(column: $table.id, builder: (column) => column);

  GeneratedColumn<String> get entityType => $composableBuilder(
    column: $table.entityType,
    builder: (column) => column,
  );

  GeneratedColumn<String> get entityId =>
      $composableBuilder(column: $table.entityId, builder: (column) => column);

  GeneratedColumn<String> get localPayloadJson => $composableBuilder(
    column: $table.localPayloadJson,
    builder: (column) => column,
  );

  GeneratedColumn<String> get serverPayloadJson => $composableBuilder(
    column: $table.serverPayloadJson,
    builder: (column) => column,
  );

  GeneratedColumn<DateTime> get createdAt =>
      $composableBuilder(column: $table.createdAt, builder: (column) => column);
}

class $$ConflictItemsTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $ConflictItemsTable,
          ConflictItem,
          $$ConflictItemsTableFilterComposer,
          $$ConflictItemsTableOrderingComposer,
          $$ConflictItemsTableAnnotationComposer,
          $$ConflictItemsTableCreateCompanionBuilder,
          $$ConflictItemsTableUpdateCompanionBuilder,
          (
            ConflictItem,
            BaseReferences<_$AppDatabase, $ConflictItemsTable, ConflictItem>,
          ),
          ConflictItem,
          PrefetchHooks Function()
        > {
  $$ConflictItemsTableTableManager(_$AppDatabase db, $ConflictItemsTable table)
    : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$ConflictItemsTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$ConflictItemsTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$ConflictItemsTableAnnotationComposer($db: db, $table: table),
          updateCompanionCallback:
              ({
                Value<int> id = const Value.absent(),
                Value<String> entityType = const Value.absent(),
                Value<String> entityId = const Value.absent(),
                Value<String> localPayloadJson = const Value.absent(),
                Value<String> serverPayloadJson = const Value.absent(),
                Value<DateTime> createdAt = const Value.absent(),
              }) => ConflictItemsCompanion(
                id: id,
                entityType: entityType,
                entityId: entityId,
                localPayloadJson: localPayloadJson,
                serverPayloadJson: serverPayloadJson,
                createdAt: createdAt,
              ),
          createCompanionCallback:
              ({
                Value<int> id = const Value.absent(),
                Value<String> entityType = const Value.absent(),
                required String entityId,
                required String localPayloadJson,
                required String serverPayloadJson,
                required DateTime createdAt,
              }) => ConflictItemsCompanion.insert(
                id: id,
                entityType: entityType,
                entityId: entityId,
                localPayloadJson: localPayloadJson,
                serverPayloadJson: serverPayloadJson,
                createdAt: createdAt,
              ),
          withReferenceMapper: (p0) => p0
              .map(
                (e) => (
                  e.readTable<$ConflictItemsTable, ConflictItem>(table),
                  BaseReferences<
                    _$AppDatabase,
                    $ConflictItemsTable,
                    ConflictItem
                  >(db, table, e),
                ),
              )
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$ConflictItemsTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $ConflictItemsTable,
      ConflictItem,
      $$ConflictItemsTableFilterComposer,
      $$ConflictItemsTableOrderingComposer,
      $$ConflictItemsTableAnnotationComposer,
      $$ConflictItemsTableCreateCompanionBuilder,
      $$ConflictItemsTableUpdateCompanionBuilder,
      (
        ConflictItem,
        BaseReferences<_$AppDatabase, $ConflictItemsTable, ConflictItem>,
      ),
      ConflictItem,
      PrefetchHooks Function()
    >;
typedef $$LocalProjectsTableCreateCompanionBuilder =
    LocalProjectsCompanion Function({
      required String id,
      required String key,
      required String name,
      Value<String?> description,
      Value<String> status,
      Value<String> priority,
      Value<bool> isArchived,
      Value<int> rowid,
    });
typedef $$LocalProjectsTableUpdateCompanionBuilder =
    LocalProjectsCompanion Function({
      Value<String> id,
      Value<String> key,
      Value<String> name,
      Value<String?> description,
      Value<String> status,
      Value<String> priority,
      Value<bool> isArchived,
      Value<int> rowid,
    });

class $$LocalProjectsTableFilterComposer
    extends Composer<_$AppDatabase, $LocalProjectsTable> {
  $$LocalProjectsTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<String> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get key => $composableBuilder(
    column: $table.key,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get name => $composableBuilder(
    column: $table.name,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get description => $composableBuilder(
    column: $table.description,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get status => $composableBuilder(
    column: $table.status,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get priority => $composableBuilder(
    column: $table.priority,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<bool> get isArchived => $composableBuilder(
    column: $table.isArchived,
    builder: (column) => ColumnFilters(column),
  );
}

class $$LocalProjectsTableOrderingComposer
    extends Composer<_$AppDatabase, $LocalProjectsTable> {
  $$LocalProjectsTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<String> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get key => $composableBuilder(
    column: $table.key,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get name => $composableBuilder(
    column: $table.name,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get description => $composableBuilder(
    column: $table.description,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get status => $composableBuilder(
    column: $table.status,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get priority => $composableBuilder(
    column: $table.priority,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<bool> get isArchived => $composableBuilder(
    column: $table.isArchived,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$LocalProjectsTableAnnotationComposer
    extends Composer<_$AppDatabase, $LocalProjectsTable> {
  $$LocalProjectsTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<String> get id =>
      $composableBuilder(column: $table.id, builder: (column) => column);

  GeneratedColumn<String> get key =>
      $composableBuilder(column: $table.key, builder: (column) => column);

  GeneratedColumn<String> get name =>
      $composableBuilder(column: $table.name, builder: (column) => column);

  GeneratedColumn<String> get description => $composableBuilder(
    column: $table.description,
    builder: (column) => column,
  );

  GeneratedColumn<String> get status =>
      $composableBuilder(column: $table.status, builder: (column) => column);

  GeneratedColumn<String> get priority =>
      $composableBuilder(column: $table.priority, builder: (column) => column);

  GeneratedColumn<bool> get isArchived => $composableBuilder(
    column: $table.isArchived,
    builder: (column) => column,
  );
}

class $$LocalProjectsTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $LocalProjectsTable,
          LocalProject,
          $$LocalProjectsTableFilterComposer,
          $$LocalProjectsTableOrderingComposer,
          $$LocalProjectsTableAnnotationComposer,
          $$LocalProjectsTableCreateCompanionBuilder,
          $$LocalProjectsTableUpdateCompanionBuilder,
          (
            LocalProject,
            BaseReferences<_$AppDatabase, $LocalProjectsTable, LocalProject>,
          ),
          LocalProject,
          PrefetchHooks Function()
        > {
  $$LocalProjectsTableTableManager(_$AppDatabase db, $LocalProjectsTable table)
    : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$LocalProjectsTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$LocalProjectsTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$LocalProjectsTableAnnotationComposer($db: db, $table: table),
          updateCompanionCallback:
              ({
                Value<String> id = const Value.absent(),
                Value<String> key = const Value.absent(),
                Value<String> name = const Value.absent(),
                Value<String?> description = const Value.absent(),
                Value<String> status = const Value.absent(),
                Value<String> priority = const Value.absent(),
                Value<bool> isArchived = const Value.absent(),
                Value<int> rowid = const Value.absent(),
              }) => LocalProjectsCompanion(
                id: id,
                key: key,
                name: name,
                description: description,
                status: status,
                priority: priority,
                isArchived: isArchived,
                rowid: rowid,
              ),
          createCompanionCallback:
              ({
                required String id,
                required String key,
                required String name,
                Value<String?> description = const Value.absent(),
                Value<String> status = const Value.absent(),
                Value<String> priority = const Value.absent(),
                Value<bool> isArchived = const Value.absent(),
                Value<int> rowid = const Value.absent(),
              }) => LocalProjectsCompanion.insert(
                id: id,
                key: key,
                name: name,
                description: description,
                status: status,
                priority: priority,
                isArchived: isArchived,
                rowid: rowid,
              ),
          withReferenceMapper: (p0) => p0
              .map(
                (e) => (
                  e.readTable<$LocalProjectsTable, LocalProject>(table),
                  BaseReferences<
                    _$AppDatabase,
                    $LocalProjectsTable,
                    LocalProject
                  >(db, table, e),
                ),
              )
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$LocalProjectsTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $LocalProjectsTable,
      LocalProject,
      $$LocalProjectsTableFilterComposer,
      $$LocalProjectsTableOrderingComposer,
      $$LocalProjectsTableAnnotationComposer,
      $$LocalProjectsTableCreateCompanionBuilder,
      $$LocalProjectsTableUpdateCompanionBuilder,
      (
        LocalProject,
        BaseReferences<_$AppDatabase, $LocalProjectsTable, LocalProject>,
      ),
      LocalProject,
      PrefetchHooks Function()
    >;
typedef $$LocalBoardColumnsTableCreateCompanionBuilder =
    LocalBoardColumnsCompanion Function({
      required String id,
      required String projectId,
      required String name,
      Value<String> color,
      Value<int> order,
      Value<int?> wipLimit,
      Value<String> statusMapping,
      Value<bool> isCompletionColumn,
      Value<int> rowid,
    });
typedef $$LocalBoardColumnsTableUpdateCompanionBuilder =
    LocalBoardColumnsCompanion Function({
      Value<String> id,
      Value<String> projectId,
      Value<String> name,
      Value<String> color,
      Value<int> order,
      Value<int?> wipLimit,
      Value<String> statusMapping,
      Value<bool> isCompletionColumn,
      Value<int> rowid,
    });

class $$LocalBoardColumnsTableFilterComposer
    extends Composer<_$AppDatabase, $LocalBoardColumnsTable> {
  $$LocalBoardColumnsTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<String> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get projectId => $composableBuilder(
    column: $table.projectId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get name => $composableBuilder(
    column: $table.name,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get color => $composableBuilder(
    column: $table.color,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<int> get order => $composableBuilder(
    column: $table.order,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<int> get wipLimit => $composableBuilder(
    column: $table.wipLimit,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get statusMapping => $composableBuilder(
    column: $table.statusMapping,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<bool> get isCompletionColumn => $composableBuilder(
    column: $table.isCompletionColumn,
    builder: (column) => ColumnFilters(column),
  );
}

class $$LocalBoardColumnsTableOrderingComposer
    extends Composer<_$AppDatabase, $LocalBoardColumnsTable> {
  $$LocalBoardColumnsTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<String> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get projectId => $composableBuilder(
    column: $table.projectId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get name => $composableBuilder(
    column: $table.name,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get color => $composableBuilder(
    column: $table.color,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<int> get order => $composableBuilder(
    column: $table.order,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<int> get wipLimit => $composableBuilder(
    column: $table.wipLimit,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get statusMapping => $composableBuilder(
    column: $table.statusMapping,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<bool> get isCompletionColumn => $composableBuilder(
    column: $table.isCompletionColumn,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$LocalBoardColumnsTableAnnotationComposer
    extends Composer<_$AppDatabase, $LocalBoardColumnsTable> {
  $$LocalBoardColumnsTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<String> get id =>
      $composableBuilder(column: $table.id, builder: (column) => column);

  GeneratedColumn<String> get projectId =>
      $composableBuilder(column: $table.projectId, builder: (column) => column);

  GeneratedColumn<String> get name =>
      $composableBuilder(column: $table.name, builder: (column) => column);

  GeneratedColumn<String> get color =>
      $composableBuilder(column: $table.color, builder: (column) => column);

  GeneratedColumn<int> get order =>
      $composableBuilder(column: $table.order, builder: (column) => column);

  GeneratedColumn<int> get wipLimit =>
      $composableBuilder(column: $table.wipLimit, builder: (column) => column);

  GeneratedColumn<String> get statusMapping => $composableBuilder(
    column: $table.statusMapping,
    builder: (column) => column,
  );

  GeneratedColumn<bool> get isCompletionColumn => $composableBuilder(
    column: $table.isCompletionColumn,
    builder: (column) => column,
  );
}

class $$LocalBoardColumnsTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $LocalBoardColumnsTable,
          LocalBoardColumn,
          $$LocalBoardColumnsTableFilterComposer,
          $$LocalBoardColumnsTableOrderingComposer,
          $$LocalBoardColumnsTableAnnotationComposer,
          $$LocalBoardColumnsTableCreateCompanionBuilder,
          $$LocalBoardColumnsTableUpdateCompanionBuilder,
          (
            LocalBoardColumn,
            BaseReferences<
              _$AppDatabase,
              $LocalBoardColumnsTable,
              LocalBoardColumn
            >,
          ),
          LocalBoardColumn,
          PrefetchHooks Function()
        > {
  $$LocalBoardColumnsTableTableManager(
    _$AppDatabase db,
    $LocalBoardColumnsTable table,
  ) : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$LocalBoardColumnsTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$LocalBoardColumnsTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$LocalBoardColumnsTableAnnotationComposer(
                $db: db,
                $table: table,
              ),
          updateCompanionCallback:
              ({
                Value<String> id = const Value.absent(),
                Value<String> projectId = const Value.absent(),
                Value<String> name = const Value.absent(),
                Value<String> color = const Value.absent(),
                Value<int> order = const Value.absent(),
                Value<int?> wipLimit = const Value.absent(),
                Value<String> statusMapping = const Value.absent(),
                Value<bool> isCompletionColumn = const Value.absent(),
                Value<int> rowid = const Value.absent(),
              }) => LocalBoardColumnsCompanion(
                id: id,
                projectId: projectId,
                name: name,
                color: color,
                order: order,
                wipLimit: wipLimit,
                statusMapping: statusMapping,
                isCompletionColumn: isCompletionColumn,
                rowid: rowid,
              ),
          createCompanionCallback:
              ({
                required String id,
                required String projectId,
                required String name,
                Value<String> color = const Value.absent(),
                Value<int> order = const Value.absent(),
                Value<int?> wipLimit = const Value.absent(),
                Value<String> statusMapping = const Value.absent(),
                Value<bool> isCompletionColumn = const Value.absent(),
                Value<int> rowid = const Value.absent(),
              }) => LocalBoardColumnsCompanion.insert(
                id: id,
                projectId: projectId,
                name: name,
                color: color,
                order: order,
                wipLimit: wipLimit,
                statusMapping: statusMapping,
                isCompletionColumn: isCompletionColumn,
                rowid: rowid,
              ),
          withReferenceMapper: (p0) => p0
              .map(
                (e) => (
                  e.readTable<$LocalBoardColumnsTable, LocalBoardColumn>(table),
                  BaseReferences<
                    _$AppDatabase,
                    $LocalBoardColumnsTable,
                    LocalBoardColumn
                  >(db, table, e),
                ),
              )
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$LocalBoardColumnsTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $LocalBoardColumnsTable,
      LocalBoardColumn,
      $$LocalBoardColumnsTableFilterComposer,
      $$LocalBoardColumnsTableOrderingComposer,
      $$LocalBoardColumnsTableAnnotationComposer,
      $$LocalBoardColumnsTableCreateCompanionBuilder,
      $$LocalBoardColumnsTableUpdateCompanionBuilder,
      (
        LocalBoardColumn,
        BaseReferences<
          _$AppDatabase,
          $LocalBoardColumnsTable,
          LocalBoardColumn
        >,
      ),
      LocalBoardColumn,
      PrefetchHooks Function()
    >;
typedef $$LocalUsersTableCreateCompanionBuilder = LocalUsersCompanion Function({
  required String id,
  Value<String> email,
  Value<String> firstName,
  Value<String> lastName,
  Value<int> rowid,
});
typedef $$LocalUsersTableUpdateCompanionBuilder = LocalUsersCompanion Function({
  Value<String> id,
  Value<String> email,
  Value<String> firstName,
  Value<String> lastName,
  Value<int> rowid,
});

class $$LocalUsersTableFilterComposer
    extends Composer<_$AppDatabase, $LocalUsersTable> {
  $$LocalUsersTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<String> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get email => $composableBuilder(
    column: $table.email,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get firstName => $composableBuilder(
    column: $table.firstName,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get lastName => $composableBuilder(
    column: $table.lastName,
    builder: (column) => ColumnFilters(column),
  );
}

class $$LocalUsersTableOrderingComposer
    extends Composer<_$AppDatabase, $LocalUsersTable> {
  $$LocalUsersTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<String> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get email => $composableBuilder(
    column: $table.email,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get firstName => $composableBuilder(
    column: $table.firstName,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get lastName => $composableBuilder(
    column: $table.lastName,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$LocalUsersTableAnnotationComposer
    extends Composer<_$AppDatabase, $LocalUsersTable> {
  $$LocalUsersTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<String> get id =>
      $composableBuilder(column: $table.id, builder: (column) => column);

  GeneratedColumn<String> get email =>
      $composableBuilder(column: $table.email, builder: (column) => column);

  GeneratedColumn<String> get firstName =>
      $composableBuilder(column: $table.firstName, builder: (column) => column);

  GeneratedColumn<String> get lastName =>
      $composableBuilder(column: $table.lastName, builder: (column) => column);
}

class $$LocalUsersTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $LocalUsersTable,
          LocalUser,
          $$LocalUsersTableFilterComposer,
          $$LocalUsersTableOrderingComposer,
          $$LocalUsersTableAnnotationComposer,
          $$LocalUsersTableCreateCompanionBuilder,
          $$LocalUsersTableUpdateCompanionBuilder,
          (
            LocalUser,
            BaseReferences<_$AppDatabase, $LocalUsersTable, LocalUser>,
          ),
          LocalUser,
          PrefetchHooks Function()
        > {
  $$LocalUsersTableTableManager(_$AppDatabase db, $LocalUsersTable table)
    : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$LocalUsersTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$LocalUsersTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$LocalUsersTableAnnotationComposer($db: db, $table: table),
          updateCompanionCallback:
              ({
                Value<String> id = const Value.absent(),
                Value<String> email = const Value.absent(),
                Value<String> firstName = const Value.absent(),
                Value<String> lastName = const Value.absent(),
                Value<int> rowid = const Value.absent(),
              }) => LocalUsersCompanion(
                id: id,
                email: email,
                firstName: firstName,
                lastName: lastName,
                rowid: rowid,
              ),
          createCompanionCallback:
              ({
                required String id,
                Value<String> email = const Value.absent(),
                Value<String> firstName = const Value.absent(),
                Value<String> lastName = const Value.absent(),
                Value<int> rowid = const Value.absent(),
              }) => LocalUsersCompanion.insert(
                id: id,
                email: email,
                firstName: firstName,
                lastName: lastName,
                rowid: rowid,
              ),
          withReferenceMapper: (p0) => p0
              .map(
                (e) => (
                  e.readTable<$LocalUsersTable, LocalUser>(table),
                  BaseReferences<_$AppDatabase, $LocalUsersTable, LocalUser>(
                    db,
                    table,
                    e,
                  ),
                ),
              )
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$LocalUsersTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $LocalUsersTable,
      LocalUser,
      $$LocalUsersTableFilterComposer,
      $$LocalUsersTableOrderingComposer,
      $$LocalUsersTableAnnotationComposer,
      $$LocalUsersTableCreateCompanionBuilder,
      $$LocalUsersTableUpdateCompanionBuilder,
      (LocalUser, BaseReferences<_$AppDatabase, $LocalUsersTable, LocalUser>),
      LocalUser,
      PrefetchHooks Function()
    >;
typedef $$SyncMetaTableCreateCompanionBuilder = SyncMetaCompanion Function({
  required String key,
  required String value,
  Value<int> rowid,
});
typedef $$SyncMetaTableUpdateCompanionBuilder = SyncMetaCompanion Function({
  Value<String> key,
  Value<String> value,
  Value<int> rowid,
});

class $$SyncMetaTableFilterComposer
    extends Composer<_$AppDatabase, $SyncMetaTable> {
  $$SyncMetaTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<String> get key => $composableBuilder(
    column: $table.key,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get value => $composableBuilder(
    column: $table.value,
    builder: (column) => ColumnFilters(column),
  );
}

class $$SyncMetaTableOrderingComposer
    extends Composer<_$AppDatabase, $SyncMetaTable> {
  $$SyncMetaTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<String> get key => $composableBuilder(
    column: $table.key,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get value => $composableBuilder(
    column: $table.value,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$SyncMetaTableAnnotationComposer
    extends Composer<_$AppDatabase, $SyncMetaTable> {
  $$SyncMetaTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<String> get key =>
      $composableBuilder(column: $table.key, builder: (column) => column);

  GeneratedColumn<String> get value =>
      $composableBuilder(column: $table.value, builder: (column) => column);
}

class $$SyncMetaTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $SyncMetaTable,
          SyncMetaData,
          $$SyncMetaTableFilterComposer,
          $$SyncMetaTableOrderingComposer,
          $$SyncMetaTableAnnotationComposer,
          $$SyncMetaTableCreateCompanionBuilder,
          $$SyncMetaTableUpdateCompanionBuilder,
          (
            SyncMetaData,
            BaseReferences<_$AppDatabase, $SyncMetaTable, SyncMetaData>,
          ),
          SyncMetaData,
          PrefetchHooks Function()
        > {
  $$SyncMetaTableTableManager(_$AppDatabase db, $SyncMetaTable table)
    : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$SyncMetaTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$SyncMetaTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$SyncMetaTableAnnotationComposer($db: db, $table: table),
          updateCompanionCallback: ({
            Value<String> key = const Value.absent(),
            Value<String> value = const Value.absent(),
            Value<int> rowid = const Value.absent(),
          }) => SyncMetaCompanion(key: key, value: value, rowid: rowid),
          createCompanionCallback: ({
            required String key,
            required String value,
            Value<int> rowid = const Value.absent(),
          }) => SyncMetaCompanion.insert(key: key, value: value, rowid: rowid),
          withReferenceMapper: (p0) => p0
              .map(
                (e) => (
                  e.readTable<$SyncMetaTable, SyncMetaData>(table),
                  BaseReferences<_$AppDatabase, $SyncMetaTable, SyncMetaData>(
                    db,
                    table,
                    e,
                  ),
                ),
              )
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$SyncMetaTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $SyncMetaTable,
      SyncMetaData,
      $$SyncMetaTableFilterComposer,
      $$SyncMetaTableOrderingComposer,
      $$SyncMetaTableAnnotationComposer,
      $$SyncMetaTableCreateCompanionBuilder,
      $$SyncMetaTableUpdateCompanionBuilder,
      (
        SyncMetaData,
        BaseReferences<_$AppDatabase, $SyncMetaTable, SyncMetaData>,
      ),
      SyncMetaData,
      PrefetchHooks Function()
    >;

class $AppDatabaseManager {
  final _$AppDatabase _db;
  $AppDatabaseManager(this._db);
  $$LocalTasksTableTableManager get localTasks =>
      $$LocalTasksTableTableManager(_db, _db.localTasks);
  $$SyncQueueItemsTableTableManager get syncQueueItems =>
      $$SyncQueueItemsTableTableManager(_db, _db.syncQueueItems);
  $$ConflictItemsTableTableManager get conflictItems =>
      $$ConflictItemsTableTableManager(_db, _db.conflictItems);
  $$LocalProjectsTableTableManager get localProjects =>
      $$LocalProjectsTableTableManager(_db, _db.localProjects);
  $$LocalBoardColumnsTableTableManager get localBoardColumns =>
      $$LocalBoardColumnsTableTableManager(_db, _db.localBoardColumns);
  $$LocalUsersTableTableManager get localUsers =>
      $$LocalUsersTableTableManager(_db, _db.localUsers);
  $$SyncMetaTableTableManager get syncMeta =>
      $$SyncMetaTableTableManager(_db, _db.syncMeta);
}
