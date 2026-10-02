import 'dart:convert';

import 'package:drift/drift.dart' hide isNull, isNotNull;
import 'package:drift/native.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:project_tracker/core/database/app_database.dart';
import 'package:project_tracker/data/models/task_payload.dart';
import 'package:project_tracker/data/repositories/task_repository.dart';

void main() {
  late AppDatabase db;
  late TaskRepository repo;

  setUp(() {
    db = AppDatabase.forTesting(NativeDatabase.memory());
    repo = TaskRepository(db);
  });

  tearDown(() => db.close());

  Future<List<SyncQueueItem>> pendingOps() =>
      (db.select(db.syncQueueItems)..where((q) => q.status.equals('pending'))).get();

  test('createTask writes a pendingCreate row and enqueues one created op', () async {
    final id = await repo.createTask(
      projectId: 'proj-1',
      title: 'Offline task',
      position: 1024,
    );

    final task = await repo.getTask(id);
    expect(task, isNotNull);
    expect(task!.syncState, 'pendingCreate');
    expect(task.taskKey, ''); // server assigns the key later
    expect(task.rowVersion, '');

    final ops = await pendingOps();
    expect(ops, hasLength(1));
    expect(ops.single.changeType, 'created');
    expect(ops.single.entityId, id);
  });

  test('updateTask on a synced task becomes pendingUpdate and replaces the op', () async {
    final id = await repo.createTask(projectId: 'p', title: 'T', position: 1024);
    // Simulate a successful sync: mark synced with a server rowversion.
    await (db.update(db.localTasks)..where((t) => t.id.equals(id))).write(
      const LocalTasksCompanion(syncState: Value('synced'), rowVersion: Value('AAA=')),
    );
    await (db.delete(db.syncQueueItems)).go(); // clear the create op

    await repo.updateTask(id, const LocalTasksCompanion(title: Value('Renamed')));

    final task = await repo.getTask(id);
    expect(task!.syncState, 'pendingUpdate');
    expect(task.title, 'Renamed');

    final ops = await pendingOps();
    expect(ops, hasLength(1));
    expect(ops.single.changeType, 'updated');
  });

  test('two quick edits coalesce to a single pending op (fresh OperationId each time)',
      () async {
    final id = await repo.createTask(projectId: 'p', title: 'T', position: 1024);
    await (db.update(db.localTasks)..where((t) => t.id.equals(id)))
        .write(const LocalTasksCompanion(syncState: Value('synced'), rowVersion: Value('AAA=')));
    await (db.delete(db.syncQueueItems)).go();

    await repo.updateTask(id, const LocalTasksCompanion(title: Value('v1')));
    final firstOp = (await pendingOps()).single.operationId;
    await repo.updateTask(id, const LocalTasksCompanion(title: Value('v2')));
    final ops = await pendingOps();

    expect(ops, hasLength(1)); // only one pending op survives
    expect(ops.single.operationId, isNot(firstOp)); // but it's a new id (not replayed)
  });

  test('deleting a never-synced task drops the row and its queued op', () async {
    final id = await repo.createTask(projectId: 'p', title: 'T', position: 1024);
    await repo.deleteTask(id);

    expect(await repo.getTask(id), isNull);
    expect(await pendingOps(), isEmpty);
  });

  test('deleting a synced task marks pendingDelete and enqueues a delete op', () async {
    final id = await repo.createTask(projectId: 'p', title: 'T', position: 1024);
    await (db.update(db.localTasks)..where((t) => t.id.equals(id)))
        .write(const LocalTasksCompanion(syncState: Value('synced'), rowVersion: Value('AAA=')));
    await (db.delete(db.syncQueueItems)).go();

    await repo.deleteTask(id);

    final task = await repo.getTask(id);
    expect(task!.isDeleted, true);
    expect(task.syncState, 'pendingDelete');
    final ops = await pendingOps();
    expect(ops.single.changeType, 'deleted');
  });

  test('payload round-trips through the wire shape and back into a row', () async {
    final id = await repo.createTask(
      projectId: 'p',
      title: 'Round trip',
      priority: 4,
      status: 2,
      storyPoints: 8,
      position: 2048,
    );
    final row = await repo.getTask(id);

    final json = TaskRepository.payloadJsonFromRow(row!);
    final decoded = TaskPayload.fromJson(jsonDecode(json) as Map<String, dynamic>);

    expect(decoded.title, 'Round trip');
    expect(decoded.priority, 4);
    expect(decoded.status, 2);
    expect(decoded.storyPoints, 8);
    expect(decoded.position, 2048);

    // Re-materialize as a server-sourced row.
    await db.into(db.localTasks).insertOnConflictUpdate(
          TaskRepository.companionFromPayload(
            TaskPayload(
              id: decoded.id,
              projectId: decoded.projectId,
              title: decoded.title,
              priority: decoded.priority,
              status: decoded.status,
              storyPoints: decoded.storyPoints,
              position: decoded.position,
              taskKey: 'ERP-1',
              rowVersion: 'BBB=',
            ),
          ),
        );
    final synced = await repo.getTask(id);
    expect(synced!.syncState, 'synced');
    expect(synced.taskKey, 'ERP-1');
    expect(synced.rowVersion, 'BBB=');
  });
}
