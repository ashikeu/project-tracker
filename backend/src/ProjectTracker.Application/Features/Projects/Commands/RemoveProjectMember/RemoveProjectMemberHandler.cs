using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Projects.Commands.RemoveProjectMember;

public record RemoveProjectMemberCommand(Guid ProjectId, Guid ProjectMemberId);

public interface IRemoveProjectMemberHandler
{
    Task<Result> Handle(RemoveProjectMemberCommand command, CancellationToken ct);
}

public class RemoveProjectMemberHandler : IRemoveProjectMemberHandler
{
    private readonly IApplicationDbContext _db;

    public RemoveProjectMemberHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(RemoveProjectMemberCommand command, CancellationToken ct)
    {
        var member = await _db.ProjectMembers.FirstOrDefaultAsync(pm => pm.Id == command.ProjectMemberId && pm.ProjectId == command.ProjectId, ct);
        if (member is null) return Result.Fail("Project member not found.");

        _db.ProjectMembers.Remove(member);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Project member removed.");
    }
}
