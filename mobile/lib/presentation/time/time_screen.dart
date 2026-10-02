import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:intl/intl.dart';

import '../../data/models/time_entry_model.dart';
import 'time_controller.dart';

class TimeScreen extends StatelessWidget {
  const TimeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<TimeController>();
    return Scaffold(
      appBar: AppBar(title: const Text('Time tracking')),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _logManual(context, controller),
        icon: const Icon(Icons.add),
        label: const Text('Log time'),
      ),
      body: RefreshIndicator(
        onRefresh: controller.load,
        child: Obx(() {
          if (controller.isLoading.value && controller.entries.isEmpty) {
            return const Center(child: CircularProgressIndicator());
          }
          return ListView(
            padding: const EdgeInsets.all(16),
            children: [
              if (controller.error.value.isNotEmpty) ...[
                _ErrorText(message: controller.error.value),
                const SizedBox(height: 12),
              ],
              _SummaryCard(summary: controller.summary.value),
              const SizedBox(height: 12),
              _TimerCard(controller: controller),
              const SizedBox(height: 20),
              Text('Entries', style: Theme.of(context).textTheme.titleSmall),
              const SizedBox(height: 8),
              if (controller.entries.isEmpty)
                const Padding(
                  padding: EdgeInsets.symmetric(vertical: 24),
                  child: Center(child: Text('No time logged yet.')),
                )
              else
                ...controller.entries.map((e) => _EntryTile(
                      entry: e,
                      canEdit: controller.isMine(e),
                      onEdit: () => _edit(context, controller, e),
                      onDelete: () => _delete(context, controller, e),
                    )),
            ],
          );
        }),
      ),
    );
  }

  Future<void> _logManual(BuildContext context, TimeController controller) async {
    final messenger = ScaffoldMessenger.of(context);
    final result = await showDialog<_ManualEntry>(
      context: context,
      builder: (_) => const _ManualEntryDialog(),
    );
    if (result == null) return;
    final ok = await controller.logManual(
      date: result.date,
      durationMinutes: result.minutes,
      isBillable: result.billable,
      description: result.description,
    );
    if (!ok) messenger.showSnackBar(SnackBar(content: Text(controller.error.value)));
  }

  Future<void> _edit(BuildContext context, TimeController controller, TimeEntryModel e) async {
    final messenger = ScaffoldMessenger.of(context);
    final result = await showDialog<_ManualEntry>(
      context: context,
      builder: (_) => _ManualEntryDialog(existing: e),
    );
    if (result == null) return;
    final ok = await controller.editEntry(
      e.id,
      date: result.date,
      durationMinutes: result.minutes,
      isBillable: result.billable,
      description: result.description,
    );
    if (!ok) messenger.showSnackBar(SnackBar(content: Text(controller.error.value)));
  }

  Future<void> _delete(BuildContext context, TimeController controller, TimeEntryModel e) async {
    final messenger = ScaffoldMessenger.of(context);
    final ok = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text('Delete entry?'),
        content: const Text('This removes the logged time from the task.'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context, false), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.pop(context, true), child: const Text('Delete')),
        ],
      ),
    );
    if (ok == true) {
      final done = await controller.removeEntry(e.id);
      if (!done) messenger.showSnackBar(SnackBar(content: Text(controller.error.value)));
    }
  }
}

class _ErrorText extends StatelessWidget {
  const _ErrorText({required this.message});
  final String message;
  @override
  Widget build(BuildContext context) => Text(message,
      style: TextStyle(color: Theme.of(context).colorScheme.error));
}

class _SummaryCard extends StatelessWidget {
  const _SummaryCard({required this.summary});
  final TaskTimeSummaryModel? summary;

  @override
  Widget build(BuildContext context) {
    final s = summary;
    String h(double? v) => v == null ? '—' : '${v.toStringAsFixed(v == v.roundToDouble() ? 0 : 1)}h';
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceAround,
          children: [
            _Metric(label: 'Estimated', value: h(s?.estimatedHours)),
            _Metric(label: 'Logged', value: h(s?.loggedHours)),
            _Metric(label: 'Remaining', value: h(s?.remainingHours)),
          ],
        ),
      ),
    );
  }
}

class _Metric extends StatelessWidget {
  const _Metric({required this.label, required this.value});
  final String label;
  final String value;
  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Column(
      children: [
        Text(value, style: theme.textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
        const SizedBox(height: 2),
        Text(label, style: theme.textTheme.labelSmall),
      ],
    );
  }
}

class _TimerCard extends StatelessWidget {
  const _TimerCard({required this.controller});
  final TimeController controller;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    // Self-observing: reading entries/currentUserId here (not in the parent Obx's
    // synchronous build) means this card must track them itself.
    return Obx(() {
      final running = controller.myRunningTimer;
      return Card(
        color: running != null ? theme.colorScheme.tertiaryContainer : null,
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              Icon(running != null ? Icons.timer : Icons.timer_outlined,
                  color: running != null ? theme.colorScheme.onTertiaryContainer : null),
              const SizedBox(width: 12),
              Expanded(
                child: Text(
                  running != null
                      ? 'Timer running since ${running.startedAt != null ? DateFormat.jm().format(running.startedAt!.toLocal()) : "now"}'
                      : 'No timer running',
                  style: theme.textTheme.bodyMedium?.copyWith(
                      color: running != null ? theme.colorScheme.onTertiaryContainer : null),
                ),
              ),
              running != null
                  ? FilledButton.icon(
                      onPressed: controller.busy.value ? null : () => controller.stopTimer(),
                      icon: const Icon(Icons.stop),
                      label: const Text('Stop'),
                    )
                  : FilledButton.icon(
                      onPressed: controller.busy.value ? null : () => controller.startTimer(),
                      icon: const Icon(Icons.play_arrow),
                      label: const Text('Start'),
                    ),
            ],
          ),
        ),
      );
    });
  }
}

