namespace ProjectTracker.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);
