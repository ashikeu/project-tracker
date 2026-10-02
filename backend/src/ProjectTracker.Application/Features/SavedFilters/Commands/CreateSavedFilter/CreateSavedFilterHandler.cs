using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.SavedFilters.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.SavedFilters.Commands.CreateSavedFilter;

public record CreateSavedFilterCommand(string Name, string Scope, Guid? ProjectId, string FilterJson);

public interface ICreateSavedFilterHandler
{
    Task<Result<SavedFilterDto>> Handle(CreateSavedFilterCommand command, CancellationToken ct);
}

public class CreateSavedFilterHandler : ICreateSavedFilterHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateSavedFilterHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<SavedFilterDto>> Handle(CreateSavedFilterCommand command, CancellationToken ct)
    {
        if (!IsValidJson(command.FilterJson))
        {
            return Result<SavedFilterDto>.FailValidation(new[] { new ValidationError(nameof(command.FilterJson), "FilterJson must be valid JSON.") });
        }

        if (command.ProjectId is not null)
        {
            var projectExists = await _db.Projects.AsNoTracking().AnyAsync(p => p.Id == command.ProjectId, ct);
            if (!projectExists)
            {
                return Result<SavedFilterDto>.FailValidation(new[] { new ValidationError(nameof(command.ProjectId), "Project not found.") });
            }
        }

        var filter = new SavedFilter
        {
            OrganizationId = _currentUser.OrganizationId!.Value,
            UserId = _currentUser.UserId!.Value,
            ProjectId = command.ProjectId,
            Name = command.Name.Trim(),
            Scope = string.IsNullOrWhiteSpace(command.Scope) ? "tasks" : command.Scope.Trim(),
            FilterJson = command.FilterJson
        };
        _db.SavedFilters.Add(filter);
        await _db.SaveChangesAsync(ct);

        return Result<SavedFilterDto>.Ok(new SavedFilterDto(
            filter.Id, filter.ProjectId, filter.Name, filter.Scope, filter.FilterJson, filter.CreatedAt));
    }

    private static bool IsValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        try { using var _ = JsonDocument.Parse(json); return true; }
        catch (JsonException) { return false; }
    }
}
