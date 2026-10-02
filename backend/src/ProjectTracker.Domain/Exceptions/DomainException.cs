namespace ProjectTracker.Domain.Exceptions;

/// <summary>Thrown when a Domain BusinessRule is violated. Caught by the Api's global exception
/// middleware and translated to a 422/409 response — never bubbles a stack trace to the client.</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" ({key}) was not found.")
    {
    }
}

public class TenantAccessDeniedException : Exception
{
    public TenantAccessDeniedException()
        : base("The requested resource does not belong to the caller's organization.")
    {
    }
}

public class ConcurrencyConflictException : Exception
{
    public object? ServerState { get; }

    public ConcurrencyConflictException(string message, object? serverState = null) : base(message)
    {
        ServerState = serverState;
    }
}
