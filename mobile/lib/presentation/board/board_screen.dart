import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../core/database/app_database.dart';
import '../../core/routes/app_routes.dart';
import '../../core/utils/color_utils.dart';
import '../../core/widgets/offline_banner.dart';
import '../../core/widgets/sync_status_chip.dart';
import 'board_controller.dart';
import 'widgets/task_card.dart';

class BoardScreen extends StatelessWidget {
  const BoardScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<BoardController>();

    return Scaffold(
      appBar: AppBar(
        title: Obx(() => Text(controller.projectName)),
        actions: [
          IconButton(
            tooltip: 'Dashboard',
            icon: const Icon(Icons.insights_outlined),
            onPressed: () => Get.toNamed(Routes.dashboard,
                arguments: {'projectId': controller.projectId}),
          ),
          PopupMenuButton<String>(
            tooltip: 'More',
            onSelected: (v) {
              final args = {
                'projectId': controller.projectId,
                'projectName': controller.projectName,
              };
              if (v == 'backlog') Get.toNamed(Routes.backlog, arguments: args);
              if (v == 'sprints') Get.toNamed(Routes.sprints, arguments: args);
            },
            itemBuilder: (_) => const [
              PopupMenuItem(value: 'backlog', child: ListTile(
                  leading: Icon(Icons.list_alt), title: Text('Backlog'), contentPadding: EdgeInsets.zero)),
              PopupMenuItem(value: 'sprints', child: ListTile(
                  leading: Icon(Icons.directions_run), title: Text('Sprints'), contentPadding: EdgeInsets.zero)),
            ],
          ),
          const SyncStatusChip(),
          const SizedBox(width: 4),
        ],
      ),
      body: Column(
        children: [
          const OfflineBanner(),
          Expanded(
            child: Obx(() {
              if (controller.isLoading.value) {
                return const Center(child: CircularProgressIndicator());
              }
              if (controller.columns.isEmpty) {
                return const Center(
                  child: Padding(
                    padding: EdgeInsets.all(24),
                    child: Text(
                      'No board columns cached yet.\nConnect once to load the board configuration.',
                      textAlign: TextAlign.center,
                    ),
                  ),
                );
              }
              return ListView.builder(
                scrollDirection: Axis.horizontal,
                padding: const EdgeInsets.all(12),
                itemCount: controller.columns.length,
                itemBuilder: (context, i) =>
                    _BoardColumnView(column: controller.columns[i], controller: controller),
              );
            }),
          ),
        ],
      ),
    );
  }
}

class _BoardColumnView extends StatelessWidget {
  const _BoardColumnView({required this.column, required this.controller});

  final LocalBoardColumn column;
  final BoardController controller;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final accent = hexToColor(column.color);

    return Container(
      width: 280,
      margin: const EdgeInsets.only(right: 12),
      decoration: BoxDecoration(
        color: theme.colorScheme.surfaceContainerLow,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: theme.colorScheme.outlineVariant),
      ),
      child: DragTarget<String>(
        onWillAcceptWithDetails: (_) => true,
        onAcceptWithDetails: (details) =>
            controller.moveTaskToColumn(details.data, column),
        builder: (context, candidate, rejected) {
          final highlighted = candidate.isNotEmpty;
          return Obx(() {
            final items = controller.tasksForColumn(column);
            final wip = column.wipLimit;
            final overWip = wip != null && items.length > wip;
            return Container(
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(14),
                color: highlighted ? accent.withValues(alpha: 0.08) : null,
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(12, 12, 8, 8),
                    child: Row(
                      children: [
                        Container(width: 10, height: 10,
                            decoration: BoxDecoration(color: accent, shape: BoxShape.circle)),
                        const SizedBox(width: 8),
                        Expanded(
                          child: Text(column.name,
                              style: theme.textTheme.titleSmall,
                              overflow: TextOverflow.ellipsis),
                        ),
                        Text(
                          wip == null ? '${items.length}' : '${items.length}/$wip',
                          style: theme.textTheme.labelMedium?.copyWith(
                            color: overWip
                                ? theme.colorScheme.error
                                : theme.colorScheme.onSurfaceVariant,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ),
                  ),
                  Expanded(
                    child: items.isEmpty
                        ? _DropHint(highlighted: highlighted)
                        : ListView.builder(
                            padding: const EdgeInsets.symmetric(horizontal: 8),
                            itemCount: items.length,
                            itemBuilder: (context, i) {
                              final task = items[i];
                              return LongPressDraggable<String>(
                                data: task.id,
                                feedback: Material(
                                  color: Colors.transparent,
                                  child: SizedBox(width: 260, child: TaskCard(task: task, dense: true)),
                                ),
                                childWhenDragging:
                                    Opacity(opacity: 0.4, child: TaskCard(task: task)),
                                child: TaskCard(
                                  task: task,
                                  onTap: () => Get.toNamed(Routes.taskDetail,
                                      arguments: {'taskId': task.id}),
                                ),
                              );
                            },
                          ),
                  ),
                  TextButton.icon(
                    onPressed: () => _addTask(context),
                    icon: const Icon(Icons.add, size: 18),
                    label: const Text('Add task'),
                  ),
                ],
              ),
            );
          });
        },
      ),
    );
  }

  void _addTask(BuildContext context) {
    final titleCtrl = TextEditingController();
    Get.dialog(
      AlertDialog(
        title: Text('Add task to ${column.name}'),
        content: TextField(
          controller: titleCtrl,
          autofocus: true,
          decoration: const InputDecoration(labelText: 'Title'),
          onSubmitted: (_) => _submit(titleCtrl),
        ),
        actions: [
          TextButton(onPressed: () => Get.back(), child: const Text('Cancel')),
          FilledButton(onPressed: () => _submit(titleCtrl), child: const Text('Add')),
        ],
      ),
    );
  }

  void _submit(TextEditingController ctrl) {
    final title = ctrl.text.trim();
    if (title.isEmpty) return;
    controller.createTask(column, title);
    Get.back();
  }
}

class _DropHint extends StatelessWidget {
  const _DropHint({required this.highlighted});
  final bool highlighted;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Center(
      child: Text(
        highlighted ? 'Drop here' : 'No tasks',
        style: theme.textTheme.bodySmall?.copyWith(color: theme.colorScheme.outline),
      ),
    );
  }
}
