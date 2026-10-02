using Microsoft.AspNetCore.Identity;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Authentication;

/// <summary>Wraps ASP.NET Core Identity's PasswordHasher<T> (PBKDF2-HMAC-SHA256, salted,
/// versioned) — battle-tested, not a hand-rolled hash.</summary>
public class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<User> _inner = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

    public string Hash(string password) => _inner.HashPassword(null!, password);

    public bool Verify(string password, string hash)
    {
        var result = _inner.VerifyHashedPassword(null!, hash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
