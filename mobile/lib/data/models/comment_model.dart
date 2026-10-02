/// Mirrors the server CommentDto. Threaded one level: a top-level comment carries
/// its [replies] inline; a reply's own [replies] list is always empty.
class CommentModel {
  final String id;
  final String taskId;
  final String authorUserId;
  final String authorName;
  final String body;
  final String? parentCommentId;
  final bool isEdited;
  final DateTime? editedAt;
  final DateTime createdAt;
  final List<MentionModel> mentions;
  final List<CommentModel> replies;

  CommentModel({
    required this.id,
    required this.taskId,
    required this.authorUserId,
    required this.authorName,
    required this.body,
    this.parentCommentId,
    required this.isEdited,
    this.editedAt,
    required this.createdAt,
    required this.mentions,
    required this.replies,
  });

  factory CommentModel.fromJson(Map<String, dynamic> j) => CommentModel(
        id: j['id'] as String,
        taskId: j['taskId'] as String,
        authorUserId: (j['authorUserId'] ?? '') as String,
        authorName: (j['authorName'] ?? '') as String,
        body: (j['body'] ?? '') as String,
        parentCommentId: j['parentCommentId'] as String?,
        isEdited: (j['isEdited'] ?? false) as bool,
        editedAt: j['editedAt'] == null
            ? null
            : DateTime.tryParse(j['editedAt'] as String),
        createdAt: DateTime.tryParse((j['createdAt'] ?? '') as String) ??
            DateTime.fromMillisecondsSinceEpoch(0),
        mentions: (j['mentions'] as List<dynamic>? ?? [])
            .map((e) => MentionModel.fromJson(e as Map<String, dynamic>))
            .toList(),
        replies: (j['replies'] as List<dynamic>? ?? [])
            .map((e) => CommentModel.fromJson(e as Map<String, dynamic>))
            .toList(),
      );
}

class MentionModel {
  final String userId;
  final String name;
  MentionModel(this.userId, this.name);

  factory MentionModel.fromJson(Map<String, dynamic> j) =>
      MentionModel((j['userId'] ?? '') as String, (j['name'] ?? '') as String);
}
