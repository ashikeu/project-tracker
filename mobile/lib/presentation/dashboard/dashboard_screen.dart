import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../data/models/dashboard_model.dart';
import 'dashboard_controller.dart';

class DashboardScreen extends StatelessWidget {
  const DashboardScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<DashboardController>();

    return Scaffold(
      appBar: AppBar(title: const Text('Dashboard')),
      body: RefreshIndicator(
        onRefresh: controller.load,
        child: Obx(() {
          if (controller.isLoading.value && controller.data.value == null) {
            return const Center(child: CircularProgressIndicator());
          }
          final d = controller.data.value;
          if (d == null) {
            return ListView(children: [
              const SizedBox(height: 120),
              Center(child: Text(controller.error.value.isEmpty
                  ? 'No data.'
                  : controller.error.value)),
            ]);
          }
          return ListView(
            padding: const EdgeInsets.all(16),
            children: [
              Text(d.projectName, style: Theme.of(context).textTheme.titleLarge),
              const SizedBox(height: 12),
              _KpiRow(dashboard: d),
              const SizedBox(height: 20),
              if (d.activeSprint != null) ...[
                _ActiveSprintCard(sprint: d.activeSprint!),
                const SizedBox(height: 20),
              ],
              _DistributionCard(title: 'By status', buckets: d.byStatus),
              const SizedBox(height: 12),
              _DistributionCard(title: 'By priority', buckets: d.byPriority),
              const SizedBox(height: 20),
              if (d.epicProgress.isNotEmpty) ...[
                _EpicProgressCard(rows: d.epicProgress),
                const SizedBox(height: 20),
              ],
              if (d.workload.isNotEmpty) _WorkloadCard(rows: d.workload),
            ],
          );
        }),
      ),
    );
  }
}

class _KpiRow extends StatelessWidget {
  const _KpiRow({required this.dashboard});
  final DashboardModel dashboard;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Row(
      children: [
        _KpiTile(label: 'Total', value: dashboard.totalTasks, color: scheme.primary),
        _KpiTile(label: 'Open', value: dashboard.openTasks, color: scheme.tertiary),
        _KpiTile(label: 'Done', value: dashboard.doneTasks, color: scheme.secondary),
        _KpiTile(label: 'Overdue', value: dashboard.overdueTasks, color: scheme.error),
      ],
    );
  }
}

class _KpiTile extends StatelessWidget {
  const _KpiTile({required this.label, required this.value, required this.color});
  final String label;
  final int value;
  final Color color;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Expanded(
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 4),
        padding: const EdgeInsets.symmetric(vertical: 14),
        decoration: BoxDecoration(
          color: color.withValues(alpha: 0.10),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Column(
          children: [
            Text('$value',
                style: theme.textTheme.headlineSmall?.copyWith(
                    color: color, fontWeight: FontWeight.w700)),
            const SizedBox(height: 2),
            Text(label, style: theme.textTheme.labelSmall),
          ],
        ),
      ),
    );
  }
}

class _DistributionCard extends StatelessWidget {
  const _DistributionCard({required this.title, required this.buckets});
  final String title;
  final List<CountBucket> buckets;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final total = buckets.fold<int>(0, (s, b) => s + b.count);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(title, style: theme.textTheme.titleSmall),
            const SizedBox(height: 12),
            if (buckets.isEmpty)
              Text('No tasks yet.',
                  style: TextStyle(color: theme.colorScheme.onSurfaceVariant))
            else
              ...buckets.map((b) => Padding(
                    padding: const EdgeInsets.only(bottom: 10),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: [
                            Expanded(child: Text(b.key, style: theme.textTheme.bodyMedium)),
                            Text('${b.count}', style: theme.textTheme.labelMedium),
                          ],
                        ),
                        const SizedBox(height: 4),
                        ClipRRect(
                          borderRadius: BorderRadius.circular(4),
                          child: LinearProgressIndicator(
                            value: total == 0 ? 0 : b.count / total,
                            minHeight: 6,
                            backgroundColor: theme.colorScheme.surfaceContainerHighest,
                          ),
                        ),
                      ],
                    ),
                  )),
          ],
        ),
      ),
    );
  }
}

class _EpicProgressCard extends StatelessWidget {
  const _EpicProgressCard({required this.rows});
  final List<EpicProgressRow> rows;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Epic progress', style: theme.textTheme.titleSmall),
            const SizedBox(height: 12),
            ...rows.map((e) => Padding(
                  padding: const EdgeInsets.only(bottom: 10),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          Expanded(
                              child: Text(e.name,
                                  maxLines: 1,
                                  overflow: TextOverflow.ellipsis,
                                  style: theme.textTheme.bodyMedium)),
                          Text('${e.doneTasks}/${e.totalTasks}',
                              style: theme.textTheme.labelMedium),
                        ],
                      ),
                      const SizedBox(height: 4),
                      ClipRRect(
                        borderRadius: BorderRadius.circular(4),
                        child: LinearProgressIndicator(
                          value: e.fraction,
                          minHeight: 6,
                          backgroundColor: theme.colorScheme.surfaceContainerHighest,
                        ),
                      ),
                    ],
                  ),
                )),
          ],
        ),
      ),
    );
  }
}

class _WorkloadCard extends StatelessWidget {
  const _WorkloadCard({required this.rows});
  final List<WorkloadRow> rows;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Workload', style: theme.textTheme.titleSmall),
            const SizedBox(height: 8),
            ...rows.map((w) => ListTile(
                  contentPadding: EdgeInsets.zero,
                  dense: true,
                  leading: CircleAvatar(
                    radius: 16,
                    child: Text(
                      w.userName.isNotEmpty ? w.userName.characters.first.toUpperCase() : '?',
                      style: theme.textTheme.labelMedium,
                    ),
                  ),
                  title: Text(w.userName),
                  trailing: Text('${w.openTaskCount} open · ${w.storyPoints} SP',
                      style: theme.textTheme.labelMedium),
                )),
          ],
        ),
      ),
    );
  }
}

class _ActiveSprintCard extends StatelessWidget {
  const _ActiveSprintCard({required this.sprint});
  final ActiveSprintSummary sprint;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final fraction = sprint.taskCount == 0 ? 0.0 : sprint.doneTaskCount / sprint.taskCount;
    return Card(
      color: theme.colorScheme.primaryContainer,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(Icons.directions_run, color: theme.colorScheme.onPrimaryContainer),
                const SizedBox(width: 8),
                Expanded(
                  child: Text('Active sprint · ${sprint.name}',
                      style: theme.textTheme.titleSmall
                          ?.copyWith(color: theme.colorScheme.onPrimaryContainer)),
                ),
              ],
            ),
            const SizedBox(height: 12),
            ClipRRect(
              borderRadius: BorderRadius.circular(4),
              child: LinearProgressIndicator(
                value: fraction,
                minHeight: 8,
                backgroundColor: theme.colorScheme.onPrimaryContainer.withValues(alpha: 0.2),
              ),
            ),
            const SizedBox(height: 8),
            Text(
              '${sprint.doneTaskCount}/${sprint.taskCount} tasks · '
              '${sprint.completedStoryPoints}/${sprint.committedStoryPoints} SP',
              style: theme.textTheme.bodySmall
                  ?.copyWith(color: theme.colorScheme.onPrimaryContainer),
            ),
          ],
        ),
      ),
    );
  }
}
