import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:intl/intl.dart';

import '../../data/models/sprint_model.dart';
import 'sprints_controller.dart';

class SprintsScreen extends StatelessWidget {
  const SprintsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<SprintsController>();
    return Scaffold(
      appBar: AppBar(title: Text('Sprints · ${controller.projectName}')),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _openEditor(context, controller),
        icon: const Icon(Icons.add),
        label: const Text('New sprint'),
      ),
      body: RefreshIndicator(
        onRefresh: controller.load,
        child: Obx(() {
          if (controller.isLoading.value && controller.sprints.isEmpty) {
            return const Center(child: CircularProgressIndicator());
          }
          if (controller.sprints.isEmpty) {
            return ListView(children: [
              const SizedBox(height: 120),
              Center(
                child: Text(controller.error.value.isEmpty
                    ? 'No sprints yet. Create one to start planning.'
                    : controller.error.value),
              ),
            ]);
          }
          return ListView(
            padding: const EdgeInsets.all(12),
            children: [
              if (controller.error.value.isNotEmpty)
                _ErrorBanner(message: controller.error.value),
              ...controller.sprints
                  .map((s) => _SprintCard(sprint: s, controller: controller, onEdit: () => _openEditor(context, controller, existing: s))),
            ],
          );
        }),
      ),
    );
  }

  void _openEditor(BuildContext context, SprintsController controller,
      {SprintModel? existing}) {
    showDialog<void>(
      context: context,
      builder: (_) => _SprintEditorDialog(controller: controller, existing: existing),
    );
  }
}

class _ErrorBanner extends StatelessWidget {
  const _ErrorBanner({required this.message});
  final String message;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: scheme.errorContainer,
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(children: [
        Icon(Icons.error_outline, color: scheme.onErrorContainer, size: 18),
        const SizedBox(width: 8),
        Expanded(
            child: Text(message,
                style: TextStyle(color: scheme.onErrorContainer))),
      ]),
    );
  }
}

class _SprintCard extends StatelessWidget {
  const _SprintCard({required this.sprint, required this.controller, required this.onEdit});
  final SprintModel sprint;
  final SprintsController controller;
  final VoidCallback onEdit;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final dates = _dateRange(sprint);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(sprint.name,
                      style: theme.textTheme.titleMedium,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis),
                ),
                _StatusChip(status: sprint.status),
                _SprintMenu(sprint: sprint, controller: controller, onEdit: onEdit),
              ],
            ),
            if (sprint.goal != null && sprint.goal!.isNotEmpty) ...[
              const SizedBox(height: 4),
              Text(sprint.goal!,
                  style: theme.textTheme.bodySmall
                      ?.copyWith(color: theme.colorScheme.onSurfaceVariant)),
            ],
            if (dates != null) ...[
              const SizedBox(height: 6),
              Row(children: [
                Icon(Icons.date_range, size: 14, color: theme.colorScheme.onSurfaceVariant),
                const SizedBox(width: 4),
                Text(dates, style: theme.textTheme.labelSmall),
              ]),
            ],
            const SizedBox(height: 12),
            ClipRRect(
              borderRadius: BorderRadius.circular(4),
              child: LinearProgressIndicator(
                value: sprint.progress,
                minHeight: 6,
                backgroundColor: theme.colorScheme.surfaceContainerHighest,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              '${sprint.completedTaskCount}/${sprint.taskCount} tasks · '
              '${sprint.completedStoryPoints}/${sprint.committedStoryPoints} SP'
              '${sprint.isCompleted ? " · velocity ${sprint.completedStoryPoints}" : ""}',
              style: theme.textTheme.labelMedium
                  ?.copyWith(color: theme.colorScheme.onSurfaceVariant),
            ),
          ],
        ),
      ),
    );
  }

  String? _dateRange(SprintModel s) {
    String fmt(String? iso) {
      if (iso == null) return '';
      final d = DateTime.tryParse(iso);
      return d == null ? '' : DateFormat.yMMMd().format(d);
    }

    final start = fmt(s.startDate);
    final end = fmt(s.endDate);
    if (start.isEmpty && end.isEmpty) return null;
    return '${start.isEmpty ? "?" : start} → ${end.isEmpty ? "?" : end}';
  }
}

