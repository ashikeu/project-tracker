import 'package:drift/drift.dart';
import 'package:drift_flutter/drift_flutter.dart';

import 'tables.dart';

part 'app_database.g.dart';

/// The app's single Drift database. Local Tasks are the offline-editable source
/// of truth; projects/columns/users are read-only caches; SyncQueueItems is the
/// outbox and ConflictItems the conflict bucket.
@DriftDatabase(
  tables: [
    LocalTasks,
    SyncQueueItems,
    ConflictItems,
    LocalProjects,
    LocalBoardColumns,
    LocalUsers,
    SyncMeta,
  ],
)
class AppDatabase extends _$AppDatabase {
  AppDatabase() : super(_open());

  AppDatabase.forTesting(super.executor);

  @override
  int get schemaVersion => 1;

  static QueryExecutor _open() =>
      driftDatabase(name: 'project_tracker');

  // ---- SyncMeta helpers ----

  static const _kLastSyncVersion = 'lastSyncVersion';

  Future<int> getLastSyncVersion() async {
    final row = await (select(syncMeta)..where((t) => t.key.equals(_kLastSyncVersion)))
        .getSingleOrNull();
    return row == null ? 0 : int.tryParse(row.value) ?? 0;
  }

  Future<void> setLastSyncVersion(int version) => into(syncMeta).insertOnConflictUpdate(
        SyncMetaCompanion.insert(key: _kLastSyncVersion, value: version.toString()),
      );
}
