import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:intl/intl.dart';

import '../../core/database/app_database.dart';
import '../../core/routes/app_routes.dart';
import '../../core/widgets/offline_banner.dart';
import '../../domain/enums.dart';
import 'task_detail_controller.dart';

class TaskDetailScreen extends StatelessWidget {
  const TaskDetailScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<TaskDetailController>();
    return Scaffold(
      appBar: AppBar(
        title: const Text('Task'),
        actions: [
          IconButton(
            tooltip: 'Delete',
            icon: const Icon(Icons.delete_outline),
            onPressed: () => _confirmDelete(controller),
          ),
        ],
      ),
      body: Column(
        children: [
          const OfflineBanner(),
          Expanded(
            child: Obx(() {
              final task = controller.task.value;
              if (task == null) {
                return const Center(child: CircularProgressIndicator());
              }
              return _TaskForm(task: task, controller: controller);
            }),
          ),
        ],
      ),
    );
  }

  void _confirmDelete(TaskDetailController controller) {
    Get.dialog(AlertDialog(
      title: const Text('Delete task?'),
      content: const Text('It will be removed and the deletion synced to the server.'),
      actions: [
        TextButton(onPressed: () => Get.back(), child: const Text('Cancel')),
        FilledButton(
          style: FilledButton.styleFrom(backgroundColor: Get.theme.colorScheme.error),
          onPressed: () {
            Get.back();
            controller.delete();
          },
          child: const Text('Delete'),
        ),
      ],
    ));
  }
}

class _TaskForm extends StatefulWidget {
  const _TaskForm({required this.task, required this.controller});
  final LocalTask task;
  final TaskDetailController controller;

  @override
  State<_TaskForm> createState() => _TaskFormState();
}

class _TaskFormState extends State<_TaskForm> {
  late TextEditingController _title;
  late TextEditingController _description;
  late TextEditingController _points;
  late TaskType _type;
  late TaskItemStatus _status;
  late Priority _priority;
  DateTime? _dueDate;

  @override
  void initState() {
    super.initState();
    _hydrate(widget.task);
  }

  @override
  void didUpdateWidget(covariant _TaskForm oldWidget) {
    super.didUpdateWidget(oldWidget);
    // Only re-hydrate identity-changing fields if the task id changed; keep the
    // user's in-progress text edits otherwise.
    if (oldWidget.task.id != widget.task.id) _hydrate(widget.task);
  }

  void _hydrate(LocalTask t) {
    _title = TextEditingController(text: t.title);
    _description = TextEditingController(text: t.description ?? '');
    _points = TextEditingController(text: t.storyPoints?.toString() ?? '');
    _type = TaskType.fromValue(t.taskType);
    _status = TaskItemStatus.fromValue(t.status);
    _priority = Priority.fromValue(t.priority);
    _dueDate = t.dueDate;
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final t = widget.task;
    final keyLabel = t.taskKey.isNotEmpty ? t.taskKey : 'Not yet assigned (offline-created)';

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        _SyncStateBadge(task: t),
        const SizedBox(height: 12),
        Text(keyLabel, style: theme.textTheme.labelLarge?.copyWith(
            color: theme.colorScheme.onSurfaceVariant)),
        const SizedBox(height: 12),
        TextField(
          controller: _title,
          decoration: const InputDecoration(labelText: 'Title'),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: _description,
          maxLines: 4,
          decoration: const InputDecoration(labelText: 'Description', alignLabelWithHint: true),
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: DropdownButtonFormField<TaskType>(
                initialValue: _type,
                decoration: const InputDecoration(labelText: 'Type'),
                items: TaskType.values
                    .map((e) => DropdownMenuItem(value: e, child: Text(e.label)))
                    .toList(),
                onChanged: (v) => setState(() => _type = v ?? _type),
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: DropdownButtonFormField<Priority>(
                initialValue: _priority,
                decoration: const InputDecoration(labelText: 'Priority'),
                items: Priority.values
                    .map((e) => DropdownMenuItem(value: e, child: Text(e.label)))
                    .toList(),
                onChanged: (v) => setState(() => _priority = v ?? _priority),
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: DropdownButtonFormField<TaskItemStatus>(
                initialValue: _status,
                decoration: const InputDecoration(labelText: 'Status'),
                items: TaskItemStatus.values
                    .map((e) => DropdownMenuItem(value: e, child: Text(e.label)))
                    .toList(),
                onChanged: (v) => setState(() => _status = v ?? _status),
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: TextField(
                controller: _points,
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(labelText: 'Story points'),
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        InkWell(
          onTap: _pickDueDate,
          child: InputDecorator(
            decoration: const InputDecoration(labelText: 'Due date'),
            child: Row(
              children: [
                Text(_dueDate == null ? 'None' : DateFormat.yMMMd().format(_dueDate!)),
                const Spacer(),
                if (_dueDate != null)
                  IconButton(
                    icon: const Icon(Icons.clear, size: 18),
                    onPressed: () => setState(() => _dueDate = null),
                  ),
                const Icon(Icons.calendar_today, size: 18),
              ],
            ),
          ),
        ),
        const SizedBox(height: 24),
        FilledButton.icon(
          onPressed: _save,
          icon: const Icon(Icons.save_outlined),
          label: const Text('Save changes'),
        ),
        const SizedBox(height: 24),
        const Divider(),
        _RelatedSection(task: t),
      ],
    );
  }

  Future<void> _pickDueDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _dueDate ?? DateTime.now(),
      firstDate: DateTime(2020),
      lastDate: DateTime(2035),
    );
    if (picked != null) setState(() => _dueDate = picked);
  }

  void _save() {
    final title = _title.text.trim();
    if (title.isEmpty) {
      ScaffoldMessenger.of(context)
          .showSnackBar(const SnackBar(content: Text('Title is required.')));
      return;
    }
    widget.controller.save(
      title: title,
      description: _description.text.trim().isEmpty ? null : _description.text.trim(),
      taskType: _type.value,
      status: _status.value,
      priority: _priority.value,
      storyPoints: int.tryParse(_points.text.trim()),
      dueDate: _dueDate,
    );
    ScaffoldMessenger.of(context)
        .showSnackBar(const SnackBar(content: Text('Saved locally — syncing…')));
  }

  @override
  void dispose() {
    _title.dispose();
    _description.dispose();
    _points.dispose();
    super.dispose();
  }
}

/// Links to the task's online-only sub-resources (comments, attachments, time).
/// These live only on the server, so they're unavailable until the task has been
/// synced at least once (an offline-created task has no server row yet).
class _RelatedSection extends StatelessWidget {
  const _RelatedSection({required this.task});
  final LocalTask task;

