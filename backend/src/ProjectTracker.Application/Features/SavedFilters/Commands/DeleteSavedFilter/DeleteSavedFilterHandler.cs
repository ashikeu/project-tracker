using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.SavedFilters.Commands.DeleteSavedFilter;

public record DeleteSavedFilterCommand(Guid SavedFilterId);

public interface IDeleteSavedFilterHandler
{
    Task<Result> Handle(DeleteSavedFilterCommand command, CancellationToken ct);
}

public class DeleteSavedFilterHandler : IDeleteSavedFilterHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteSavedFilterHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteSavedFilterCommand command, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var filter = await _db.SavedFilters
            .FirstOrDefaultAsync(f => f.Id == command.SavedFilterId && f.UserId == userId, ct);
        if (filter is null) return Result.Fail("Saved filter not found.");

        _db.SavedFilters.Remove(filter);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Saved filter deleted.");
    }
}
