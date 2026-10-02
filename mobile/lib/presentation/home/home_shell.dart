import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../notifications/notifications_controller.dart';
import '../notifications/notifications_screen.dart';
import '../profile/profile_screen.dart';
import '../projects/projects_screen.dart';

/// Bottom-nav shell hosting the three top-level tabs. Each tab keeps its own
/// Scaffold/AppBar; the shell only owns the NavigationBar and selected index.
class HomeShell extends StatefulWidget {
  const HomeShell({super.key});

  @override
  State<HomeShell> createState() => _HomeShellState();
}

class _HomeShellState extends State<HomeShell> {
  int _index = 0;

  static const _tabs = [ProjectsScreen(), NotificationsScreen(), ProfileScreen()];

  @override
  Widget build(BuildContext context) {
    final notifications = Get.find<NotificationsController>();

    return Scaffold(
      body: IndexedStack(index: _index, children: _tabs),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _index,
        onDestinationSelected: (i) {
          setState(() => _index = i);
          if (i == 1) notifications.load(); // refresh on entering the tab
        },
        destinations: [
          const NavigationDestination(
            icon: Icon(Icons.folder_outlined),
            selectedIcon: Icon(Icons.folder),
            label: 'Projects',
          ),
          NavigationDestination(
            icon: Obx(() => _BadgeIcon(
                  icon: Icons.notifications_outlined,
                  count: notifications.unread.value,
                )),
            selectedIcon: Obx(() => _BadgeIcon(
                  icon: Icons.notifications,
                  count: notifications.unread.value,
                )),
            label: 'Alerts',
          ),
          const NavigationDestination(
            icon: Icon(Icons.person_outline),
            selectedIcon: Icon(Icons.person),
            label: 'Profile',
          ),
        ],
      ),
    );
  }
}

class _BadgeIcon extends StatelessWidget {
  const _BadgeIcon({required this.icon, required this.count});
  final IconData icon;
  final int count;

  @override
  Widget build(BuildContext context) {
    if (count == 0) return Icon(icon);
    return Badge(
      label: Text(count > 99 ? '99+' : '$count'),
      child: Icon(icon),
    );
  }
}
