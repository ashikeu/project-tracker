import 'dart:io';

import 'package:file_picker/file_picker.dart';
import 'package:get/get.dart';
import 'package:path/path.dart' as p;
import 'package:path_provider/path_provider.dart';

import '../../core/errors/failure.dart';
import '../../data/datasources/remote/attachment_remote.dart';
import '../../data/models/attachment_model.dart';

/// Lists, uploads, downloads and deletes task attachments (online-only). Upload
/// uses the native file picker; download streams the bytes and saves them under
/// the app documents directory, returning the saved path for the UI to surface.
class AttachmentsController extends GetxController {
  AttachmentsController(this._remote);

  final AttachmentRemote _remote;

  late final String taskId;
  final RxList<AttachmentModel> items = <AttachmentModel>[].obs;
  final RxBool isLoading = false.obs;
  final RxString error = ''.obs;
  final RxBool busy = false.obs; // upload/download/delete in flight

  @override
  void onInit() {
    super.onInit();
    taskId = (Get.arguments as Map)['taskId'] as String;
    load();
  }

  Future<void> load() async {
    isLoading.value = true;
    error.value = '';
    try {
      items.assignAll(await _remote.list(taskId));
    } on Failure catch (f) {
      error.value = f.message;
    } finally {
      isLoading.value = false;
    }
  }

  /// Opens the native picker and uploads the chosen file. Returns null on
  /// success, a user-facing message on failure, or null-with-no-change when the
  /// user cancels the picker.
  Future<String?> pickAndUpload() async {
    final picked = await FilePicker.pickFile();
    final path = picked?.path;
    if (path == null) return null; // cancelled, or no on-disk path (web)
    final name = picked!.name;

    busy.value = true;
    error.value = '';
    try {
      await _remote.upload(taskId, filePath: path, fileName: name);
      await load();
      return null;
    } on Failure catch (f) {
      error.value = f.message;
      return f.message;
    } finally {
      busy.value = false;
    }
  }

  /// Downloads [meta] and saves it under the app documents `attachments/`
  /// folder, returning the saved file path (errors go to [error], not thrown).
  Future<String?> download(AttachmentModel meta) async {
    busy.value = true;
    error.value = '';
    try {
      final file = await _remote.download(meta);
      final dir = await getApplicationDocumentsDirectory();
      final folder = Directory(p.join(dir.path, 'attachments'));
      if (!await folder.exists()) await folder.create(recursive: true);
      final dest = File(p.join(folder.path, meta.originalFileName));
      await dest.writeAsBytes(file.bytes);
      return dest.path;
    } on Failure catch (f) {
      error.value = f.message;
      return null;
    } finally {
      busy.value = false;
    }
  }

  Future<bool> remove(String id) async {
    busy.value = true;
    error.value = '';
    try {
      await _remote.delete(id);
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
