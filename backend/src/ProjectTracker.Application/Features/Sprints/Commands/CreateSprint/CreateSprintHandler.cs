using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Sprints.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Sprints.Commands.CreateSprint;

public interface ICreateSprintHandler
{
    Task<Result<SprintDto>> Handle(CreateSprintCommand command, CancellationToken ct);
}

public class CreateSprintHandler : ICreateSprintHandler
{
    private readonly IApplicationDbContext _db;

    public CreateSprintHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SprintDto>> Handle(CreateSprintCommand command, CancellationToken ct)
    {
        var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);
        if (project is null) return Result<SprintDto>.Fail("Project not found.");

        var sprint = new Sprint
        {
            OrganizationId = project.OrganizationId,
            ProjectId = command.ProjectId,
            Name = command.Name.Trim(),
            Goal = command.Goal?.Trim(),
            Status = SprintStatus.Planning,
            StartDate = command.StartDate,
            EndDate = command.EndDate
        };
        _db.Sprints.Add(sprint);
        await _db.SaveChangesAsync(ct);

        return Result<SprintDto>.Ok(new SprintDto(
            sprint.Id, sprint.ProjectId, sprint.Name, sprint.Goal, sprint.Status.ToString(),
            sprint.StartDate, sprint.EndDate, sprint.CompletedAt, 0, 0, 0, 0, sprint.CreatedAt));
    }
}
