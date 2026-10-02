namespace ProjectTracker.Domain.Common;

public abstract class BaseDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
