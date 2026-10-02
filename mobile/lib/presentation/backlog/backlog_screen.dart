import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../data/models/backlog_task_model.dart';
import '../../data/models/sprint_model.dart';
import 'backlog_controller.dart';

class BacklogScreen extends StatelessWidget {
  const BacklogScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<BacklogController>();
    return Scaffold(
      appBar: AppBar(
        title: Text('Backlog · ${controller.projectName}'),
        actions: [
          IconButton(
            tooltip: 'Refresh',
            icon: const Icon(Icons.refresh),
            onPressed: controller.load,
          ),
        ],
      ),
      body: Obx(() {
        if (controller.isLoading.value && controller.items.isEmpty) {
          return const Center(child: CircularProgressIndicator());
        }
        if (controller.items.isEmpty) {
          return Center(
            child: Padding(
              padding: const EdgeInsets.all(24),
              child: Text(
                controller.error.value.isEmpty
                    ? 'The backlog is empty.\nTop-level tasks not yet in a sprint appear here.'
                    : controller.error.value,
                textAlign: TextAlign.center,
              ),
            ),
          );
        }
        return Column(
          children: [
            if (controller.error.value.isNotEmpty)
              _ErrorBar(message: controller.error.value),
            Expanded(
              child: ReorderableListView.builder(
                padding: const EdgeInsets.all(12),
                itemCount: controller.items.length,
                onReorderItem: controller.reorder,
                itemBuilder: (context, i) {
                  final task = controller.items[i];
                  return _BacklogTile(
                    key: ValueKey(task.id),
                    index: i,
                    task: task,
                    sprints: controller.sprints,
                    onAssign: (sprintId) => _assign(context, controller, task, sprintId),
                  );
                },
              ),
            ),
          ],
        );
      }),
    );
  }

  Future<void> _assign(BuildContext context, BacklogController controller,
      BacklogTaskModel task, String? sprintId) async {
    final messenger = ScaffoldMessenger.of(context);
    final ok = await controller.assignToSprint(task.id, sprintId);
    messenger.showSnackBar(SnackBar(
      content: Text(ok ? '${task.taskKey} moved to sprint.' : controller.error.value),
    ));
  }
}

class _ErrorBar extends StatelessWidget {
  const _ErrorBar({required this.message});
  final String message;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Container(
      width: double.infinity,
      color: scheme.errorContainer,
      padding: const EdgeInsets.all(10),
      child: Text(message, style: TextStyle(color: scheme.onErrorContainer)),
    );
  }
}

class _BacklogTile extends StatelessWidget {
  const _BacklogTile({
    required super.key,
    required this.index,
    required this.task,
    required this.sprints,
    required this.onAssign,
  });
  final int index;
  final BacklogTaskModel task;
  final List<SprintModel> sprints;
  final ValueChanged<String?> onAssign;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: ReorderableDragStartListener(
          index: index,
          child: const Icon(Icons.drag_handle),
        ),
        title: Text(task.title, maxLines: 2, overflow: TextOverflow.ellipsis),
        subtitle: Padding(
          padding: const EdgeInsets.only(top: 4),
          child: Wrap(
            spacing: 8,
            runSpacing: 4,
            crossAxisAlignment: WrapCrossAlignment.center,
            children: [
              if (task.taskKey.isNotEmpty)
                Text(task.taskKey,
                    style: theme.textTheme.labelSmall
                        ?.copyWith(fontWeight: FontWeight.w700)),
              _MiniChip(label: task.priority),
              _MiniChip(label: task.taskType),
              if (task.storyPoints != null) _MiniChip(label: '${task.storyPoints} SP'),
              if (task.assigneeName != null) _MiniChip(label: task.assigneeName!),
            ],
          ),
        ),
        trailing: PopupMenuButton<String>(
          icon: const Icon(Icons.playlist_add),
          tooltip: 'Move to sprint',
          onSelected: (v) => onAssign(v == '__none__' ? null : v),
          itemBuilder: (_) => [
            if (sprints.isEmpty)
              const PopupMenuItem(
                enabled: false,
                child: Text('No open sprints'),
              )
            else
              ...sprints.map((s) => PopupMenuItem(
                    value: s.id,
                    child: Text('→ ${s.name}${s.isActive ? " (active)" : ""}'),
                  )),
          ],
        ),
      ),
    );
  }
}

class _MiniChip extends StatelessWidget {
  const _MiniChip({required this.label});
  final String label;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
      decoration: BoxDecoration(
        color: theme.colorScheme.surfaceContainerHighest,
        borderRadius: BorderRadius.circular(6),
      ),
      child: Text(label, style: theme.textTheme.labelSmall),
    );
  }
}
