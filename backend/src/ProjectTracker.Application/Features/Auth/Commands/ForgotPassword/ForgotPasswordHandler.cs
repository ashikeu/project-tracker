using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Auth.Commands.ForgotPassword;

public interface IForgotPasswordHandler
{
    Task<Result> Handle(ForgotPasswordCommand command, CancellationToken ct);
}

public class ForgotPasswordHandler : IForgotPasswordHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IEmailService _emailService;

    public ForgotPasswordHandler(IApplicationDbContext db, IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand command, CancellationToken ct)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);

        // Always return success, whether or not the account exists, to avoid user enumeration.
        if (user is not null && user.IsActive)
        {
            user.PasswordResetToken = Guid.NewGuid().ToString("N");
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
            await _db.SaveChangesAsync(ct);
            await _emailService.SendPasswordResetAsync(user.Email, user.FullName, user.PasswordResetToken, ct);
        }

        return Result.Ok("If an account with that email exists, a password reset link has been sent.");
    }
}
