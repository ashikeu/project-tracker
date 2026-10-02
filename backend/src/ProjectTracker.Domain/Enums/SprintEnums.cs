namespace ProjectTracker.Domain.Enums;

/// <summary>Lifecycle state of a Sprint. Transitions are guarded (see
/// Domain/BusinessRules/SprintStatusTransition): Planning→Active→Completed, and Planning/Active→
/// Cancelled; Completed and Cancelled are terminal.</summary>
public enum SprintStatus
{
    Planning = 0,
    Active = 1,
    Completed = 2,
    Cancelled = 3
}
