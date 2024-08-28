using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Services;
using AnTrello.Backend.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnTrello.Backend.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(config.GetSection("JWT"));
        services.Configure<HashSettings>(config.GetSection("HashSettings"));
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITimeBlockService, TimeBlockService>();
        services.AddScoped<IPomodoroService, PomodoroService>();
        
        return services;
    }
}