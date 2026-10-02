using Microsoft.AspNetCore.Http;
using ProjectTracker.Application.Common.Interfaces;

namespace ProjectTracker.Infrastructure.Authentication;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private System.Security.Claims.ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid? OrganizationId
    {
        get
        {
            var value = User?.FindFirst(CustomClaimTypes.OrganizationId)?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public IReadOnlyList<string> Permissions =>
        User?.FindAll(CustomClaimTypes.Permission).Select(c => c.Value).ToList() ?? new List<string>();

    public bool HasPermission(string permissionKey) => Permissions.Contains(permissionKey);
}
