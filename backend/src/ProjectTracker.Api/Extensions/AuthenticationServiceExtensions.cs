using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProjectTracker.Infrastructure.Authentication;

namespace ProjectTracker.Api.Extensions;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Without this, ASP.NET Core remaps short claim names ("sub", "email", ...) to
            // long legacy XML-schema URIs on the way in, which breaks any code (like
            // ICurrentUserService) that looks up claims by the short names the token was
            // actually issued with (see JwtTokenService — issues JwtRegisteredClaimNames.Sub).
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                        .CreateLogger("JwtAuthentication");
                    logger.LogWarning("JWT authentication failed: {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
