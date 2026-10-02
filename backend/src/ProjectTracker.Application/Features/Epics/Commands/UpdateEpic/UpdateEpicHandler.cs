using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Epics.Dtos;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Epics.Commands.UpdateEpic;

public interface IUpdateEpicHandler
{
    Task<Result<EpicDto>> Handle(UpdateEpicCommand command, CancellationToken ct);
}

public class UpdateEpicHandler : IUpdateEpicHandler
{
    private readonly IApplicationDbContext _db;

    public UpdateEpicHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<EpicDto>> Handle(UpdateEpicCommand command, CancellationToken ct)
    {
        var epic = await _db.Epics.FirstOrDefaultAsync(e => e.Id == command.EpicId, ct);
        if (epic is null) return Result<EpicDto>.Fail("Epic not found.");

        if (command.OwnerUserId is not null)
        {
            var ownerIsMember = await _db.OrganizationUsers.AsNoTracking()
                .AnyAsync(ou => ou.OrganizationId == epic.OrganizationId && ou.UserId == command.OwnerUserId, ct);
            if (!ownerIsMember)
            {
                return Result<EpicDto>.FailValidation(new[]
                {
                    new ValidationError(nameof(command.OwnerUserId), "The specified owner is not a member of this organization.")
                });
            }
        }

        epic.Name = command.Name.Trim();
        epic.Description = command.Description?.Trim();
        epic.OwnerUserId = command.OwnerUserId;
        epic.Status = command.Status;
        epic.Priority = command.Priority;
        epic.Color = command.Color;
        epic.Icon = command.Icon;
        epic.StartDate = command.StartDate;
        epic.EndDate = command.EndDate;

        await _db.SaveChangesAsync(ct);

        var taskCount = await _db.Tasks.AsNoTracking().CountAsync(t => t.EpicId == epic.Id, ct);
        var doneCount = await _db.Tasks.AsNoTracking().CountAsync(t => t.EpicId == epic.Id && t.Status == TaskItemStatus.Done, ct);
        string? ownerName = command.OwnerUserId is null ? null : await _db.Users.AsNoTracking()
            .Where(u => u.Id == command.OwnerUserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefaultAsync(ct);

        return Result<EpicDto>.Ok(new EpicDto(
            epic.Id, epic.ProjectId, epic.Name, epic.Description, epic.OwnerUserId, ownerName,
            epic.Status.ToString(), epic.Priority.ToString(), epic.Color, epic.Icon,
            epic.StartDate, epic.EndDate, taskCount, doneCount, epic.CreatedAt));
    }
}
