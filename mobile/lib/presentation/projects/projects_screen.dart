import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../core/routes/app_routes.dart';
import '../../core/widgets/sync_status_chip.dart';
import '../auth/auth_controller.dart';
import 'projects_controller.dart';

class ProjectsScreen extends StatelessWidget {
  const ProjectsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<ProjectsController>();
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Projects'),
        actions: [
          const SyncStatusChip(),
          PopupMenuButton<String>(
            onSelected: (v) {
              if (v == 'logout') Get.find<AuthController>().logout();
              if (v == 'sync') Get.toNamed(Routes.syncCenter);
            },
            itemBuilder: (_) => [
              const PopupMenuItem(value: 'sync', child: Text('Sync center')),
              const PopupMenuItem(value: 'logout', child: Text('Sign out')),
            ],
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _showCreateDialog(context, controller),
        icon: const Icon(Icons.add),
        label: const Text('New project'),
      ),
      body: RefreshIndicator(
        onRefresh: controller.refreshList,
        child: Obx(() {
          if (controller.isLoading.value && controller.projects.isEmpty) {
            return const Center(child: CircularProgressIndicator());
          }
          if (controller.projects.isEmpty) {
            return _EmptyState(error: controller.error.value);
          }
          return ListView.separated(
            padding: const EdgeInsets.all(16),
            itemCount: controller.projects.length,
            separatorBuilder: (_, _) => const SizedBox(height: 10),
            itemBuilder: (context, i) {
              final p = controller.projects[i];
              return Card(
                child: ListTile(
                  leading: CircleAvatar(
                    backgroundColor: theme.colorScheme.primaryContainer,
                    child: Text(
                      p.key.isNotEmpty ? p.key.characters.first : '?',
                      style: TextStyle(color: theme.colorScheme.onPrimaryContainer),
                    ),
                  ),
                  title: Text(p.name),
                  subtitle: Text('${p.key} · ${p.status}'),
                  trailing: const Icon(Icons.chevron_right),
                  onTap: () => Get.toNamed(Routes.board, arguments: {
                    'projectId': p.id,
                    'projectName': p.name,
                    'projectKey': p.key,
                  }),
                ),
              );
            },
          );
        }),
      ),
    );
  }

  void _showCreateDialog(BuildContext context, ProjectsController controller) {
    final keyCtrl = TextEditingController();
    final nameCtrl = TextEditingController();
    final descCtrl = TextEditingController();
    final errorText = ''.obs;
    final busy = false.obs;

    Get.dialog(
      AlertDialog(
        title: const Text('New project'),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: keyCtrl,
                textCapitalization: TextCapitalization.characters,
                decoration: const InputDecoration(
                  labelText: 'Key (e.g. ERP)',
                  helperText: 'Short, unique per organization',
                ),
              ),
              const SizedBox(height: 10),
              TextField(
                controller: nameCtrl,
                decoration: const InputDecoration(labelText: 'Name'),
              ),
              const SizedBox(height: 10),
              TextField(
                controller: descCtrl,
                maxLines: 2,
                decoration: const InputDecoration(labelText: 'Description (optional)'),
              ),
              const SizedBox(height: 8),
              Obx(() => errorText.value.isEmpty
                  ? const SizedBox.shrink()
                  : Text(errorText.value,
                      style: TextStyle(color: Theme.of(context).colorScheme.error))),
            ],
          ),
        ),
        actions: [
          TextButton(onPressed: () => Get.back(), child: const Text('Cancel')),
          Obx(() => FilledButton(
                onPressed: busy.value
                    ? null
                    : () async {
                        busy.value = true;
                        errorText.value = '';
                        final err = await controller.createProject(
                          key: keyCtrl.text.trim(),
                          name: nameCtrl.text.trim(),
                          description: descCtrl.text.trim(),
                        );
                        busy.value = false;
                        if (err == null) {
                          Get.back();
                        } else {
                          errorText.value = err;
                        }
                      },
                child: busy.value
                    ? const SizedBox(
                        height: 18, width: 18, child: CircularProgressIndicator(strokeWidth: 2))
                    : const Text('Create'),
              )),
        ],
      ),
    );
  }
}

class _EmptyState extends StatelessWidget {
  const _EmptyState({required this.error});
  final String error;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return ListView(
      children: [
        const SizedBox(height: 120),
        Icon(Icons.folder_open, size: 64, color: theme.colorScheme.outline),
        const SizedBox(height: 16),
        Center(
          child: Text(
            error.isEmpty ? 'No projects yet.\nCreate your first one.' : error,
            textAlign: TextAlign.center,
            style: theme.textTheme.bodyLarge,
          ),
        ),
      ],
    );
  }
}
