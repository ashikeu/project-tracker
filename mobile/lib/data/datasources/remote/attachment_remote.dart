import 'dart:typed_data';

import 'package:dio/dio.dart';

import '../../../core/constants/api_constants.dart';
import '../../../core/errors/failure.dart';
import '../../../core/network/response_handler.dart';
import '../../models/attachment_model.dart';

/// Downloaded file bytes plus the metadata needed to save them.
class DownloadedFile {
  final Uint8List bytes;
  final String fileName;
  final String contentType;
  DownloadedFile(this.bytes, this.fileName, this.contentType);
}

/// Task attachments are server-owned (not offline-synced); a thin online remote
/// over the attachments endpoints. Upload posts multipart form-data (field
/// `file`, matching the controller's IFormFile parameter); download streams raw
/// bytes back (not the JSON envelope).
class AttachmentRemote {
  AttachmentRemote(this._dio);
  final Dio _dio;

  Future<List<AttachmentModel>> list(String taskId,
      {int page = 1, int pageSize = 100}) async {
    try {
      final resp = await _dio.get(ApiConstants.taskAttachments(taskId),
          queryParameters: {'page': page, 'pageSize': pageSize});
      final data = unwrap(resp) as Map<String, dynamic>;
      final items = (data['items'] as List<dynamic>? ?? []);
      return items
          .map((e) => AttachmentModel.fromJson(e as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<AttachmentModel> upload(
    String taskId, {
    required String filePath,
    required String fileName,
  }) async {
    try {
      final form = FormData.fromMap({
        'file': await MultipartFile.fromFile(filePath, filename: fileName),
      });
      final resp = await _dio.post(ApiConstants.taskAttachments(taskId), data: form);
      return AttachmentModel.fromJson(unwrap(resp) as Map<String, dynamic>);
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<DownloadedFile> download(AttachmentModel meta) async {
    try {
      final resp = await _dio.get<List<int>>(
        ApiConstants.attachmentDownload(meta.id),
        options: Options(responseType: ResponseType.bytes),
      );
      final code = resp.statusCode ?? 0;
      if (code < 200 || code >= 300) {
        throw ServerFailure('Download failed ($code).', statusCode: code);
      }
      return DownloadedFile(
        Uint8List.fromList(resp.data ?? const []),
        meta.originalFileName,
        meta.contentType,
      );
    } catch (e) {
      throw mapDioError(e);
    }
  }

  Future<void> delete(String id) async {
    try {
      final resp = await _dio.delete(ApiConstants.attachment(id));
      unwrap(resp);
    } catch (e) {
      throw mapDioError(e);
    }
  }
}
