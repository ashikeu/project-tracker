import 'package:drift/drift.dart';

import '../../core/database/app_database.dart';
import '../../core/errors/failure.dart';
import '../datasources/remote/project_remote.dart';
import '../models/project_model.dart';

/// Offline-first for *reads*: always serve from the Drift cache, and refresh
/// that cache from the server when connectivity allows. Projects and board
/// columns aren't offline-editable (no sync feed), so creates are online-only.
class ProjectRepository {
  ProjectRepository(this._remote, this._db);

  final ProjectRemote _remote;
  final AppDatabase _db;

  /// Returns cached projects immediately; callers can [refreshProjects] to update.
  Future<List<LocalProject>> cachedProjects() {
    return (_db.select(_db.localProjects)
          ..where((p) => p.isArchived.equals(false))
          ..orderBy([(p) => OrderingTerm(expression: p.name)]))
        .get();
  }

  Future<void> refreshProjects() async {
    final projects = await _remote.getProjects();
    await _db.batch((b) {
      for (final p in projects) {
        b.insert(
          _db.localProjects,
          LocalProjectsCompanion.insert(
            id: p.id,
            key: p.key,
            name: p.name,
            description: Value(p.description),
            status: Value(p.status),
            priority: Value(p.priority),
            isArchived: Value(p.isArchived),
          ),
          onConflict: DoUpdate((_) => LocalProjectsCompanion(
                key: Value(p.key),
                name: Value(p.name),
                description: Value(p.description),
                status: Value(p.status),
                priority: Value(p.priority),
                isArchived: Value(p.isArchived),
              )),
        );
      }
    });
  }

  Future<ProjectModel> createProject({
    required String key,
    required String name,
    String? description,
  }) async {
    final created = await _remote.createProject(key: key, name: name, description: description);
    await _db.into(_db.localProjects).insertOnConflictUpdate(
          LocalProjectsCompanion.insert(
            id: created.id,
            key: created.key,
            name: created.name,
            description: Value(created.description),
            status: Value(created.status),
            priority: Value(created.priority),
          ),
        );
    return created;
  }

  /// Board columns define the Kanban buckets. Cached per project; refreshed online.
  Future<List<LocalBoardColumn>> cachedColumns(String projectId) {
    return (_db.select(_db.localBoardColumns)
          ..where((c) => c.projectId.equals(projectId))
          ..orderBy([(c) => OrderingTerm(expression: c.order)]))
        .get();
  }

  Future<void> refreshColumns(String projectId) async {
    final columns = await _remote.getBoardColumns(projectId);
    await _db.batch((b) {
      for (final c in columns) {
        b.insert(
          _db.localBoardColumns,
          LocalBoardColumnsCompanion.insert(
            id: c.id,
            projectId: c.projectId,
            name: c.name,
            color: Value(c.color),
            order: Value(c.order),
            wipLimit: Value(c.wipLimit),
            statusMapping: Value(c.statusMapping),
            isCompletionColumn: Value(c.isCompletionColumn),
          ),
          onConflict: DoUpdate((_) => LocalBoardColumnsCompanion(
                name: Value(c.name),
                color: Value(c.color),
                order: Value(c.order),
                wipLimit: Value(c.wipLimit),
                statusMapping: Value(c.statusMapping),
                isCompletionColumn: Value(c.isCompletionColumn),
              )),
        );
      }
    });
  }

  /// Best-effort refresh that swallows network errors — the cache stays usable
  /// offline. Non-network failures (e.g. auth) still propagate.
  Future<void> tryRefreshProjects() => _swallowNetwork(refreshProjects());
  Future<void> tryRefreshColumns(String projectId) => _swallowNetwork(refreshColumns(projectId));

  Future<void> _swallowNetwork(Future<void> op) async {
    try {
      await op;
    } on NetworkFailure {
      // Offline — keep serving the cache.
    }
  }
}