  bool get _serverBacked => task.syncState != 'pendingCreate' && task.taskKey.isNotEmpty;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('More', style: theme.textTheme.titleSmall),
        if (!_serverBacked)
          Padding(
            padding: const EdgeInsets.only(top: 4, bottom: 4),
            child: Text(
              'Sync this task to the server to add comments, attachments or time.',
              style: theme.textTheme.bodySmall
                  ?.copyWith(color: theme.colorScheme.onSurfaceVariant),
            ),
          ),
        _RelatedTile(
          icon: Icons.comment_outlined,
          label: 'Comments',
          enabled: _serverBacked,
          route: Routes.taskComments,
          taskId: task.id,
        ),
        _RelatedTile(
          icon: Icons.attach_file_outlined,
          label: 'Attachments',
          enabled: _serverBacked,
          route: Routes.taskAttachments,
          taskId: task.id,
        ),
        _RelatedTile(
          icon: Icons.schedule_outlined,
          label: 'Time tracking',
          enabled: _serverBacked,
          route: Routes.taskTime,
          taskId: task.id,
        ),
      ],
    );
  }
}

class _RelatedTile extends StatelessWidget {
  const _RelatedTile({
    required this.icon,
    required this.label,
    required this.enabled,
    required this.route,
    required this.taskId,
  });
  final IconData icon;
  final String label;
  final bool enabled;
  final String route;
  final String taskId;

  @override
  Widget build(BuildContext context) {
    return ListTile(
      contentPadding: EdgeInsets.zero,
      enabled: enabled,
      leading: Icon(icon),
      title: Text(label),
      trailing: const Icon(Icons.chevron_right),
      onTap: enabled ? () => Get.toNamed(route, arguments: {'taskId': taskId}) : null,
    );
  }
}

class _SyncStateBadge extends StatelessWidget {
  const _SyncStateBadge({required this.task});
  final LocalTask task;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    late final String label;
    late final Color color;
    late final IconData icon;

    if (task.needsReview) {
      label = 'Conflict — review needed';
      color = theme.colorScheme.error;
      icon = Icons.warning_amber_rounded;
    } else {
      switch (task.syncState) {
        case 'pendingCreate':
          label = 'Pending create (offline)';
          color = theme.colorScheme.tertiary;
          icon = Icons.cloud_upload_outlined;
        case 'pendingUpdate':
          label = 'Pending update (offline)';
          color = theme.colorScheme.tertiary;
          icon = Icons.cloud_upload_outlined;
        case 'pendingDelete':
          label = 'Pending delete';
          color = theme.colorScheme.error;
          icon = Icons.cloud_upload_outlined;
        default:
          label = 'Synced';
          color = theme.colorScheme.primary;
          icon = Icons.cloud_done_outlined;
      }
    }

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 16, color: color),
          const SizedBox(width: 6),
          Text(label, style: theme.textTheme.labelMedium?.copyWith(color: color)),
        ],
      ),
    );
  }
}
