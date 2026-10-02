import 'package:get/get.dart';

import '../../data/datasources/remote/attachment_remote.dart';
import '../../data/datasources/remote/backlog_remote.dart';
import '../../data/datasources/remote/comment_remote.dart';
import '../../data/datasources/remote/dashboard_remote.dart';
import '../../data/datasources/remote/notification_remote.dart';
import '../../data/datasources/remote/sprint_remote.dart';
import '../../data/datasources/remote/time_entry_remote.dart';
import '../../data/repositories/auth_repository.dart';
import '../../data/repositories/project_repository.dart';
import '../../data/repositories/task_repository.dart';
import '../../data/sync/sync_engine.dart';
import '../../presentation/attachments/attachments_controller.dart';
import '../../presentation/attachments/attachments_screen.dart';
import '../../presentation/auth/auth_controller.dart';
import '../../presentation/auth/login_screen.dart';
import '../../presentation/backlog/backlog_controller.dart';
import '../../presentation/backlog/backlog_screen.dart';
import '../../presentation/board/board_controller.dart';
import '../../presentation/board/board_screen.dart';
import '../../presentation/comments/comments_controller.dart';
import '../../presentation/comments/comments_screen.dart';
import '../../presentation/dashboard/dashboard_controller.dart';
import '../../presentation/dashboard/dashboard_screen.dart';
import '../../presentation/home/home_shell.dart';
import '../../presentation/notifications/notifications_controller.dart';
import '../../presentation/projects/projects_controller.dart';
import '../../presentation/splash/splash_screen.dart';
import '../../presentation/sprints/sprints_controller.dart';
import '../../presentation/sprints/sprints_screen.dart';
import '../../presentation/sync/sync_center_controller.dart';
import '../../presentation/sync/sync_center_screen.dart';
import '../../presentation/task/task_detail_controller.dart';
import '../../presentation/task/task_detail_screen.dart';
import '../../presentation/time/time_controller.dart';
import '../../presentation/time/time_screen.dart';
import 'app_routes.dart';

/// Route table. Each page lazily binds its controller from the already-registered
/// singletons (see injector.dart), so controllers stay out of memory until used.
class AppPages {
  static final routes = <GetPage>[
    GetPage(name: Routes.splash, page: () => const SplashScreen()),
    GetPage(
      name: Routes.login,
      page: () => const LoginScreen(),
      binding: BindingsBuilder(() {
        Get.lazyPut(() => AuthController(Get.find<AuthRepository>(), Get.find<SyncEngine>()));
      }),
    ),
    GetPage(
      name: Routes.home,
      page: () => const HomeShell(),
      binding: BindingsBuilder(() {
        // The shell hosts Projects/Alerts/Profile tabs, so bind all three plus
        // Auth (used by Profile's sign-out). NotificationsController is eager so
        // its unread badge is live the moment the shell renders.
        Get.lazyPut(() => AuthController(Get.find<AuthRepository>(), Get.find<SyncEngine>()));
        Get.lazyPut(() =>
            ProjectsController(Get.find<ProjectRepository>(), Get.find<AuthRepository>()));
        Get.put(NotificationsController(Get.find<NotificationRemote>()));
      }),
    ),
    GetPage(
      name: Routes.board,
      page: () => const BoardScreen(),
      binding: BindingsBuilder(() {
        Get.lazyPut(() => BoardController(
              Get.find<ProjectRepository>(),
              Get.find<TaskRepository>(),
              Get.find<SyncEngine>(),
              Get.find<AuthRepository>(),
            ));
      }),
    ),
    GetPage(
      name: Routes.dashboard,
      page: () => const DashboardScreen(),
      binding: BindingsBuilder(() {
        Get.lazyPut(() => DashboardController(Get.find<DashboardRemote>()));
      }),
    ),
    GetPage(
      name: Routes.taskDetail,
      page: () => const TaskDetailScreen(),
      binding: BindingsBuilder(() {
        Get.lazyPut(() => TaskDetailController(Get.find<TaskRepository>(), Get.find<SyncEngine>()));
      }),
    ),
    GetPage(
      name: Routes.syncCenter,
      page: () => const SyncCenterScreen(),
      binding: BindingsBuilder(() {
        Get.lazyPut(() => SyncCenterController(Get.find<SyncEngine>(), Get.find<TaskRepository>()));
      }),
    ),
    GetPage(
      name: Routes.backlog,
      page: () => const BacklogScreen(),
      binding: BindingsBuilder(() {
        Get.lazyPut(() => BacklogController(Get.find<BacklogRemote>(), Get.find<SprintRemote>()));
      }),
    ),
    GetPage(
      name: Routes.sprints,
      page: () => const SprintsScreen(),
      binding: BindingsBuilder(() {
        Get.lazyPut(() => SprintsController(Get.find<SprintRemote>()));
      }),
    ),
    GetPage(
      name: Routes.taskComments,
      page: () => const CommentsScreen(),
      binding: BindingsBuilder(() {
        Get.lazyPut(() => CommentsController(Get.find<CommentRemote>(), Get.find<AuthRepository>()));
      }),
    ),
    GetPage(
      name: Routes.taskAttachments,
      page: () => const AttachmentsScreen(),
      binding: BindingsBuilder(() {
        Get.lazyPut(() => AttachmentsController(Get.find<AttachmentRemote>()));
      }),
    ),
    GetPage(
      name: Routes.taskTime,
      page: () => const TimeScreen(),
      binding: BindingsBuilder(() {
        Get.lazyPut(() => TimeController(Get.find<TimeEntryRemote>(), Get.find<AuthRepository>()));
      }),
    ),
  ];
}
