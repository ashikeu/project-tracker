import 'package:get/get.dart';

import '../../core/errors/failure.dart';
import '../../data/datasources/remote/comment_remote.dart';
import '../../data/models/comment_model.dart';
import '../../data/repositories/auth_repository.dart';

/// Threaded task comments (online-only). Knows the current user id so the UI can
/// offer edit/delete only on the user's own comments (the server enforces the
/// same rule — this is just to avoid showing actions that would be rejected).
class CommentsController extends GetxController {
  CommentsController(this._remote, this._auth);

  final CommentRemote _remote;
  final AuthRepository _auth;

  late final String taskId;
  final RxList<CommentModel> comments = <CommentModel>[].obs;
  final RxBool isLoading = false.obs;
  final RxString error = ''.obs;
  final RxBool busy = false.obs;
  final RxnString currentUserId = RxnString();

  @override
  void onInit() {
    super.onInit();
    taskId = (Get.arguments as Map)['taskId'] as String;
    _loadUser();
    load();
  }

  Future<void> _loadUser() async {
    currentUserId.value = await _auth.currentUserId();
  }

  bool isMine(CommentModel c) =>
      currentUserId.value != null && c.authorUserId == currentUserId.value;

  Future<void> load() async {
    isLoading.value = true;
    error.value = '';
    try {
      comments.assignAll(await _remote.list(taskId));
    } on Failure catch (f) {
      error.value = f.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<bool> post(String body, {String? parentCommentId}) =>
      _run(() => _remote.create(taskId, body, parentCommentId: parentCommentId));

  Future<bool> edit(String id, String body) => _run(() => _remote.update(id, body));

  Future<bool> remove(String id) => _run(() => _remote.delete(id));

  Future<bool> _run(Future<void> Function() action) async {
    busy.value = true;
    error.value = '';
    try {
      await action();
      await load();
      return true;
    } on Failure catch (f) {
      error.value = f.message;
      return false;
    } finally {
      busy.value = false;
    }
  }
}
