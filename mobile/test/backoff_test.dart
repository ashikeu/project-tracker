import 'package:flutter_test/flutter_test.dart';
import 'package:project_tracker/data/sync/sync_engine.dart';

void main() {
  test('backoff grows exponentially then caps at the last step', () {
    expect(SyncEngine.backoffFor(0), Duration.zero);
    expect(SyncEngine.backoffFor(1), const Duration(seconds: 5));
    expect(SyncEngine.backoffFor(2), const Duration(seconds: 15));
    expect(SyncEngine.backoffFor(3), const Duration(seconds: 60));
    expect(SyncEngine.backoffFor(4), const Duration(minutes: 5));
    // Beyond the schedule length it stays capped at 5 minutes.
    expect(SyncEngine.backoffFor(5), const Duration(minutes: 5));
    expect(SyncEngine.backoffFor(50), const Duration(minutes: 5));
  });
}
