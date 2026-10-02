import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../core/utils/time_ago.dart';
import '../../data/models/attachment_model.dart';
import 'attachments_controller.dart';

class AttachmentsScreen extends StatelessWidget {
  const AttachmentsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<AttachmentsController>();
    return Scaffold(
      appBar: AppBar(title: const Text('Attachments')),
      floatingActionButton: Obx(() => FloatingActionButton.extended(
            onPressed: controller.busy.value ? null : () => _upload(context, controller),
            icon: const Icon(Icons.upload_file),
            label: const Text('Upload'),
          )),
      body: Obx(() {
        if (controller.isLoading.value && controller.items.isEmpty) {
          return const Center(child: CircularProgressIndicator());
        }
        return Stack(
          children: [
            RefreshIndicator(
              onRefresh: controller.load,
              child: controller.items.isEmpty
                  ? ListView(children: [
                      const SizedBox(height: 120),
                      Center(
                        child: Text(controller.error.value.isEmpty
                            ? 'No attachments yet.'
                            : controller.error.value),
                      ),
                    ])
                  : ListView(
                      padding: const EdgeInsets.all(12),
                      children: controller.items
                          .map((a) => _AttachmentTile(
                                attachment: a,
                                onDownload: () => _download(context, controller, a),
                                onDelete: () => _delete(context, controller, a),
                              ))
                          .toList(),
                    ),
            ),
            if (controller.busy.value)
              const Positioned(
                top: 0, left: 0, right: 0,
                child: LinearProgressIndicator(minHeight: 3),
              ),
          ],
        );
      }),
    );
  }

  Future<void> _upload(BuildContext context, AttachmentsController controller) async {
    final messenger = ScaffoldMessenger.of(context);
    final err = await controller.pickAndUpload();
    if (err != null) {
      messenger.showSnackBar(SnackBar(content: Text(err)));
    }
  }

  Future<void> _download(
      BuildContext context, AttachmentsController controller, AttachmentModel a) async {
    final messenger = ScaffoldMessenger.of(context);
    final path = await controller.download(a);
    messenger.showSnackBar(SnackBar(
      content: Text(path != null ? 'Saved to $path' : controller.error.value),
    ));
  }

  Future<void> _delete(
      BuildContext context, AttachmentsController controller, AttachmentModel a) async {
    final messenger = ScaffoldMessenger.of(context);
    final ok = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text('Delete attachment?'),
        content: Text('Remove "${a.originalFileName}"? This also deletes the stored file.'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context, false), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.pop(context, true), child: const Text('Delete')),
        ],
      ),
    );
    if (ok == true) {
      final done = await controller.remove(a.id);
      if (!done) messenger.showSnackBar(SnackBar(content: Text(controller.error.value)));
    }
  }
}

class _AttachmentTile extends StatelessWidget {
  const _AttachmentTile({
    required this.attachment,
    required this.onDownload,
    required this.onDelete,
  });
  final AttachmentModel attachment;
  final VoidCallback onDownload;
  final VoidCallback onDelete;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: Icon(_iconFor(attachment.contentType), size: 32,
            color: theme.colorScheme.primary),
        title: Text(attachment.originalFileName,
            maxLines: 1, overflow: TextOverflow.ellipsis),
        subtitle: Text(
          '${attachment.sizeLabel} · ${attachment.uploadedByName} · ${timeAgo(attachment.uploadedAt)}',
          style: theme.textTheme.labelSmall,
        ),
        trailing: PopupMenuButton<String>(
          onSelected: (v) => v == 'download' ? onDownload() : onDelete(),
          itemBuilder: (_) => const [
            PopupMenuItem(value: 'download', child: Text('Download')),
            PopupMenuItem(value: 'delete', child: Text('Delete')),
          ],
        ),
      ),
    );
  }

  IconData _iconFor(String contentType) {
    if (contentType.startsWith('image/')) return Icons.image_outlined;
    if (contentType.startsWith('video/')) return Icons.movie_outlined;
    if (contentType.startsWith('audio/')) return Icons.audiotrack_outlined;
    if (contentType.contains('pdf')) return Icons.picture_as_pdf_outlined;
    if (contentType.contains('zip') || contentType.contains('compressed')) {
      return Icons.folder_zip_outlined;
    }
    if (contentType.startsWith('text/')) return Icons.description_outlined;
    return Icons.insert_drive_file_outlined;
  }
}
