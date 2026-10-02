import 'package:get/get.dart';

import '../../core/errors/failure.dart';
import '../../data/datasources/remote/dashboard_remote.dart';
import '../../data/models/dashboard_model.dart';

class DashboardController extends GetxController {
  DashboardController(this._remote);

  final DashboardRemote _remote;

  late final String projectId;
  final Rxn<DashboardModel> data = Rxn<DashboardModel>();
  final RxBool isLoading = false.obs;
  final RxString error = ''.obs;

  @override
  void onInit() {
    super.onInit();
    projectId = (Get.arguments as Map)['projectId'] as String;
    load();
  }

  Future<void> load() async {
    isLoading.value = true;
    error.value = '';
    try {
      data.value = await _remote.getDashboard(projectId);
    } on Failure catch (f) {
      error.value = f.message;
    } finally {
      isLoading.value = false;
    }
  }
}