class _EntryTile extends StatelessWidget {
  const _EntryTile({
    required this.entry,
    required this.canEdit,
    required this.onEdit,
    required this.onDelete,
  });
  final TimeEntryModel entry;
  final bool canEdit;
  final VoidCallback onEdit;
  final VoidCallback onDelete;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final subtitleParts = [
      entry.date,
      entry.userName,
      if (entry.isBillable) 'billable',
      if (entry.description != null && entry.description!.isNotEmpty) entry.description!,
    ];
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: entry.isRunningTimer
            ? Icon(Icons.timer, color: theme.colorScheme.tertiary)
            : const Icon(Icons.schedule),
        title: Text(entry.isRunningTimer ? 'Running…' : entry.durationLabel,
            style: theme.textTheme.titleSmall),
        subtitle: Text(subtitleParts.join(' · '),
            maxLines: 2, overflow: TextOverflow.ellipsis),
        trailing: canEdit && !entry.isRunningTimer
            ? PopupMenuButton<String>(
                onSelected: (v) => v == 'edit' ? onEdit() : onDelete(),
                itemBuilder: (_) => const [
                  PopupMenuItem(value: 'edit', child: Text('Edit')),
                  PopupMenuItem(value: 'delete', child: Text('Delete')),
                ],
              )
            : null,
      ),
    );
  }
}

/// Result of the manual-entry dialog.
class _ManualEntry {
  final String date; // yyyy-MM-dd
  final int minutes;
  final bool billable;
  final String? description;
  _ManualEntry(this.date, this.minutes, this.billable, this.description);
}

class _ManualEntryDialog extends StatefulWidget {
  const _ManualEntryDialog({this.existing});
  final TimeEntryModel? existing;

  @override
  State<_ManualEntryDialog> createState() => _ManualEntryDialogState();
}

class _ManualEntryDialogState extends State<_ManualEntryDialog> {
  late DateTime _date;
  late final TextEditingController _hours;
  late final TextEditingController _minutes;
  late final TextEditingController _description;
  bool _billable = false;

  @override
  void initState() {
    super.initState();
    final e = widget.existing;
    _date = e == null ? DateTime.now() : (DateTime.tryParse(e.date) ?? DateTime.now());
    _hours = TextEditingController(text: e == null ? '' : '${e.durationMinutes ~/ 60}');
    _minutes = TextEditingController(text: e == null ? '' : '${e.durationMinutes % 60}');
    _description = TextEditingController(text: e?.description ?? '');
    _billable = e?.isBillable ?? false;
  }

  @override
  void dispose() {
    _hours.dispose();
    _minutes.dispose();
    _description.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(widget.existing == null ? 'Log time' : 'Edit entry'),
      content: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            InkWell(
              onTap: () async {
                final picked = await showDatePicker(
                  context: context,
                  initialDate: _date,
                  firstDate: DateTime(2020),
                  lastDate: DateTime(2035),
                );
                if (picked != null) setState(() => _date = picked);
              },
              child: InputDecorator(
                decoration: const InputDecoration(labelText: 'Date'),
                child: Row(children: [
                  Text(DateFormat.yMMMd().format(_date)),
                  const Spacer(),
                  const Icon(Icons.calendar_today, size: 18),
                ]),
              ),
            ),
            Row(children: [
              Expanded(
                child: TextField(
                  controller: _hours,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(labelText: 'Hours'),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: TextField(
                  controller: _minutes,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(labelText: 'Minutes'),
                ),
              ),
            ]),
            TextField(
              controller: _description,
              decoration: const InputDecoration(labelText: 'Description (optional)'),
            ),
            CheckboxListTile(
              contentPadding: EdgeInsets.zero,
              value: _billable,
              title: const Text('Billable'),
              onChanged: (v) => setState(() => _billable = v ?? false),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(onPressed: () => Navigator.pop(context), child: const Text('Cancel')),
        FilledButton(onPressed: _submit, child: const Text('Save')),
      ],
    );
  }

  void _submit() {
    final h = int.tryParse(_hours.text.trim()) ?? 0;
    final m = int.tryParse(_minutes.text.trim()) ?? 0;
    final total = h * 60 + m;
    if (total <= 0) {
      ScaffoldMessenger.of(context)
          .showSnackBar(const SnackBar(content: Text('Enter a duration greater than zero.')));
      return;
    }
    Navigator.pop(
      context,
      _ManualEntry(
        DateFormat('yyyy-MM-dd').format(_date),
        total,
        _billable,
        _description.text.trim().isEmpty ? null : _description.text.trim(),
      ),
    );
  }
}
