/// Mirrors the server NotificationDto.
class NotificationModel {
  final String id;
  final String type;
  final String title;
  final String body;
  final String? relatedEntityType;
  final String? relatedEntityId;
  final bool isRead;
  final DateTime? readAt;
  final DateTime createdAt;

  NotificationModel({
    required this.id,
    required this.type,
    required this.title,
    required this.body,
    this.relatedEntityType,
    this.relatedEntityId,
    required this.isRead,
    this.readAt,
    required this.createdAt,
  });

  factory NotificationModel.fromJson(Map<String, dynamic> j) => NotificationModel(
        id: j['id'] as String,
        type: (j['type'] ?? '') as String,
        title: (j['title'] ?? '') as String,
        body: (j['body'] ?? '') as String,
        relatedEntityType: j['relatedEntityType'] as String?,
        relatedEntityId: j['relatedEntityId'] as String?,
        isRead: j['isRead'] == true,
        readAt: j['readAt'] == null ? null : DateTime.parse(j['readAt'] as String),
        createdAt: DateTime.parse(j['createdAt'] as String),
      );
}
