using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Boards;
using ProjectTracker.Application.Features.Projects.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Projects.Commands.CreateProject;

public interface ICreateProjectHandler
{
    Task<Result<ProjectDto>> Handle(CreateProjectCommand command, CancellationToken ct);
}

public class CreateProjectHandler : ICreateProjectHandler
{
    private readonly IApplicationDbContext _db;

    public CreateProjectHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ProjectDto>> Handle(CreateProjectCommand command, CancellationToken ct)
    {
        var key = command.Key.Trim().ToUpperInvariant();

        var keyTaken = await _db.Projects.AsNoTracking()
            .AnyAsync(p => p.OrganizationId == command.OrganizationId && p.Key == key, ct);
        if (keyTaken)
        {
            return Result<ProjectDto>.FailValidation(new[]
            {
                new ValidationError(nameof(command.Key), $"Project key '{key}' is already in use in this organization.")
            });
        }

        if (command.ManagerUserId is not null)
        {
            var managerIsMember = await _db.OrganizationUsers.AsNoTracking()
                .AnyAsync(ou => ou.OrganizationId == command.OrganizationId && ou.UserId == command.ManagerUserId, ct);
            if (!managerIsMember)
            {
                return Result<ProjectDto>.FailValidation(new[]
                {
                    new ValidationError(nameof(command.ManagerUserId), "The specified manager is not a member of this organization.")
                });
            }
        }

        var project = new Project
        {
            OrganizationId = command.OrganizationId,
            Key = key,
            Name = command.Name.Trim(),
            Description = command.Description?.Trim(),
            OwnerUserId = command.CreatedByUserId,
            ManagerUserId = command.ManagerUserId,
            Priority = command.Priority,
            StartDate = command.StartDate,
            TargetDate = command.TargetDate,
            NextTaskNumber = 1
        };
        _db.Projects.Add(project);

        foreach (var column in DefaultBoardColumns.Build(project.Id))
        {
            _db.BoardColumns.Add(column);
        }

        await _db.SaveChangesAsync(ct);

        var owner = await _db.Users.AsNoTracking().FirstAsync(u => u.Id == command.CreatedByUserId, ct);
        string? managerName = null;
        if (command.ManagerUserId is not null)
        {
            managerName = await _db.Users.AsNoTracking().Where(u => u.Id == command.ManagerUserId)
                .Select(u => u.FirstName + " " + u.LastName).FirstOrDefaultAsync(ct);
        }

        return Result<ProjectDto>.Ok(new ProjectDto(
            project.Id, project.Key, project.Name, project.Description,
            project.OwnerUserId, owner.FirstName + " " + owner.LastName, project.ManagerUserId, managerName,
            project.Status.ToString(), project.Priority.ToString(), project.StartDate, project.TargetDate,
            project.IsArchived, project.CreatedAt));
    }
}
