using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Auth.Commands.ChangePassword;

public interface IChangePasswordHandler
{
    Task<Result> Handle(ChangePasswordCommand command, CancellationToken ct);
}

public class ChangePasswordHandler : IChangePasswordHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordHandler(IApplicationDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, ct);
        if (user is null)
        {
            return Result.Fail("User not found.");
        }

        if (!_passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
        {
            return Result.Fail("Current password is incorrect.");
        }

        user.PasswordHash = _passwordHasher.Hash(command.NewPassword);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Password changed successfully.");
    }
}
