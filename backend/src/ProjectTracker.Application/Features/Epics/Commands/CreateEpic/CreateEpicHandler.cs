using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Epics.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Epics.Commands.CreateEpic;

public interface ICreateEpicHandler
{
    Task<Result<EpicDto>> Handle(CreateEpicCommand command, CancellationToken ct);
}

public class CreateEpicHandler : ICreateEpicHandler
{
    private readonly IApplicationDbContext _db;

    public CreateEpicHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<EpicDto>> Handle(CreateEpicCommand command, CancellationToken ct)
    {
        var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);
        if (project is null) return Result<EpicDto>.Fail("Project not found.");

        if (command.OwnerUserId is not null)
        {
            var ownerIsMember = await _db.OrganizationUsers.AsNoTracking()
                .AnyAsync(ou => ou.OrganizationId == project.OrganizationId && ou.UserId == command.OwnerUserId, ct);
            if (!ownerIsMember)
            {
                return Result<EpicDto>.FailValidation(new[]
                {
                    new ValidationError(nameof(command.OwnerUserId), "The specified owner is not a member of this organization.")
                });
            }
        }

        var epic = new Epic
        {
            OrganizationId = project.OrganizationId,
            ProjectId = command.ProjectId,
            Name = command.Name.Trim(),
            Description = command.Description?.Trim(),
            OwnerUserId = command.OwnerUserId,
            Priority = command.Priority,
            Color = command.Color,
            Icon = command.Icon,
            StartDate = command.StartDate,
            EndDate = command.EndDate
        };
        _db.Epics.Add(epic);
        await _db.SaveChangesAsync(ct);

        string? ownerName = null;
        if (command.OwnerUserId is not null)
        {
            ownerName = await _db.Users.AsNoTracking().Where(u => u.Id == command.OwnerUserId)
                .Select(u => u.FirstName + " " + u.LastName).FirstOrDefaultAsync(ct);
        }

        return Result<EpicDto>.Ok(new EpicDto(
            epic.Id, epic.ProjectId, epic.Name, epic.Description, epic.OwnerUserId, ownerName,
            epic.Status.ToString(), epic.Priority.ToString(), epic.Color, epic.Icon,
            epic.StartDate, epic.EndDate, 0, 0, epic.CreatedAt));
    }
}
