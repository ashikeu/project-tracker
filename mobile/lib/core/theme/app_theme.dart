import 'package:flutter/material.dart';

/// A small, consistent Material 3 theme. One seed colour drives both schemes.
class AppTheme {
  AppTheme._();

  static const Color _seed = Color(0xFF4F46E5); // indigo

  static ThemeData get light => _build(Brightness.light);
  static ThemeData get dark => _build(Brightness.dark);

  static ThemeData _build(Brightness brightness) {
    final scheme = ColorScheme.fromSeed(seedColor: _seed, brightness: brightness);
    return ThemeData(
      useMaterial3: true,
      colorScheme: scheme,
      scaffoldBackgroundColor: scheme.surface,
      appBarTheme: AppBarTheme(
        backgroundColor: scheme.surface,
        foregroundColor: scheme.onSurface,
        elevation: 0,
        centerTitle: false,
      ),
      cardTheme: CardThemeData(
        elevation: 0,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
          side: BorderSide(color: scheme.outlineVariant),
        ),
        clipBehavior: Clip.antiAlias,
      ),
      inputDecorationTheme: InputDecorationTheme(
        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
        isDense: true,
      ),
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          minimumSize: const Size.fromHeight(48),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
        ),
      ),
    );
  }

  /// Priority colours (indexes match the backend Priority enum 0..5).
  static const List<Color> priorityColors = [
    Color(0xFF9CA3AF), // Lowest
    Color(0xFF60A5FA), // Low
    Color(0xFF34D399), // Medium
    Color(0xFFFBBF24), // High
    Color(0xFFF97316), // Highest
    Color(0xFFEF4444), // Critical
  ];
}
