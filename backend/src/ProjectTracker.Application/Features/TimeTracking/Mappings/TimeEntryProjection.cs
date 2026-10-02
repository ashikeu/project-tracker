using System.Linq.Expressions;
using ProjectTracker.Application.Features.TimeTracking.Dtos;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Application.Features.TimeTracking.Mappings;

public static class TimeEntryProjection
{
    public static readonly Expression<Func<TimeEntry, TimeEntryDto>> ToDto = e => new TimeEntryDto(
        e.Id, e.TaskId, e.UserId, e.User.FirstName + " " + e.User.LastName,
        e.Date, e.DurationMinutes, e.IsBillable, e.Description, e.IsRunningTimer, e.StartedAt, e.CreatedAt);
}
