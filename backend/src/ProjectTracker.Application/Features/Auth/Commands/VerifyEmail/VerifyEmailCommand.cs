namespace ProjectTracker.Application.Features.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string Email, string Token);
