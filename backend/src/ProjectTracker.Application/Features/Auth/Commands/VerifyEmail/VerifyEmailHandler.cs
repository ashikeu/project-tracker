using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Auth.Commands.VerifyEmail;

public interface IVerifyEmailHandler
{
    Task<Result> Handle(VerifyEmailCommand command, CancellationToken ct);
}

public class VerifyEmailHandler : IVerifyEmailHandler
{
    private readonly IApplicationDbContext _db;

    public VerifyEmailHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(VerifyEmailCommand command, CancellationToken ct)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);

        if (user is null || user.EmailVerificationToken != command.Token
            || user.EmailVerificationTokenExpiresAt is null || user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
        {
            return Result.Fail("This verification link is invalid or has expired.");
        }

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiresAt = null;
        await _db.SaveChangesAsync(ct);

        return Result.Ok("Email verified successfully.");
    }
}
