import 'package:get/get.dart';

import '../../core/errors/failure.dart';
import '../../data/datasources/remote/notification_remote.dart';
import '../../data/models/notification_model.dart';

class NotificationsController extends GetxController {
  NotificationsController(this._remote);

  final NotificationRemote _remote;

  final RxList<NotificationModel> items = <NotificationModel>[].obs;
  final RxInt unread = 0.obs;
  final RxBool isLoading = false.obs;
  final RxString error = ''.obs;

  @override
  void onInit() {
    super.onInit();
    load();
  }

  Future<void> load() async {
    isLoading.value = true;
    error.value = '';
    try {
      items.assignAll(await _remote.list());
      unread.value = await _remote.unreadCount();
    } on Failure catch (f) {
      error.value = f.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> markRead(NotificationModel n) async {
    if (n.isRead) return;
    try {
      await _remote.markRead(n.id);
      await load();
    } on Failure catch (f) {
      error.value = f.message;
    }
  }

  Future<void> markAllRead() async {
    try {
      await _remote.markAllRead();
      await load();
    } on Failure catch (f) {
      error.value = f.message;
    }
  }
}
