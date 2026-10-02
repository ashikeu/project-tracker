import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '../../core/routes/app_routes.dart';
import '../../data/repositories/auth_repository.dart';

/// Decides the landing route: straight to the board if a session is stored,
/// otherwise the login screen. Works offline — the check is local secure storage.
class SplashScreen extends StatefulWidget {
  const SplashScreen({super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _decide());
  }

  Future<void> _decide() async {
    final hasSession = await Get.find<AuthRepository>().hasSession();
    Get.offAllNamed(hasSession ? Routes.home : Routes.login);
  }

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      body: Center(child: CircularProgressIndicator()),
    );
  }
}
