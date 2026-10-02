import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../core/utils/time_ago.dart';
import '../../data/models/comment_model.dart';
import 'comments_controller.dart';

class CommentsScreen extends StatelessWidget {
  const CommentsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<CommentsController>();
    return Scaffold(
      appBar: AppBar(title: const Text('Comments')),
      body: Column(
        children: [
          Expanded(
            child: RefreshIndicator(
              onRefresh: controller.load,
              child: Obx(() {
                // Touch currentUserId so the list rebuilds once it resolves —
                // the per-comment author menus depend on it.
                controller.currentUserId.value;
                if (controller.isLoading.value && controller.comments.isEmpty) {
                  return const Center(child: CircularProgressIndicator());
                }
                if (controller.comments.isEmpty) {
                  return ListView(children: [
                    const SizedBox(height: 120),
                    Center(
                      child: Text(controller.error.value.isEmpty
                          ? 'No comments yet. Start the discussion.'
                          : controller.error.value),
                    ),
                  ]);
                }
                return ListView(
                  padding: const EdgeInsets.all(12),
                  children: controller.comments
                      .map((c) => _CommentThread(comment: c, controller: controller))
                      .toList(),
                );
              }),
            ),
          ),
          _Composer(controller: controller),
        ],
      ),
    );
  }
}

class _CommentThread extends StatelessWidget {
  const _CommentThread({required this.comment, required this.controller});
  final CommentModel comment;
  final CommentsController controller;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _CommentCard(comment: comment, controller: controller, isReply: false),
        if (comment.replies.isNotEmpty)
          Padding(
            padding: const EdgeInsets.only(left: 24),
            child: Column(
              children: comment.replies
                  .map((r) => _CommentCard(comment: r, controller: controller, isReply: true))
                  .toList(),
            ),
          ),
      ],
    );
  }
}

class _CommentCard extends StatelessWidget {
  const _CommentCard({required this.comment, required this.controller, required this.isReply});
  final CommentModel comment;
  final CommentsController controller;
  final bool isReply;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final mine = controller.isMine(comment);
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      color: isReply ? theme.colorScheme.surfaceContainerLow : null,
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  radius: 12,
                  child: Text(
                    comment.authorName.isNotEmpty
                        ? comment.authorName.characters.first.toUpperCase()
                        : '?',
                    style: theme.textTheme.labelSmall,
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(comment.authorName,
                      style: theme.textTheme.labelLarge,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis),
                ),
                Text(
                  '${timeAgo(comment.createdAt)}${comment.isEdited ? " · edited" : ""}',
                  style: theme.textTheme.labelSmall
                      ?.copyWith(color: theme.colorScheme.onSurfaceVariant),
                ),
                if (!isReply || mine)
                  _CommentMenu(
                    comment: comment,
                    controller: controller,
                    canReply: !isReply,
                    canEditDelete: mine,
                  ),
              ],
            ),
            const SizedBox(height: 6),
            Text(comment.body, style: theme.textTheme.bodyMedium),
          ],
        ),
      ),
    );
  }
}

class _CommentMenu extends StatelessWidget {
  const _CommentMenu({
    required this.comment,
    required this.controller,
    required this.canReply,
    required this.canEditDelete,
  });
  final CommentModel comment;
  final CommentsController controller;
  final bool canReply;
  final bool canEditDelete;

  @override
  Widget build(BuildContext context) {
    return PopupMenuButton<String>(
      padding: EdgeInsets.zero,
      iconSize: 18,
      onSelected: (v) => _handle(context, v),
      itemBuilder: (_) => [
        if (canReply) const PopupMenuItem(value: 'reply', child: Text('Reply')),
        if (canEditDelete) const PopupMenuItem(value: 'edit', child: Text('Edit')),
        if (canEditDelete) const PopupMenuItem(value: 'delete', child: Text('Delete')),
      ],
    );
  }

  Future<void> _handle(BuildContext context, String action) async {
    switch (action) {
      case 'reply':
        await _editDialog(context, title: 'Reply', onSubmit: (text) =>
            controller.post(text, parentCommentId: comment.id));
      case 'edit':
        await _editDialog(context, title: 'Edit comment', initial: comment.body,
            onSubmit: (text) => controller.edit(comment.id, text));
      case 'delete':
        final messenger = ScaffoldMessenger.of(context);
        final ok = await showDialog<bool>(
          context: context,
          builder: (_) => AlertDialog(
            title: const Text('Delete comment?'),
            content: const Text('This cannot be undone.'),
            actions: [
              TextButton(onPressed: () => Navigator.pop(context, false), child: const Text('Cancel')),
              FilledButton(onPressed: () => Navigator.pop(context, true), child: const Text('Delete')),
            ],
          ),
        );
        if (ok == true) {
          final done = await controller.remove(comment.id);
          if (!done) messenger.showSnackBar(SnackBar(content: Text(controller.error.value)));
        }
    }
  }

  Future<void> _editDialog(BuildContext context,
      {required String title, String? initial, required Future<bool> Function(String) onSubmit}) async {
    final ctrl = TextEditingController(text: initial ?? '');
    final messenger = ScaffoldMessenger.of(context);
    final submitted = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(title),
        content: TextField(
          controller: ctrl,
          autofocus: true,
          maxLines: 4,
          minLines: 1,
          decoration: const InputDecoration(hintText: 'Write a comment…'),
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context, false), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.pop(context, true), child: const Text('Send')),
        ],
      ),
    );
    if (submitted == true && ctrl.text.trim().isNotEmpty) {
      final ok = await onSubmit(ctrl.text.trim());
      if (!ok) messenger.showSnackBar(SnackBar(content: Text(controller.error.value)));
    }
  }
}

class _Composer extends StatefulWidget {
  const _Composer({required this.controller});
  final CommentsController controller;

  @override
  State<_Composer> createState() => _ComposerState();
}

class _ComposerState extends State<_Composer> {
  final _ctrl = TextEditingController();

  @override
  void dispose() {
    _ctrl.dispose();
    super.dispose();
  }

  Future<void> _send() async {
    final text = _ctrl.text.trim();
    if (text.isEmpty) return;
    final messenger = ScaffoldMessenger.of(context);
    final ok = await widget.controller.post(text);
    if (ok) {
      _ctrl.clear();
    } else {
      messenger.showSnackBar(SnackBar(content: Text(widget.controller.error.value)));
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return SafeArea(
      top: false,
      child: Container(
        padding: const EdgeInsets.fromLTRB(12, 8, 12, 8),
        decoration: BoxDecoration(
          color: theme.colorScheme.surface,
          border: Border(top: BorderSide(color: theme.colorScheme.outlineVariant)),
        ),
        child: Row(
          children: [
            Expanded(
              child: TextField(
                controller: _ctrl,
                minLines: 1,
                maxLines: 4,
                textInputAction: TextInputAction.newline,
                decoration: const InputDecoration(
                  hintText: 'Add a comment…',
                  border: OutlineInputBorder(),
                  isDense: true,
                ),
              ),
            ),
            const SizedBox(width: 8),
            Obx(() => IconButton.filled(
                  onPressed: widget.controller.busy.value ? null : _send,
                  icon: widget.controller.busy.value
                      ? const SizedBox(width: 18, height: 18, child: CircularProgressIndicator(strokeWidth: 2))
                      : const Icon(Icons.send),
                )),
          ],
        ),
      ),
    );
  }
}
