using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Auth.Commands.ResetPassword;

public interface IResetPasswordHandler
{
    Task<Result> Handle(ResetPasswordCommand command, CancellationToken ct);
}

public class ResetPasswordHandler : IResetPasswordHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordHandler(IApplicationDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken ct)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);

        if (user is null || user.PasswordResetToken != command.Token
            || user.PasswordResetTokenExpiresAt is null || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
        {
            return Result.Fail("This password reset link is invalid or has expired.");
        }

        user.PasswordHash = _passwordHasher.Hash(command.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;

        // Revoke all active refresh tokens: a password reset invalidates every existing session.
        var activeTokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == user.Id && rt.RevokedAt == null)
            .ToListAsync(ct);
        foreach (var t in activeTokens) t.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result.Ok("Password has been reset. Please log in with your new password.");
    }
}
