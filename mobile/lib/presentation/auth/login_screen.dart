import 'package:flutter/material.dart';
import 'package:get/get.dart';

import 'auth_controller.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _email = TextEditingController(text: 'pm@acme.test');
  final _password = TextEditingController(text: 'Passw0rd!');
  bool _obscure = true;

  late final AuthController _controller = Get.find<AuthController>();

  @override
  void dispose() {
    _email.dispose();
    _password.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Scaffold(
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 420),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Icon(Icons.task_alt, size: 56, color: theme.colorScheme.primary),
                const SizedBox(height: 16),
                Text('Project Tracker',
                    textAlign: TextAlign.center, style: theme.textTheme.headlineSmall),
                const SizedBox(height: 4),
                Text('Sign in to your workspace',
                    textAlign: TextAlign.center,
                    style: theme.textTheme.bodyMedium
                        ?.copyWith(color: theme.colorScheme.onSurfaceVariant)),
                const SizedBox(height: 28),
                TextField(
                  controller: _email,
                  keyboardType: TextInputType.emailAddress,
                  autocorrect: false,
                  decoration: const InputDecoration(
                    labelText: 'Email',
                    prefixIcon: Icon(Icons.mail_outline),
                  ),
                ),
                const SizedBox(height: 12),
                TextField(
                  controller: _password,
                  obscureText: _obscure,
                  onSubmitted: (_) => _submit(),
                  decoration: InputDecoration(
                    labelText: 'Password',
                    prefixIcon: const Icon(Icons.lock_outline),
                    suffixIcon: IconButton(
                      icon: Icon(_obscure ? Icons.visibility_off : Icons.visibility),
                      onPressed: () => setState(() => _obscure = !_obscure),
                    ),
                  ),
                ),
                const SizedBox(height: 8),
                Obx(() => _controller.error.value.isEmpty
                    ? const SizedBox.shrink()
                    : Padding(
                        padding: const EdgeInsets.only(top: 8),
                        child: Text(_controller.error.value,
                            style: TextStyle(color: theme.colorScheme.error)),
                      )),
                const SizedBox(height: 16),
                Obx(() => FilledButton(
                      onPressed: _controller.isLoading.value ? null : _submit,
                      child: _controller.isLoading.value
                          ? const SizedBox(
                              height: 20,
                              width: 20,
                              child: CircularProgressIndicator(strokeWidth: 2),
                            )
                          : const Text('Sign in'),
                    )),
                const SizedBox(height: 12),
                Text('Dev seed: pm@acme.test · Passw0rd!',
                    textAlign: TextAlign.center,
                    style: theme.textTheme.bodySmall
                        ?.copyWith(color: theme.colorScheme.onSurfaceVariant)),
              ],
            ),
          ),
        ),
      ),
    );
  }

  void _submit() => _controller.login(_email.text, _password.text);
}