class _StatusChip extends StatelessWidget {
  const _StatusChip({required this.status});
  final String status;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final (bg, fg) = switch (status) {
      'Active' => (scheme.primaryContainer, scheme.onPrimaryContainer),
      'Completed' => (scheme.secondaryContainer, scheme.onSecondaryContainer),
      'Cancelled' => (scheme.errorContainer, scheme.onErrorContainer),
      _ => (scheme.surfaceContainerHighest, scheme.onSurfaceVariant),
    };
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(color: bg, borderRadius: BorderRadius.circular(20)),
      child: Text(status,
          style: TextStyle(color: fg, fontSize: 12, fontWeight: FontWeight.w600)),
    );
  }
}

class _SprintMenu extends StatelessWidget {
  const _SprintMenu({required this.sprint, required this.controller, required this.onEdit});
  final SprintModel sprint;
  final SprintsController controller;
  final VoidCallback onEdit;

  @override
  Widget build(BuildContext context) {
    return PopupMenuButton<String>(
      onSelected: (v) => _handle(context, v),
      itemBuilder: (_) => [
        if (sprint.isPlanning) ...[
          const PopupMenuItem(value: 'start', child: Text('Start sprint')),
          const PopupMenuItem(value: 'edit', child: Text('Edit')),
          const PopupMenuItem(value: 'cancel', child: Text('Cancel sprint')),
          const PopupMenuItem(value: 'delete', child: Text('Delete')),
        ],
        if (sprint.isActive) ...[
          const PopupMenuItem(value: 'complete', child: Text('Complete sprint')),
          const PopupMenuItem(value: 'cancel', child: Text('Cancel sprint')),
        ],
        if (sprint.isCompleted || sprint.isCancelled)
          const PopupMenuItem(value: 'delete', child: Text('Delete')),
      ],
    );
  }

  Future<void> _handle(BuildContext context, String action) async {
    final messenger = ScaffoldMessenger.of(context);
    switch (action) {
      case 'start':
        await _do(messenger, () => controller.start(sprint.id), 'Sprint started.');
      case 'edit':
        onEdit();
      case 'complete':
        await _confirmComplete(context, messenger);
      case 'cancel':
        final ok = await _confirm(context, 'Cancel sprint?',
            'All its tasks return to the backlog. This cannot be undone.');
        if (ok) await _do(messenger, () => controller.cancel(sprint.id), 'Sprint cancelled.');
      case 'delete':
        final ok = await _confirm(context, 'Delete sprint?',
            'Only an empty sprint can be deleted.');
        if (ok) await _do(messenger, () => controller.remove(sprint.id), 'Sprint deleted.');
    }
  }

  Future<void> _confirmComplete(BuildContext context, ScaffoldMessengerState messenger) async {
    var moveIncomplete = true;
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (_) => StatefulBuilder(
        builder: (context, setState) => AlertDialog(
          title: const Text('Complete sprint?'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Text('Done tasks stay on the sprint as history.'),
              CheckboxListTile(
                contentPadding: EdgeInsets.zero,
                value: moveIncomplete,
                title: const Text('Move incomplete tasks to backlog'),
                onChanged: (v) => setState(() => moveIncomplete = v ?? true),
              ),
            ],
          ),
          actions: [
            TextButton(onPressed: () => Navigator.pop(context, false), child: const Text('Cancel')),
            FilledButton(onPressed: () => Navigator.pop(context, true), child: const Text('Complete')),
          ],
        ),
      ),
    );
    if (confirmed == true) {
      await _do(messenger, () => controller.complete(sprint.id, moveIncompleteToBacklog: moveIncomplete),
          'Sprint completed.');
    }
  }

  Future<bool> _confirm(BuildContext context, String title, String message) async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(title),
        content: Text(message),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context, false), child: const Text('Back')),
          FilledButton(onPressed: () => Navigator.pop(context, true), child: const Text('Confirm')),
        ],
      ),
    );
    return ok ?? false;
  }

  Future<void> _do(ScaffoldMessengerState messenger, Future<bool> Function() action,
      String successMsg) async {
    final ok = await action();
    messenger.showSnackBar(SnackBar(
      content: Text(ok ? successMsg : controller.error.value),
    ));
  }
}

