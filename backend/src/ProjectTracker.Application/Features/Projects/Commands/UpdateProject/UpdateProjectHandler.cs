using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Projects.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Projects.Commands.UpdateProject;

public interface IUpdateProjectHandler
{
    Task<Result<ProjectDto>> Handle(UpdateProjectCommand command, CancellationToken ct);
}

public class UpdateProjectHandler : IUpdateProjectHandler
{
    private readonly IApplicationDbContext _db;

    public UpdateProjectHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand command, CancellationToken ct)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);
        if (project is null) return Result<ProjectDto>.Fail("Project not found.");

        if (command.ManagerUserId is not null)
        {
            var managerIsMember = await _db.OrganizationUsers.AsNoTracking()
                .AnyAsync(ou => ou.OrganizationId == project.OrganizationId && ou.UserId == command.ManagerUserId, ct);
            if (!managerIsMember)
            {
                return Result<ProjectDto>.FailValidation(new[]
                {
                    new ValidationError(nameof(command.ManagerUserId), "The specified manager is not a member of this organization.")
                });
            }
        }

        project.Name = command.Name.Trim();
        project.Description = command.Description?.Trim();
        project.ManagerUserId = command.ManagerUserId;
        project.Status = command.Status;
        project.Priority = command.Priority;
        project.StartDate = command.StartDate;
        project.TargetDate = command.TargetDate;

        await _db.SaveChangesAsync(ct);

        var owner = await _db.Users.AsNoTracking().FirstAsync(u => u.Id == project.OwnerUserId, ct);
        string? managerName = null;
        if (project.ManagerUserId is not null)
        {
            managerName = await _db.Users.AsNoTracking().Where(u => u.Id == project.ManagerUserId)
                .Select(u => u.FirstName + " " + u.LastName).FirstOrDefaultAsync(ct);
        }

        return Result<ProjectDto>.Ok(new ProjectDto(
            project.Id, project.Key, project.Name, project.Description,
            project.OwnerUserId, owner.FirstName + " " + owner.LastName, project.ManagerUserId, managerName,
            project.Status.ToString(), project.Priority.ToString(), project.StartDate, project.TargetDate,
            project.IsArchived, project.CreatedAt));
    }
}
