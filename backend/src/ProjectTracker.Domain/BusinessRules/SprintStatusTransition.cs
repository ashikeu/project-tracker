using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.BusinessRules;

/// <summary>Business-rule validation for the Sprint lifecycle state machine. Lives in Domain (per
/// CLAUDE.md: business-rule validation is Domain rule classes, invoked from Application handlers,
/// never controllers). Pure — no persistence — so it stays trivially testable.</summary>
public static class SprintStatusTransition
{
    /// <summary>The set of states each status may move to. Completed and Cancelled are terminal.</summary>
    private static readonly IReadOnlyDictionary<SprintStatus, SprintStatus[]> Allowed =
        new Dictionary<SprintStatus, SprintStatus[]>
        {
            [SprintStatus.Planning] = new[] { SprintStatus.Active, SprintStatus.Cancelled },
            [SprintStatus.Active] = new[] { SprintStatus.Completed, SprintStatus.Cancelled },
            [SprintStatus.Completed] = Array.Empty<SprintStatus>(),
            [SprintStatus.Cancelled] = Array.Empty<SprintStatus>()
        };

    public static bool CanTransition(SprintStatus from, SprintStatus to) =>
        Allowed.TryGetValue(from, out var targets) && Array.IndexOf(targets, to) >= 0;

    /// <summary>Human-readable reason a transition is rejected, for surfacing in an API error.</summary>
    public static string DescribeInvalid(SprintStatus from, SprintStatus to) =>
        from == to
            ? $"Sprint is already {from}."
            : $"Cannot move a sprint from {from} to {to}.";
}
