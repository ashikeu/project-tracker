/// Mirrors the server AttachmentDto (metadata only; the bytes are streamed from
/// the download endpoint).
class AttachmentModel {
  final String id;
  final String entityType;
  final String entityId;
  final String originalFileName;
  final String contentType;
  final int fileSize;
  final String uploadedByUserId;
  final String uploadedByName;
  final DateTime uploadedAt;

  AttachmentModel({
    required this.id,
    required this.entityType,
    required this.entityId,
    required this.originalFileName,
    required this.contentType,
    required this.fileSize,
    required this.uploadedByUserId,
    required this.uploadedByName,
    required this.uploadedAt,
  });

  /// Human-friendly size label (e.g. "12.4 KB", "3.1 MB").
  String get sizeLabel {
    const units = ['B', 'KB', 'MB', 'GB'];
    var size = fileSize.toDouble();
    var unit = 0;
    while (size >= 1024 && unit < units.length - 1) {
      size /= 1024;
      unit++;
    }
    final text = unit == 0 ? size.toStringAsFixed(0) : size.toStringAsFixed(1);
    return '$text ${units[unit]}';
  }

  factory AttachmentModel.fromJson(Map<String, dynamic> j) => AttachmentModel(
        id: j['id'] as String,
        entityType: (j['entityType'] ?? '') as String,
        entityId: (j['entityId'] ?? '') as String,
        originalFileName: (j['originalFileName'] ?? '') as String,
        contentType: (j['contentType'] ?? '') as String,
        fileSize: (j['fileSize'] as num?)?.toInt() ?? 0,
        uploadedByUserId: (j['uploadedByUserId'] ?? '') as String,
        uploadedByName: (j['uploadedByName'] ?? '') as String,
        uploadedAt: DateTime.tryParse((j['uploadedAt'] ?? '') as String) ??
            DateTime.fromMillisecondsSinceEpoch(0),
      );
}
