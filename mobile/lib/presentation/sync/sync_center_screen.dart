import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../core/database/app_database.dart';
import '../../core/widgets/offline_banner.dart';
import 'sync_center_controller.dart';

class SyncCenterScreen extends StatelessWidget {
  const SyncCenterScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<SyncCenterController>();
    final engine = controller.engine;

    return Scaffold(
      appBar: AppBar(title: const Text('Sync center')),
      body: Column(
        children: [
          const OfflineBanner(),
          Expanded(
            child: ListView(
              padding: const EdgeInsets.all(16),
              children: [
                _StatusCard(controller: controller),
                const SizedBox(height: 20),
                _SectionHeader(
                  icon: Icons.warning_amber_rounded,
                  title: 'Conflicts',
                  countStream: engine.watchConflictCount(),
                ),
                Obx(() {
                  if (controller.conflicts.isEmpty) {
                    return const _EmptyRow(text: 'No conflicts. 🎉');
                  }
                  return Column(
                    children: controller.conflicts
                        .map((c) => _ConflictTile(controller: controller, conflict: c))
                        .toList(),
                  );
                }),
                const SizedBox(height: 20),
                _SectionHeader(
                  icon: Icons.sync_problem,
                  title: 'Failed operations',
                  countStream: engine.watchFailedCount(),
                ),
                Obx(() {
                  if (controller.failed.isEmpty) {
                    return const _EmptyRow(text: 'Nothing failed.');
                  }
                  return Column(
                    children: controller.failed
                        .map((f) => _FailedTile(controller: controller, item: f))
                        .toList(),
                  );
                }),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _StatusCard extends StatelessWidget {
  const _StatusCard({required this.controller});
  final SyncCenterController controller;

  @override
  Widget build(BuildContext context) {
    final engine = controller.engine;
    final theme = Theme.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                StreamBuilder<int>(
                  stream: engine.watchPendingCount(),
                  builder: (_, snap) => Text('${snap.data ?? 0} pending',
                      style: theme.textTheme.titleMedium),
                ),
                const Spacer(),
                Obx(() => FilledButton.icon(
                      onPressed: engine.isSyncing.value ? null : controller.syncNow,
                      icon: engine.isSyncing.value
                          ? const SizedBox(
                              height: 16, width: 16,
                              child: CircularProgressIndicator(strokeWidth: 2))
                          : const Icon(Icons.sync),
                      label: const Text('Sync now'),
                    )),
              ],
            ),
            const SizedBox(height: 8),
            Obx(() {
              final last = engine.lastSyncAt.value;
              final err = engine.lastError.value;
              if (err.isNotEmpty) {
                return Text('Last error: $err',
                    style: TextStyle(color: theme.colorScheme.error));
              }
              return Text(
                last == null ? 'Not synced yet this session.' : 'Last synced: ${last.toLocal()}',
                style: theme.textTheme.bodySmall
                    ?.copyWith(color: theme.colorScheme.onSurfaceVariant),
              );
            }),
          ],
        ),
      ),
    );
  }
}

class _ConflictTile extends StatelessWidget {
  const _ConflictTile({required this.controller, required this.conflict});
  final SyncCenterController controller;
  final ConflictItem conflict;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(controller.titleOf(conflict), style: theme.textTheme.titleSmall),
            const SizedBox(height: 4),
            Text(controller.diffSummary(conflict),
                style: theme.textTheme.bodySmall
                    ?.copyWith(color: theme.colorScheme.onSurfaceVariant)),
            const SizedBox(height: 10),
            Row(
              children: [
                Expanded(
                  child: OutlinedButton(
                    onPressed: () => controller.keepServer(conflict),
                    child: const Text('Keep server'),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: FilledButton(
                    onPressed: () => controller.keepMine(conflict),
                    child: const Text('Keep mine'),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _FailedTile extends StatelessWidget {
  const _FailedTile({required this.controller, required this.item});
  final SyncCenterController controller;
  final SyncQueueItem item;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      child: ListTile(
        leading: Icon(Icons.error_outline, color: theme.colorScheme.error),
        title: Text('${item.changeType} · ${item.entityType}'),
        subtitle: Text(item.lastError ?? 'Rejected',
            maxLines: 2, overflow: TextOverflow.ellipsis),
        trailing: TextButton(
          onPressed: () => controller.retry(item.operationId),
          child: const Text('Retry'),
        ),
      ),
    );
  }
}

class _SectionHeader extends StatelessWidget {
  const _SectionHeader({required this.icon, required this.title, required this.countStream});
  final IconData icon;
  final String title;
  final Stream<int> countStream;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        children: [
          Icon(icon, size: 18, color: theme.colorScheme.onSurfaceVariant),
          const SizedBox(width: 8),
          Text(title, style: theme.textTheme.titleMedium),
          const SizedBox(width: 6),
          StreamBuilder<int>(
            stream: countStream,
            builder: (_, snap) {
              final c = snap.data ?? 0;
              if (c == 0) return const SizedBox.shrink();
              return CircleAvatar(
                radius: 10,
                backgroundColor: theme.colorScheme.error,
                child: Text('$c',
                    style: TextStyle(fontSize: 11, color: theme.colorScheme.onError)),
              );
            },
          ),
        ],
      ),
    );
  }
}

class _EmptyRow extends StatelessWidget {
  const _EmptyRow({required this.text});
  final String text;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: Text(text,
          style: TextStyle(color: Theme.of(context).colorScheme.onSurfaceVariant)),
    );
  }
}
