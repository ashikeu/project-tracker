namespace ProjectTracker.Shared.Constants;

/// <summary>
/// Canonical list of permission keys. Seeded into the Permissions table at migration time and
/// used to dynamically register one authorization policy per key at startup (see
/// ProjectTracker.Api/Extensions/AuthorizationServiceExtensions.cs) — never hardcode a policy
/// name string outside this list.
/// </summary>
public static class PermissionKeys
{
    // Organization
    public const string OrganizationManage = "organization.manage";
    public const string OrganizationViewMembers = "organization.view_members";
    public const string OrganizationInviteMembers = "organization.invite_members";

    // Users / Roles
    public const string UserManage = "user.manage";
    public const string RoleManage = "role.manage";

    // Team
    public const string TeamCreate = "team.create";
    public const string TeamUpdate = "team.update";
    public const string TeamDelete = "team.delete";
    public const string TeamView = "team.view";
    public const string TeamManageMembers = "team.manage_members";

    // Project
    public const string ProjectCreate = "project.create";
    public const string ProjectUpdate = "project.update";
    public const string ProjectDelete = "project.delete";
    public const string ProjectView = "project.view";
    public const string ProjectManageMembers = "project.manage_members";

    // Epic
    public const string EpicCreate = "epic.create";
    public const string EpicUpdate = "epic.update";
    public const string EpicDelete = "epic.delete";

    // Task
    public const string TaskView = "task.view";
    public const string TaskCreate = "task.create";
    public const string TaskUpdate = "task.update";
    public const string TaskDelete = "task.delete";
    public const string TaskAssign = "task.assign";
    public const string TaskMove = "task.move";

    // Sprint
    public const string SprintCreate = "sprint.create";
    public const string SprintStart = "sprint.start";
    public const string SprintComplete = "sprint.complete";

    // Comment
    public const string CommentCreate = "comment.create";
    public const string CommentDelete = "comment.delete";

    // Attachment
    public const string AttachmentUpload = "attachment.upload";
    public const string AttachmentDelete = "attachment.delete";

    // Time tracking
    public const string TimeLog = "time.log";
    public const string TimeEdit = "time.edit";

    public static IReadOnlyList<string> All { get; } = new[]
    {
        OrganizationManage, OrganizationViewMembers, OrganizationInviteMembers,
        UserManage, RoleManage,
        TeamCreate, TeamUpdate, TeamDelete, TeamView, TeamManageMembers,
        ProjectCreate, ProjectUpdate, ProjectDelete, ProjectView, ProjectManageMembers,
        EpicCreate, EpicUpdate, EpicDelete,
        TaskView, TaskCreate, TaskUpdate, TaskDelete, TaskAssign, TaskMove,
        SprintCreate, SprintStart, SprintComplete,
        CommentCreate, CommentDelete,
        AttachmentUpload, AttachmentDelete,
        TimeLog, TimeEdit
    };
}

public static class SystemRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string OrganizationOwner = "OrganizationOwner";
    public const string OrganizationAdmin = "OrganizationAdmin";
    public const string ProjectManager = "ProjectManager";
    public const string TeamLead = "TeamLead";
    public const string Developer = "Developer";
    public const string Tester = "Tester";
    public const string Member = "Member";
    public const string Viewer = "Viewer";

    public static IReadOnlyList<string> All { get; } = new[]
    {
        SuperAdmin, OrganizationOwner, OrganizationAdmin, ProjectManager, TeamLead, Developer,
        Tester, Member, Viewer
    };
}
