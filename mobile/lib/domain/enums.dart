// Client mirrors of the backend enums. Int values MUST match the server
// (they travel as ints in the sync payload).

enum TaskItemStatus {
  backlog(0, 'Backlog'),
  todo(1, 'To Do'),
  inProgress(2, 'In Progress'),
  inReview(3, 'In Review'),
  testing(4, 'Testing'),
  done(5, 'Done');

  const TaskItemStatus(this.value, this.label);
  final int value;
  final String label;

  static TaskItemStatus fromValue(int v) =>
      values.firstWhere((e) => e.value == v, orElse: () => backlog);

  /// Map a board column's StatusMapping string (e.g. "InProgress") to an enum.
  static TaskItemStatus fromMapping(String mapping) {
    final normalized = mapping.replaceAll(' ', '').toLowerCase();
    return values.firstWhere(
      (e) => e.name.toLowerCase() == normalized,
      orElse: () => todo,
    );
  }
}

enum TaskType {
  task(0, 'Task'),
  bug(1, 'Bug'),
  story(2, 'Story'),
  feature(3, 'Feature'),
  improvement(4, 'Improvement'),
  incident(5, 'Incident'),
  request(6, 'Request');

  const TaskType(this.value, this.label);
  final int value;
  final String label;

  static TaskType fromValue(int v) =>
      values.firstWhere((e) => e.value == v, orElse: () => task);
}

enum Priority {
  lowest(0, 'Lowest'),
  low(1, 'Low'),
  medium(2, 'Medium'),
  high(3, 'High'),
  highest(4, 'Highest'),
  critical(5, 'Critical');

  const Priority(this.value, this.label);
  final int value;
  final String label;

  static Priority fromValue(int v) =>
      values.firstWhere((e) => e.value == v, orElse: () => medium);
}
