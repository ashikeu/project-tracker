using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Infrastructure.Authentication;
using ProjectTracker.Infrastructure.FileStorage;
using ProjectTracker.Infrastructure.Notifications;
using ProjectTracker.Infrastructure.Persistence;

namespace ProjectTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenHasher, TokenHasher>();
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services.Configure<SmtpEmailSettings>(configuration.GetSection(SmtpEmailSettings.SectionName));
        services.AddTransient<IEmailService, SmtpEmailService>();

        services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
        services.AddSingleton<IFileStorageService, LocalFileStorageService>();
        services.AddSingleton<IFileUploadPolicy, FileUploadPolicy>();

        services.Configure<FcmSettings>(configuration.GetSection(FcmSettings.SectionName));
        services.AddSingleton<IPushSender, FcmPushSender>();
        services.AddScoped<IActivityLogger, ActivityLogger>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        services.AddHostedService<DueSoonNotificationJob>();

        return services;
    }
}