class _SprintEditorDialog extends StatefulWidget {
  const _SprintEditorDialog({required this.controller, this.existing});
  final SprintsController controller;
  final SprintModel? existing;

  @override
  State<_SprintEditorDialog> createState() => _SprintEditorDialogState();
}

class _SprintEditorDialogState extends State<_SprintEditorDialog> {
  late final TextEditingController _name;
  late final TextEditingController _goal;
  DateTime? _start;
  DateTime? _end;
  bool _saving = false;

  @override
  void initState() {
    super.initState();
    final e = widget.existing;
    _name = TextEditingController(text: e?.name ?? '');
    _goal = TextEditingController(text: e?.goal ?? '');
    _start = e?.startDate == null ? null : DateTime.tryParse(e!.startDate!);
    _end = e?.endDate == null ? null : DateTime.tryParse(e!.endDate!);
  }

  @override
  void dispose() {
    _name.dispose();
    _goal.dispose();
    super.dispose();
  }

  String? _iso(DateTime? d) => d == null ? null : DateFormat('yyyy-MM-dd').format(d);

  @override
  Widget build(BuildContext context) {
    final editing = widget.existing != null;
    return AlertDialog(
      title: Text(editing ? 'Edit sprint' : 'New sprint'),
      content: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: _name,
              autofocus: true,
              decoration: const InputDecoration(labelText: 'Name'),
            ),
            const SizedBox(height: 8),
            TextField(
              controller: _goal,
              decoration: const InputDecoration(labelText: 'Goal (optional)'),
            ),
            const SizedBox(height: 8),
            _DateField(
              label: 'Start date',
              value: _start,
              onPick: (d) => setState(() => _start = d),
              onClear: () => setState(() => _start = null),
            ),
            _DateField(
              label: 'End date',
              value: _end,
              onPick: (d) => setState(() => _end = d),
              onClear: () => setState(() => _end = null),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(onPressed: _saving ? null : () => Navigator.pop(context), child: const Text('Cancel')),
        FilledButton(
          onPressed: _saving ? null : _save,
          child: _saving
              ? const SizedBox(width: 18, height: 18, child: CircularProgressIndicator(strokeWidth: 2))
              : Text(editing ? 'Save' : 'Create'),
        ),
      ],
    );
  }

  Future<void> _save() async {
    final name = _name.text.trim();
    if (name.isEmpty) {
      ScaffoldMessenger.of(context)
          .showSnackBar(const SnackBar(content: Text('Name is required.')));
      return;
    }
    setState(() => _saving = true);
    final goal = _goal.text.trim().isEmpty ? null : _goal.text.trim();
    final ok = widget.existing == null
        ? await widget.controller.create(
            name: name, goal: goal, startDate: _iso(_start), endDate: _iso(_end))
        : await widget.controller.updateSprint(
            id: widget.existing!.id, name: name, goal: goal, startDate: _iso(_start), endDate: _iso(_end));
    if (!mounted) return;
    setState(() => _saving = false);
    if (ok) {
      Navigator.pop(context);
    } else {
      ScaffoldMessenger.of(context)
          .showSnackBar(SnackBar(content: Text(widget.controller.error.value)));
    }
  }
}

class _DateField extends StatelessWidget {
  const _DateField({
    required this.label,
    required this.value,
    required this.onPick,
    required this.onClear,
  });
  final String label;
  final DateTime? value;
  final ValueChanged<DateTime> onPick;
  final VoidCallback onClear;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: () async {
        final picked = await showDatePicker(
          context: context,
          initialDate: value ?? DateTime.now(),
          firstDate: DateTime(2020),
          lastDate: DateTime(2035),
        );
        if (picked != null) onPick(picked);
      },
      child: InputDecorator(
        decoration: InputDecoration(labelText: label),
        child: Row(children: [
          Text(value == null ? 'None' : DateFormat.yMMMd().format(value!)),
          const Spacer(),
          if (value != null)
            IconButton(icon: const Icon(Icons.clear, size: 18), onPressed: onClear),
          const Icon(Icons.calendar_today, size: 18),
        ]),
      ),
    );
  }
}
