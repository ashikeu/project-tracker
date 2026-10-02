import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../services/connectivity_service.dart';

/// A thin strip shown only while offline, so the user always knows edits are
/// being queued locally rather than hitting the server.
class OfflineBanner extends StatelessWidget {
  const OfflineBanner({super.key});

  @override
  Widget build(BuildContext context) {
    final connectivity = Get.find<ConnectivityService>();
    final theme = Theme.of(context);

    return Obx(() {
      if (connectivity.isOnline.value) return const SizedBox.shrink();
      return Material(
        color: theme.colorScheme.errorContainer,
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 6, horizontal: 12),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(Icons.cloud_off, size: 16, color: theme.colorScheme.onErrorContainer),
              const SizedBox(width: 8),
              Text(
                'Offline — changes are saved locally and will sync when you reconnect.',
                style: theme.textTheme.labelMedium
                    ?.copyWith(color: theme.colorScheme.onErrorContainer),
              ),
            ],
          ),
        ),
      );
    });
  }
}
