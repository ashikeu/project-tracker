// End-to-end check of the CLIENT's real networking + sync code against a live
// backend on http://localhost:5250. Exercises AuthRemote, ProjectRemote and
// SyncRemote exactly as the app uses them. Skips automatically if the backend
// isn't reachable, so it never breaks offline CI.
//
// Run with the API up:  flutter test test/live_api_test.dart
import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:project_tracker/core/constants/api_constants.dart';
import 'package:project_tracker/data/datasources/remote/auth_remote.dart';
import 'package:project_tracker/data/datasources/remote/project_remote.dart';
import 'package:project_tracker/data/datasources/remote/sync_remote.dart';
import 'package:project_tracker/data/models/sync_models.dart';
import 'package:project_tracker/data/models/task_payload.dart';
import 'package:uuid/uuid.dart';

Future<bool> _backendUp() async {
  try {
    final dio = Dio(BaseOptions(validateStatus: (_) => true));
    final resp = await dio.get('${ApiConstants.baseHost}/health',
        options: Options(receiveTimeout: const Duration(seconds: 3)));
    return resp.statusCode == 200;
  } catch (_) {
    return false;
  }
}

void main() {
  late Dio dio;
  late AuthRemote auth;
  late ProjectRemote projects;
  late SyncRemote sync;
  const uuid = Uuid();

  setUp(() {
    dio = Dio(BaseOptions(
      baseUrl: ApiConstants.baseUrl,
      validateStatus: (code) => code != null && code < 500,
    ));
    auth = AuthRemote(dio);
    projects = ProjectRemote(dio);
    sync = SyncRemote(dio);
  });

  test('offline-first round-trip against the live API', () async {
    if (!await _backendUp()) {
      markTestSkipped('Backend not reachable on ${ApiConstants.baseHost} — skipping.');
      return;
    }

    // 1) Login with the dev-seeded PM, then attach the bearer token.
    final result = await auth.login('pm@acme.test', 'Passw0rd!');
    expect(result.accessToken, isNotEmpty);
    expect(result.primaryOrg, isNotNull);
    dio.options.headers['Authorization'] = 'Bearer ${result.accessToken}';

    // 2) Create a project (PM can) so we have somewhere to put a task.
    final key = 'E${DateTime.now().millisecondsSinceEpoch % 100000}';
    final project = await projects.createProject(key: key, name: 'Live Test $key');
    expect(project.id, isNotEmpty);

    // Board columns auto-seed on project creation.
    final columns = await projects.getBoardColumns(project.id);
    expect(columns, isNotEmpty);

    // 3) Push an OFFLINE-created task (client-generated Guid, no server round-trip
    //    needed for the id). Expect Applied + a server-assigned TaskKey.
    final taskId = uuid.v4();
    final createOpId = uuid.v4();
    final payload = TaskPayload(
      id: taskId,
      projectId: project.id,
      title: 'Offline-created task',
      status: columns.first.statusMapping.isEmpty ? 1 : 1,
      priority: 3,
      boardColumnId: columns.first.id,
      position: 1024,
    );
    final createResults = await sync.push([
      SyncOperationInput(
        operationId: createOpId,
        entityId: taskId,
        changeType: 'created',
        payloadJson: jsonEncode(payload.toJson()),
      ),
    ]);
    expect(createResults, hasLength(1));
    final created = createResults.single;
    expect(created.status, 'Applied', reason: created.message);
    expect(created.taskKey, isNotNull);
    expect(created.taskKey, startsWith(key));
    final serverRowVersion = created.rowVersion!;

    // 4) Idempotent replay: same OperationId returns the same result, no dup.
    final replay = await sync.push([
      SyncOperationInput(
        operationId: createOpId,
        entityId: taskId,
        changeType: 'created',
        payloadJson: jsonEncode(payload.toJson()),
      ),
    ]);
    expect(replay.single.status, 'Applied');
    expect(replay.single.taskKey, created.taskKey);

    // 5) Pull the delta from 0 — our new task must be in the feed.
    final pull = await sync.pull(since: 0, pageSize: 200);
    final found = pull.changes.where((c) => c.entityId == taskId).toList();
    expect(found, isNotEmpty);
    expect(found.first.isTombstone, isFalse);

    // 6) Update with the CORRECT rowversion → Applied.
    final okUpdate = await sync.push([
      SyncOperationInput(
        operationId: uuid.v4(),
        entityId: taskId,
        changeType: 'updated',
        baseRowVersion: serverRowVersion,
        payloadJson: jsonEncode((TaskPayload(
          id: taskId,
          projectId: project.id,
          title: 'Renamed online',
          status: 2,
          priority: 3,
          boardColumnId: columns.first.id,
          position: 1024,
          rowVersion: serverRowVersion,
        )).toJson()),
      ),
    ]);
    expect(okUpdate.single.status, 'Applied', reason: okUpdate.single.message);

    // 7) Update with a STALE rowversion → Conflict carrying the server payload.
    final staleUpdate = await sync.push([
      SyncOperationInput(
        operationId: uuid.v4(),
        entityId: taskId,
        changeType: 'updated',
        baseRowVersion: serverRowVersion, // now outdated after step 6
        payloadJson: jsonEncode((TaskPayload(
          id: taskId,
          projectId: project.id,
          title: 'Conflicting edit',
          status: 4,
          priority: 3,
          position: 1024,
          rowVersion: serverRowVersion,
        )).toJson()),
      ),
    ]);
    expect(staleUpdate.single.status, 'Conflict', reason: staleUpdate.single.message);
    expect(staleUpdate.single.serverPayloadJson, isNotNull);
    final serverCopy = TaskPayload.fromJson(
        jsonDecode(staleUpdate.single.serverPayloadJson!) as Map<String, dynamic>);
    expect(serverCopy.title, 'Renamed online'); // server kept the applied version
  }, timeout: const Timeout(Duration(seconds: 60)));
}
