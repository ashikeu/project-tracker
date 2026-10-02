namespace ProjectTracker.Domain.Enums;

/// <summary>Which kind of entity an Attachment is bound to. Stored as a string; the (EntityType,
/// EntityId) pair is a polymorphic association — attachments live in one table but can hang off a
/// Task, Comment, Project or Epic.</summary>
public enum AttachmentEntityType
{
    Task = 0,
    Comment = 1,
    Project = 2,
    Epic = 3
}
