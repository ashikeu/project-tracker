import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../core/utils/time_ago.dart';
import 'notifications_controller.dart';

/// Notifications tab. Server-owned and online-read; pull-to-refresh reloads.
class NotificationsScreen extends StatelessWidget {
  const NotificationsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<NotificationsController>();
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Notifications'),
        actions: [
          Obx(() => controller.unread.value == 0
              ? const SizedBox.shrink()
              : TextButton(
                  onPressed: controller.markAllRead,
                  child: const Text('Mark all read'),
                )),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: controller.load,
        child: Obx(() {
          if (controller.isLoading.value && controller.items.isEmpty) {
            return const Center(child: CircularProgressIndicator());
          }
          if (controller.items.isEmpty) {
            return ListView(
              children: [
                const SizedBox(height: 120),
                Icon(Icons.notifications_none, size: 64, color: theme.colorScheme.outline),
                const SizedBox(height: 12),
                Center(
                  child: Text(
                    controller.error.value.isEmpty ? 'No notifications.' : controller.error.value,
                    style: theme.textTheme.bodyLarge,
                  ),
                ),
              ],
            );
          }
          return ListView.separated(
            padding: const EdgeInsets.symmetric(vertical: 8),
            itemCount: controller.items.length,
            separatorBuilder: (_, _) => const Divider(height: 1),
            itemBuilder: (context, i) {
              final n = controller.items[i];
              return ListTile(
                leading: CircleAvatar(
                  backgroundColor: n.isRead
                      ? theme.colorScheme.surfaceContainerHighest
                      : theme.colorScheme.primaryContainer,
                  child: Icon(_iconFor(n.type),
                      size: 20,
                      color: n.isRead
                          ? theme.colorScheme.onSurfaceVariant
                          : theme.colorScheme.onPrimaryContainer),
                ),
                title: Text(n.title,
                    style: TextStyle(
                        fontWeight: n.isRead ? FontWeight.normal : FontWeight.w600)),
                subtitle: Text(n.body, maxLines: 2, overflow: TextOverflow.ellipsis),
                trailing: Text(timeAgo(n.createdAt),
                    style: theme.textTheme.labelSmall
                        ?.copyWith(color: theme.colorScheme.onSurfaceVariant)),
                onTap: () => controller.markRead(n),
              );
            },
          );
        }),
      ),
    );
  }

  IconData _iconFor(String type) {
    switch (type) {
      case 'TaskAssigned':
        return Icons.assignment_ind_outlined;
      case 'TaskStatusChanged':
        return Icons.swap_horiz;
      case 'CommentMention':
        return Icons.alternate_email;
      case 'CommentAdded':
        return Icons.mode_comment_outlined;
      case 'TaskDueSoon':
        return Icons.schedule;
      default:
        return Icons.notifications_outlined;
    }
  }
}
