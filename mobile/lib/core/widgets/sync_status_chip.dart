import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../data/sync/sync_engine.dart';
import '../routes/app_routes.dart';
import '../services/connectivity_service.dart';

/// Compact app-bar indicator: offline state, in-flight sync, and the count of
/// unsynced/failed operations. Tapping opens the Sync Center.
class SyncStatusChip extends StatelessWidget {
  const SyncStatusChip({super.key});

  @override
  Widget build(BuildContext context) {
    final connectivity = Get.find<ConnectivityService>();
    final sync = Get.find<SyncEngine>();
    final theme = Theme.of(context);

    return StreamBuilder<int>(
      stream: sync.watchPendingCount(),
      builder: (context, pendingSnap) {
        return StreamBuilder<int>(
          stream: sync.watchFailedCount(),
          builder: (context, failedSnap) {
            final pending = pendingSnap.data ?? 0;
            final failed = failedSnap.data ?? 0;

            return Obx(() {
              final online = connectivity.isOnline.value;
              final syncing = sync.isSyncing.value;

              late final IconData icon;
              late final Color color;
              late final String label;

              if (!online) {
                icon = Icons.cloud_off;
                color = theme.colorScheme.error;
                label = 'Offline';
              } else if (syncing) {
                icon = Icons.sync;
                color = theme.colorScheme.primary;
                label = 'Syncing';
              } else if (failed > 0) {
                icon = Icons.sync_problem;
                color = theme.colorScheme.error;
                label = '$failed failed';
              } else if (pending > 0) {
                icon = Icons.cloud_upload_outlined;
                color = theme.colorScheme.tertiary;
                label = '$pending pending';
              } else {
                icon = Icons.cloud_done_outlined;
                color = theme.colorScheme.primary;
                label = 'Synced';
              }

              return InkWell(
                borderRadius: BorderRadius.circular(20),
                onTap: () => Get.toNamed(Routes.syncCenter),
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      syncing
                          ? SizedBox(
                              height: 16,
                              width: 16,
                              child: CircularProgressIndicator(strokeWidth: 2, color: color),
                            )
                          : Icon(icon, size: 18, color: color),
                      const SizedBox(width: 6),
                      Text(label, style: theme.textTheme.labelMedium?.copyWith(color: color)),
                    ],
                  ),
                ),
              );
            });
          },
        );
      },
    );
  }
}
