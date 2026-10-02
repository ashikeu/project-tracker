namespace ProjectTracker.Application.Features.Notifications.Dtos;

public record NotificationDto(
    Guid Id, string Type, string Title, string Body,
    string? RelatedEntityType, Guid? RelatedEntityId, bool IsRead, DateTime? ReadAt, DateTime CreatedAt);

public record NotificationPreferenceDto(string NotificationType, bool InAppEnabled, bool PushEnabled);

public record ActivityLogDto(
    Guid Id, string EntityType, Guid EntityId, Guid? UserId, string? UserName,
    string Action, string? OldValueJson, string? NewValueJson, DateTime CreatedAt);
