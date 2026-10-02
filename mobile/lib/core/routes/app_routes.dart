/// Named-route constants. Kept string-only so they can be referenced from the
/// DI layer (e.g. the session-expired redirect) without importing pages.
abstract class Routes {
  static const splash = '/';
  static const login = '/login';
  static const home = '/home'; // bottom-nav shell (projects/alerts/profile)
  static const board = '/board'; // expects projectId arg
  static const dashboard = '/dashboard'; // expects projectId arg
  static const taskDetail = '/task'; // expects taskId arg
  static const syncCenter = '/sync-center';
  static const backlog = '/backlog'; // expects projectId arg
  static const sprints = '/sprints'; // expects projectId arg
  static const taskComments = '/task/comments'; // expects taskId arg
  static const taskAttachments = '/task/attachments'; // expects taskId arg
  static const taskTime = '/task/time'; // expects taskId arg
}
