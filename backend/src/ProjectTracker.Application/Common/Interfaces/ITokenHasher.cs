namespace ProjectTracker.Application.Common.Interfaces;

/// <summary>Deterministic (unsalted) hashing for opaque tokens that must be looked up by
/// equality — e.g. refresh tokens, email-verification tokens. NOT for passwords: use
/// IPasswordHasher for those, which is intentionally salted/non-deterministic.</summary>
public interface ITokenHasher
{
    string Hash(string rawValue);
}
