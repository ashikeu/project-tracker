// End-to-end check of the CLIENT code behind the newly-built screens (Sprints,
// Backlog, Comments, Attachments, Time tracking) against a live backend on
// http://localhost:5250. Drives SprintRemote / BacklogRemote / CommentRemote /
// AttachmentRemote / TimeEntryRemote exactly as the screens use them. Skips
// automatically if the backend isn't reachable, so it never breaks offline CI.
//
// Run with the API up:  flutter test test/live_deferred_screens_test.dart
import 'dart:convert';
import 'dart:io';

import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:intl/intl.dart';
import 'package:project_tracker/core/constants/api_constants.dart';
import 'package:project_tracker/data/datasources/remote/attachment_remote.dart';
import 'package:project_tracker/data/datasources/remote/auth_remote.dart';
import 'package:project_tracker/data/datasources/remote/backlog_remote.dart';
import 'package:project_tracker/data/datasources/remote/comment_remote.dart';
import 'package:project_tracker/data/datasources/remote/project_remote.dart';
import 'package:project_tracker/data/datasources/remote/sprint_remote.dart';
import 'package:project_tracker/data/datasources/remote/sync_remote.dart';
import 'package:project_tracker/data/datasources/remote/time_entry_remote.dart';
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
  const uuid = Uuid();

  test('deferred screens: sprints, backlog, comments, attachments, time', () async {
    if (!await _backendUp()) {
      markTestSkipped('Backend not reachable on ${ApiConstants.baseHost} — skipping.');
      return;
    }

    final dio = Dio(BaseOptions(
      baseUrl: ApiConstants.baseUrl,
      validateStatus: (code) => code != null && code < 500,
    ));
    final auth = AuthRemote(dio);
    final projects = ProjectRemote(dio);
    final sync = SyncRemote(dio);
    final sprintRemote = SprintRemote(dio);
    final backlogRemote = BacklogRemote(dio);
    final commentRemote = CommentRemote(dio);
    final attachmentRemote = AttachmentRemote(dio);
    final timeRemote = TimeEntryRemote(dio);

    // --- Login + project + a server-backed task to hang everything off of.
    final login = await auth.login('pm@acme.test', 'Passw0rd!');
    dio.options.headers['Authorization'] = 'Bearer ${login.accessToken}';

    final key = 'D${DateTime.now().millisecondsSinceEpoch % 100000}';
    final project = await projects.createProject(key: key, name: 'Deferred Test $key');
    final columns = await projects.getBoardColumns(project.id);

    final taskId = uuid.v4();
    final created = (await sync.push([
      SyncOperationInput(
        operationId: uuid.v4(),
        entityId: taskId,
        changeType: 'created',
        payloadJson: jsonEncode(TaskPayload(
          id: taskId,
          projectId: project.id,
          title: 'Deferred-screens task',
          status: 1,
          priority: 3,
          boardColumnId: columns.first.id,
          position: 1024,
        ).toJson()),
      ),
    ]))
        .single;
    expect(created.status, 'Applied', reason: created.message);

    // --- SPRINTS: create → list → start → complete.
    final sprint = await sprintRemote.create(
      projectId: project.id,
      name: 'Sprint 1',
      goal: 'Ship the thing',
      startDate: DateFormat('yyyy-MM-dd').format(DateTime.now()),
      endDate: DateFormat('yyyy-MM-dd').format(DateTime.now().add(const Duration(days: 14))),
    );
    expect(sprint.status, 'Planning');

    var list = await sprintRemote.list(project.id);
    expect(list.map((s) => s.id), contains(sprint.id));

    final started = await sprintRemote.start(sprint.id);
    expect(started.status, 'Active');

    // --- BACKLOG: our task is top-level with no sprint → it's in the backlog.
    var backlog = await backlogRemote.list(project.id);
    expect(backlog.map((t) => t.id), contains(taskId), reason: 'new task should be in the backlog');

    // Reorder (single item, index 0) must succeed.
    await backlogRemote.reorder(taskId, 0);

    // Assign to the active sprint → it leaves the backlog.
    await backlogRemote.assignToSprint(taskId, sprint.id);
    backlog = await backlogRemote.list(project.id);
    expect(backlog.map((t) => t.id), isNot(contains(taskId)),
        reason: 'task assigned to a sprint should leave the backlog');

    // Send it back to the backlog.
    await backlogRemote.assignToSprint(taskId, null);
    backlog = await backlogRemote.list(project.id);
    expect(backlog.map((t) => t.id), contains(taskId));

    // Complete the (now empty) sprint.
    final completed = await sprintRemote.complete(sprint.id);
    expect(completed.status, 'Completed');

    // --- COMMENTS: create → edit → reply → list (nested) → delete.
    final c1 = await commentRemote.create(taskId, 'First comment');
    expect(c1.body, 'First comment');

    final edited = await commentRemote.update(c1.id, 'First comment (edited)');
    expect(edited.isEdited, isTrue);
    expect(edited.body, 'First comment (edited)');

    final reply = await commentRemote.create(taskId, 'A reply', parentCommentId: c1.id);
    expect(reply.parentCommentId, c1.id);

    final threads = await commentRemote.list(taskId);
    final top = threads.firstWhere((c) => c.id == c1.id);
    expect(top.replies.map((r) => r.id), contains(reply.id), reason: 'reply nests under its parent');

    await commentRemote.delete(reply.id);
    await commentRemote.delete(c1.id);

    // --- ATTACHMENTS: upload a temp file → list → download (bytes match) → delete.
    final tmp = File('${Directory.systemTemp.path}/pt_live_${uuid.v4()}.txt');
    const fileBody = 'hello from the live attachment test';
    await tmp.writeAsString(fileBody);
    final uploaded = await attachmentRemote.upload(taskId,
        filePath: tmp.path, fileName: 'note.txt');
    expect(uploaded.originalFileName, 'note.txt');

    final attachments = await attachmentRemote.list(taskId);
    expect(attachments.map((a) => a.id), contains(uploaded.id));

    final downloaded = await attachmentRemote.download(uploaded);
    expect(utf8.decode(downloaded.bytes), fileBody, reason: 'downloaded bytes round-trip');

    await attachmentRemote.delete(uploaded.id);
    expect((await attachmentRemote.list(taskId)).map((a) => a.id), isNot(contains(uploaded.id)));
    await tmp.delete();

    // --- TIME: manual log → summary → start/stop timer → delete.
    final entry = await timeRemote.create(taskId,
        date: DateFormat('yyyy-MM-dd').format(DateTime.now()),
        durationMinutes: 90,
        description: 'manual log');
    expect(entry.durationMinutes, 90);

    final summary = await timeRemote.summary(taskId);
    expect(summary.loggedHours, closeTo(1.5, 0.001));

    final running = await timeRemote.startTimer(taskId);
    expect(running.isRunningTimer, isTrue);
    final stopped = await timeRemote.stopTimer();
    expect(stopped.isRunningTimer, isFalse);

    await timeRemote.delete(entry.id);
    final summaryAfter = await timeRemote.summary(taskId);
    // The manual 90m is gone; only the (tiny) stopped-timer entry remains.
    expect(summaryAfter.loggedHours, lessThan(1.5));
  }, timeout: const Timeout(Duration(seconds: 90)));
}
