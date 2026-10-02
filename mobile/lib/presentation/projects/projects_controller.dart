import 'package:get/get.dart';

import '../../core/database/app_database.dart';
import '../../core/errors/failure.dart';
import '../../data/repositories/auth_repository.dart';
import '../../data/repositories/project_repository.dart';

class ProjectsController extends GetxController {
  ProjectsController(this._projects, this._auth);

  final ProjectRepository _projects;
  final AuthRepository _auth;

  final RxList<LocalProject> projects = <LocalProject>[].obs;
  final RxBool isLoading = false.obs;
  final RxString error = ''.obs;
  final RxString userName = ''.obs;

  @override
  void onInit() {
    super.onInit();
    _load();
  }

  Future<void> _load() async {
    isLoading.value = true;
    userName.value = (await _auth.currentUserName()) ?? '';
    // Serve the cache first (instant, works offline)...
    projects.assignAll(await _projects.cachedProjects());
    // ...then refresh from the server if we can.
    await refreshList();
    isLoading.value = false;
  }

  Future<void> refreshList() async {
    error.value = '';
    try {
      await _projects.tryRefreshProjects();
      projects.assignAll(await _projects.cachedProjects());
    } on Failure catch (f) {
      error.value = f.message;
    }
  }

  Future<String?> createProject({
    required String key,
    required String name,
    String? description,
  }) async {
    try {
      await _projects.createProject(key: key.toUpperCase(), name: name, description: description);
      projects.assignAll(await _projects.cachedProjects());
      return null;
    } on ValidationFailure catch (f) {
      return f.fieldErrors.values.isNotEmpty ? f.fieldErrors.values.first : f.message;
    } on Failure catch (f) {
      return f.message;
    }
  }
}
