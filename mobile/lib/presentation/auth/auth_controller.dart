import 'dart:async';

import 'package:get/get.dart';

import '../../core/errors/failure.dart';
import '../../core/routes/app_routes.dart';
import '../../data/repositories/auth_repository.dart';
import '../../data/sync/sync_engine.dart';

class AuthController extends GetxController {
  AuthController(this._auth, this._sync);

  final AuthRepository _auth;
  final SyncEngine _sync;

  final RxBool isLoading = false.obs;
  final RxString error = ''.obs;

  Future<void> login(String email, String password) async {
    if (email.trim().isEmpty || password.isEmpty) {
      error.value = 'Enter your email and password.';
      return;
    }
    isLoading.value = true;
    error.value = '';
    try {
      await _auth.login(email.trim(), password);
      // Kick an initial sync so the board has data as soon as it opens.
      unawaited(_sync.sync());
      Get.offAllNamed(Routes.home);
    } on ValidationFailure catch (f) {
      error.value = f.fieldErrors.values.isNotEmpty ? f.fieldErrors.values.first : f.message;
    } on Failure catch (f) {
      error.value = f.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> logout() async {
    await _auth.logout();
    Get.offAllNamed(Routes.login);
  }
}
