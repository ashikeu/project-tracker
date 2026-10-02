import 'package:flutter/material.dart';
import 'package:get/get.dart';

import 'core/di/injector.dart';
import 'core/routes/app_pages.dart';
import 'core/routes/app_routes.dart';
import 'core/theme/app_theme.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await setupDependencies();
  runApp(const ProjectTrackerApp());
}

class ProjectTrackerApp extends StatelessWidget {
  const ProjectTrackerApp({super.key});

  @override
  Widget build(BuildContext context) {
    return GetMaterialApp(
      title: 'Project Tracker',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.light,
      darkTheme: AppTheme.dark,
      themeMode: ThemeMode.system,
      initialRoute: Routes.splash,
      getPages: AppPages.routes,
    );
  }
}
