import 'package:flutter/material.dart';

/// Parses a '#RRGGBB' (or '#AARRGGBB') hex string to a [Color], falling back to
/// grey for anything malformed.
Color hexToColor(String hex, {Color fallback = const Color(0xFF6B7280)}) {
  var value = hex.trim().replaceAll('#', '');
  if (value.length == 6) value = 'FF$value';
  final parsed = int.tryParse(value, radix: 16);
  return parsed == null ? fallback : Color(parsed);
}
