using ProjectTracker.Domain.BusinessRules;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.UnitTests.Domain;

public class SprintStatusTransitionTests
{
    [Theory]
    [InlineData(SprintStatus.Planning, SprintStatus.Active)]
    [InlineData(SprintStatus.Planning, SprintStatus.Cancelled)]
    [InlineData(SprintStatus.Active, SprintStatus.Completed)]
    [InlineData(SprintStatus.Active, SprintStatus.Cancelled)]
    public void CanTransition_allows_valid_transitions(SprintStatus from, SprintStatus to)
    {
        Assert.True(SprintStatusTransition.CanTransition(from, to));
    }

    [Theory]
    [InlineData(SprintStatus.Planning, SprintStatus.Completed)]  // must start before completing
    [InlineData(SprintStatus.Completed, SprintStatus.Active)]     // terminal
    [InlineData(SprintStatus.Cancelled, SprintStatus.Active)]     // terminal
    [InlineData(SprintStatus.Active, SprintStatus.Planning)]      // no going back
    [InlineData(SprintStatus.Active, SprintStatus.Active)]        // no-op is not a transition
    public void CanTransition_rejects_invalid_transitions(SprintStatus from, SprintStatus to)
    {
        Assert.False(SprintStatusTransition.CanTransition(from, to));
    }

    [Fact]
    public void DescribeInvalid_calls_out_same_state()
    {
        var message = SprintStatusTransition.DescribeInvalid(SprintStatus.Active, SprintStatus.Active);
        Assert.Contains("already", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DescribeInvalid_names_both_states_for_a_cross_transition()
    {
        var message = SprintStatusTransition.DescribeInvalid(SprintStatus.Planning, SprintStatus.Completed);
        Assert.Contains("Planning", message);
        Assert.Contains("Completed", message);
    }
}
