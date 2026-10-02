// Live checks for the Dashboard + Notifications client code against a running
// backend on http://localhost:5250. Auto-skips if the API isn't reachable.
import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:project_tracker/core/constants/api_constants.dart';
import 'package:project_tracker/data/datasources/remote/auth_remote.dart';
import 'package:project_tracker/data/datasources/remote/dashboard_remote.dart';
import 'package:project_tracker/data/datasources/remote/notification_remote.dart';
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
  const uuid = Uuid();

  setUp(() {
    dio = Dio(BaseOptions(
      baseUrl: ApiConstants.baseUrl,
      validateStatus: (code) => code != null && code < 500,
    ));
  });

  test('dashboard + notifications remotes work against the live API', () async {
    if (!await _backendUp()) {
      markTestSkipped('Backend not reachable — skipping.');
      return;
    }

    final auth = AuthRemote(dio);
    final projects = ProjectRemote(dio);
    final sync = SyncRemote(dio);
    final dashboards = DashboardRemote(dio);
    final notifications = NotificationRemote(dio);

    final login = await auth.login('pm@acme.test', 'Passw0rd!');
    dio.options.headers['Authorization'] = 'Bearer ${login.accessToken}';

    // A project with one task, so the dashboard has something to aggregate.
    final key = 'D${DateTime.now().millisecondsSinceEpoch % 100000}';
    final project = await projects.createProject(key: key, name: 'Dash $key');
    final columns = await projects.getBoardColumns(project.id);

    final taskId = uuid.v4();
    await sync.push([
      SyncOperationInput(
        operationId: uuid.v4(),
        entityId: taskId,
        changeType: 'created',
        payloadJson: jsonEncode(TaskPayload(
          id: taskId,
          projectId: project.id,
          title: 'Dashboard task',
          status: 1,
          priority: 3,
          storyPoints: 5,
          boardColumnId: columns.first.id,
          position: 1024,
        ).toJson()),
      ),
    ]);

    // Dashboard aggregation reflects the task.
    final dash = await dashboards.getDashboard(project.id);
    expect(dash.projectId, project.id);
    expect(dash.totalTasks, greaterThanOrEqualTo(1));
    expect(dash.byStatus, isNotEmpty);

    // Notifications endpoints are reachable and parse.
    final count = await notifications.unreadCount();
    expect(count, greaterThanOrEqualTo(0));
    final list = await notifications.list();
    expect(list, isA<List>());
  }, timeout: const Timeout(Duration(seconds: 60)));
}
