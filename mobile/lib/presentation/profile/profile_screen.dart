import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../core/routes/app_routes.dart';
import '../../core/services/connectivity_service.dart';
import '../../data/repositories/auth_repository.dart';
import '../../data/sync/sync_engine.dart';
import '../auth/auth_controller.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final auth = Get.find<AuthRepository>();
    final connectivity = Get.find<ConnectivityService>();
    final sync = Get.find<SyncEngine>();

    return Scaffold(
      appBar: AppBar(title: const Text('Profile')),
      body: FutureBuilder<String?>(
        future: auth.currentUserName(),
        builder: (context, snap) {
          final name = (snap.data ?? '').trim();
          return ListView(
            padding: const EdgeInsets.all(16),
            children: [
              const SizedBox(height: 8),
              Center(
                child: CircleAvatar(
                  radius: 40,
                  backgroundColor: theme.colorScheme.primaryContainer,
                  child: Text(
                    name.isNotEmpty ? name.characters.first.toUpperCase() : '?',
                    style: theme.textTheme.headlineMedium
                        ?.copyWith(color: theme.colorScheme.onPrimaryContainer),
                  ),
                ),
              ),
              const SizedBox(height: 12),
              Center(
                child: Text(name.isEmpty ? 'Signed in' : name,
                    style: theme.textTheme.titleMedium),
              ),
              const SizedBox(height: 24),
              Card(
                child: Column(
                  children: [
                    Obx(() => ListTile(
                          leading: Icon(connectivity.isOnline.value
                              ? Icons.cloud_done_outlined
                              : Icons.cloud_off),
                          title: const Text('Connection'),
                          trailing: Text(connectivity.isOnline.value ? 'Online' : 'Offline'),
                        )),
                    const Divider(height: 1),
                    ListTile(
                      leading: const Icon(Icons.sync),
                      title: const Text('Sync center'),
                      trailing: const Icon(Icons.chevron_right),
                      onTap: () => Get.toNamed(Routes.syncCenter),
                    ),
                    const Divider(height: 1),
                    Obx(() {
                      final last = sync.lastSyncAt.value;
                      return ListTile(
                        leading: const Icon(Icons.schedule),
                        title: const Text('Last synced'),
                        trailing: Text(last == null ? '—' : _fmt(last)),
                      );
                    }),
                  ],
                ),
              ),
              const SizedBox(height: 24),
              FilledButton.tonalIcon(
                style: FilledButton.styleFrom(
                  backgroundColor: theme.colorScheme.errorContainer,
                  foregroundColor: theme.colorScheme.onErrorContainer,
                ),
                onPressed: () => Get.find<AuthController>().logout(),
                icon: const Icon(Icons.logout),
                label: const Text('Sign out'),
              ),
            ],
          );
        },
      ),
    );
  }

  String _fmt(DateTime t) {
    final l = t.toLocal();
    final h = l.hour.toString().padLeft(2, '0');
    final m = l.minute.toString().padLeft(2, '0');
    return '$h:$m';
  }
}
