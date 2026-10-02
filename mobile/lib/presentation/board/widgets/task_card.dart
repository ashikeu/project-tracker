import 'package:flutter/material.dart';

import '../../../core/database/app_database.dart';
import '../../../core/theme/app_theme.dart';
import '../../../domain/enums.dart';

/// A single task tile on the board. Shows the task key (or a "not synced" hint
/// while offline-created), title, priority stripe, type, and an unsynced/review
/// badge so the offline state is always visible.
class TaskCard extends StatelessWidget {
  const TaskCard({super.key, required this.task, this.onTap, this.dense = false});

  final LocalTask task;
  final VoidCallback? onTap;
  final bool dense;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final priority = Priority.fromValue(task.priority);
    final type = TaskType.fromValue(task.taskType);
    final unsynced = task.syncState != 'synced';
    final keyLabel = task.taskKey.isNotEmpty ? task.taskKey : 'NEW';

    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: InkWell(
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(10),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Container(
                    width: 8,
                    height: 8,
                    decoration: BoxDecoration(
                      color: AppTheme.priorityColors[priority.value],
                      shape: BoxShape.circle,
                    ),
                  ),
                  const SizedBox(width: 6),
                  Text(keyLabel,
                      style: theme.textTheme.labelSmall?.copyWith(
                        color: theme.colorScheme.onSurfaceVariant,
                        fontWeight: FontWeight.w600,
                      )),
                  const Spacer(),
                  if (task.needsReview)
                    Icon(Icons.warning_amber_rounded,
                        size: 15, color: theme.colorScheme.error)
                  else if (unsynced)
                    Icon(Icons.cloud_upload_outlined,
                        size: 15, color: theme.colorScheme.tertiary),
                ],
              ),
              const SizedBox(height: 6),
              Text(task.title,
                  maxLines: dense ? 2 : 3,
                  overflow: TextOverflow.ellipsis,
                  style: theme.textTheme.bodyMedium),
              const SizedBox(height: 8),
              Row(
                children: [
                  _Chip(label: type.label, color: theme.colorScheme.secondaryContainer,
                      onColor: theme.colorScheme.onSecondaryContainer),
                  const Spacer(),
                  if (task.storyPoints != null)
                    CircleAvatar(
                      radius: 10,
                      backgroundColor: theme.colorScheme.surfaceContainerHighest,
                      child: Text('${task.storyPoints}',
                          style: theme.textTheme.labelSmall),
                    ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _Chip extends StatelessWidget {
  const _Chip({required this.label, required this.color, required this.onColor});
  final String label;
  final Color color;
  final Color onColor;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
      decoration: BoxDecoration(color: color, borderRadius: BorderRadius.circular(6)),
      child: Text(label,
          style: Theme.of(context)
              .textTheme
              .labelSmall
              ?.copyWith(color: onColor, fontWeight: FontWeight.w500)),
    );
  }
}
