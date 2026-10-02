import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:get/get.dart';

import '../../data/datasources/remote/attachment_remote.dart';
import '../../data/datasources/remote/auth_remote.dart';
import '../../data/datasources/remote/backlog_remote.dart';
import '../../data/datasources/remote/comment_remote.dart';
import '../../data/datasources/remote/dashboard_remote.dart';
import '../../data/datasources/remote/notification_remote.dart';
import '../../data/datasources/remote/project_remote.dart';
import '../../data/datasources/remote/sprint_remote.dart';
import '../../data/datasources/remote/sync_remote.dart';
import '../../data/datasources/remote/time_entry_remote.dart';
import '../../data/repositories/auth_repository.dart';
import '../../data/repositories/project_repository.dart';
import '../../data/repositories/task_repository.dart';
import '../../data/sync/sync_engine.dart';
import '../database/app_database.dart';
import '../network/dio_client.dart';
import '../routes/app_routes.dart';
import '../services/connectivity_service.dart';
import '../storage/secure_storage_service.dart';

/// Composition root. Registers every long-lived singleton with GetX before the
/// app renders, in dependency order. Called once from main().
Future<void> setupDependencies() async {
  // Persistence + secure storage
  final db = Get.put(AppDatabase(), permanent: true);
  final storage = Get.put(
    SecureStorageService(const FlutterSecureStorage()),
    permanent: true,
  );

  // Connectivity (reactive online/offline)
  final connectivity = await Get.putAsync<ConnectivityService>(
    () => ConnectivityService().init(),
    permanent: true,
  );

  // Networking — on an unrecoverable 401 the client clears the session and we
  // bounce the user to the login screen.
  final dioClient = Get.put(
    DioClient(storage, onSessionExpired: () {
      if (Get.currentRoute != Routes.login) {
        Get.offAllNamed(Routes.login);
      }
    }),
    permanent: true,
  );

  // Remote datasources
  final authRemote = AuthRemote(dioClient.dio);
  final projectRemote = ProjectRemote(dioClient.dio);
  final syncRemote = SyncRemote(dioClient.dio);
  Get.put(NotificationRemote(dioClient.dio), permanent: true);
  Get.put(DashboardRemote(dioClient.dio), permanent: true);
  Get.put(SprintRemote(dioClient.dio), permanent: true);
  Get.put(BacklogRemote(dioClient.dio), permanent: true);
  Get.put(CommentRemote(dioClient.dio), permanent: true);
  Get.put(AttachmentRemote(dioClient.dio), permanent: true);
  Get.put(TimeEntryRemote(dioClient.dio), permanent: true);

  // Repositories
  Get.put(AuthRepository(authRemote, storage), permanent: true);
  Get.put(ProjectRepository(projectRemote, db), permanent: true);
  Get.put(TaskRepository(db), permanent: true);

  // Sync engine (listens for reconnect + ticks on a timer)
  Get.put(SyncEngine(db, syncRemote, connectivity).start(), permanent: true);
}
