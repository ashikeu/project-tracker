import 'dart:async';

import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:get/get.dart';

/// Reactive online/offline state. The UI shows an offline banner from [isOnline],
/// and the SyncEngine listens to [onReconnect] to drain the queue the moment
/// connectivity returns.
class ConnectivityService extends GetxService {
  final Connectivity _connectivity = Connectivity();
  StreamSubscription<List<ConnectivityResult>>? _sub;

  final RxBool isOnline = true.obs;

  /// Fires once each time we transition offline -> online.
  final _reconnectController = StreamController<void>.broadcast();
  Stream<void> get onReconnect => _reconnectController.stream;

  Future<ConnectivityService> init() async {
    final initial = await _connectivity.checkConnectivity();
    isOnline.value = _hasConnection(initial);
    _sub = _connectivity.onConnectivityChanged.listen(_handle);
    return this;
  }

  void _handle(List<ConnectivityResult> results) {
    final wasOnline = isOnline.value;
    final nowOnline = _hasConnection(results);
    isOnline.value = nowOnline;
    if (!wasOnline && nowOnline) {
      _reconnectController.add(null);
    }
  }

  bool _hasConnection(List<ConnectivityResult> results) =>
      results.any((r) => r != ConnectivityResult.none);

  @override
  void onClose() {
    _sub?.cancel();
    _reconnectController.close();
    super.onClose();
  }
}
