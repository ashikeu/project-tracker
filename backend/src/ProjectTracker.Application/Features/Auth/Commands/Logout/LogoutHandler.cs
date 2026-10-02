using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Auth.Commands.Logout;

public interface ILogoutHandler
{
    Task<Result> Handle(LogoutCommand command, CancellationToken ct);
}

public class LogoutHandler : ILogoutHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ITokenHasher _tokenHasher;

    public LogoutHandler(IApplicationDbContext db, ITokenHasher tokenHasher)
    {
        _db = db;
        _tokenHasher = tokenHasher;
    }

    public async Task<Result> Handle(LogoutCommand command, CancellationToken ct)
    {
        var hash = _tokenHasher.Hash(command.RefreshToken);
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == hash, ct);
        if (token is not null && token.RevokedAt is null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
        // Idempotent: logging out an already-revoked/unknown token is still a success.
        return Result.Ok();
    }
}
