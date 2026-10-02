using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.CreateLabel;

public record CreateLabelCommand(Guid ProjectId, string Name, string Color);

public interface ICreateLabelHandler
{
    Task<Result<LabelDto>> Handle(CreateLabelCommand command, CancellationToken ct);
}

public class CreateLabelHandler : ICreateLabelHandler
{
    private readonly IApplicationDbContext _db;

    public CreateLabelHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<LabelDto>> Handle(CreateLabelCommand command, CancellationToken ct)
    {
        var name = command.Name.Trim();
        var exists = await _db.Labels.AsNoTracking().AnyAsync(l => l.ProjectId == command.ProjectId && l.Name == name, ct);
        if (exists)
        {
            return Result<LabelDto>.FailValidation(new[] { new ValidationError(nameof(command.Name), "A label with this name already exists in the project.") });
        }

        var label = new Label { ProjectId = command.ProjectId, Name = name, Color = command.Color };
        _db.Labels.Add(label);
        await _db.SaveChangesAsync(ct);

        return Result<LabelDto>.Ok(new LabelDto(label.Id, label.Name, label.Color));
    }
}
