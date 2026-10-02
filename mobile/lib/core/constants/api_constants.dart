import 'dart:io' show Platform;

import 'package:flutter/foundation.dart';

/// Central place for the API host + route strings, so no widget hard-codes a URL.
class ApiConstants {
  ApiConstants._();

  /// The backend's HTTP dev profile is http://localhost:5250. On a real Android
  /// emulator `localhost` is the device itself, so the host loopback is 10.0.2.2.
  /// Everywhere else (web, Windows, iOS simulator) localhost is correct.
  static String get baseHost {
    const override = String.fromEnvironment('API_HOST');
    if (override.isNotEmpty) return override;
    if (!kIsWeb && Platform.isAndroid) return 'http://10.0.2.2:5250';
    return 'http://localhost:5250';
  }

  static String get baseUrl => '$baseHost/api/v1';

  // Auth
  static const String register = '/auth/register';
  static const String login = '/auth/login';
  static const String refreshToken = '/auth/refresh-token';
  static const String logout = '/auth/logout';

  // Projects
  static const String projects = '/projects';
  static String projectMembers(String id) => '/projects/$id/members';

  // Boards
  static String boardColumns(String projectId) => '/boards/$projectId/columns';

  // Tasks
  static const String tasks = '/tasks';

  // Organizations
  static String orgMembers(String orgId) => '/organizations/$orgId/members';

  // Sync
  static const String syncPush = '/sync/push';
  static const String syncPull = '/sync/pull';

  // Notifications
  static const String notifications = '/notifications';
  static const String notificationsUnreadCount = '/notifications/unread-count';

  // Sprints
  static const String sprints = '/sprints';
  static String sprintStart(String id) => '/sprints/$id/start';
  static String sprintComplete(String id) => '/sprints/$id/complete';
  static String sprintCancel(String id) => '/sprints/$id/cancel';
  static String sprint(String id) => '/sprints/$id';

  // Backlog
  static const String backlog = '/backlog';
  static const String backlogReorder = '/backlog/reorder';
  static const String tasksBulk = '/tasks/bulk';

  // Comments (task-scoped list/create; id-scoped update/delete)
  static String taskComments(String taskId) => '/tasks/$taskId/comments';
  static String comment(String id) => '/comments/$id';

  // Attachments
  static String taskAttachments(String taskId) => '/tasks/$taskId/attachments';
  static String attachmentDownload(String id) => '/attachments/$id/download';
  static String attachment(String id) => '/attachments/$id';

  // Time tracking
  static String taskTimeEntries(String taskId) => '/tasks/$taskId/time-entries';
  static String taskTimeSummary(String taskId) => '/tasks/$taskId/time-entries/summary';
  static String taskTimerStart(String taskId) => '/tasks/$taskId/time-entries/start';
  static const String timerStop = '/time-entries/stop';
  static String timeEntry(String id) => '/time-entries/$id';
}
