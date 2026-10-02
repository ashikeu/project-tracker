namespace ProjectTracker.Application.Features.Tasks.Commands.MoveTask;

/// <summary>TargetIndex is the task's desired 0-based position among the OTHER tasks already in
/// TargetColumnId (i.e. excluding the task being moved) — the client computes this from wherever
/// the user dropped it on the board.</summary>
public record MoveTaskCommand(Guid TaskId, Guid TargetColumnId, int TargetIndex);
