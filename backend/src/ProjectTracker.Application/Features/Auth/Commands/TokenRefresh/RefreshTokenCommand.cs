namespace ProjectTracker.Application.Features.Auth.Commands.TokenRefresh;

public record RefreshTokenCommand(string RefreshToken, string? IpAddress);
