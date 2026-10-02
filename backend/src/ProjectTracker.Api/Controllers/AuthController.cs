using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Auth.Commands.ChangePassword;
using ProjectTracker.Application.Features.Auth.Commands.ForgotPassword;
using ProjectTracker.Application.Features.Auth.Commands.Login;
using ProjectTracker.Application.Features.Auth.Commands.Logout;
using ProjectTracker.Application.Features.Auth.Commands.TokenRefresh;
using ProjectTracker.Application.Features.Auth.Commands.Register;
using ProjectTracker.Application.Features.Auth.Commands.ResetPassword;
using ProjectTracker.Application.Features.Auth.Commands.VerifyEmail;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[EnableRateLimiting("auth")]
public class AuthController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public AuthController(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    private string? ClientIp => HttpContext.Connection.RemoteIpAddress?.ToString();

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, [FromServices] IRegisterHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(command, ct);
        return FromResult(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, [FromServices] ILoginHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new LoginCommand(request.Email, request.Password, request.OrganizationId, ClientIp), ct);
        return FromResult(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, [FromServices] IRefreshTokenHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new RefreshTokenCommand(request.RefreshToken, ClientIp), ct);
        return FromResult(result);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, [FromServices] ILogoutHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new LogoutCommand(request.RefreshToken), ct);
        return FromResult(result);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, [FromServices] IForgotPasswordHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(command, ct);
        return FromResult(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, [FromServices] IResetPasswordHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(command, ct);
        return FromResult(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, [FromServices] IChangePasswordHandler handler, CancellationToken ct)
    {
        if (_currentUser.UserId is null) return Unauthorized();
        var result = await handler.Handle(new ChangePasswordCommand(_currentUser.UserId.Value, request.CurrentPassword, request.NewPassword), ct);
        return FromResult(result);
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command, [FromServices] IVerifyEmailHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(command, ct);
        return FromResult(result);
    }
}

public record LoginRequest(string Email, string Password, Guid? OrganizationId);
public record RefreshTokenRequest(string RefreshToken);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
