using AnTrello.Backend.DbInfrastructure.Migrations;
using AnTrello.Backend.DbInfrastructure.Repositories;
using AnTrello.Backend.DbInfrastructure.Settings;
using AnTrello.Backend.Domain.Contracts.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AnTrello.Backend.DbInfrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDbInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<DbSettings>(config.GetSection("DataBases"));
        
        services.AddMigrations(config);
        services.AddRepositories();
        MapCompositeTypes();
        
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITimeBlockRepository, TimeBlockRepository>();
        services.AddScoped<IPomodoroSessionRepository, PomodoroSessionRepository>();
        services.AddScoped<IPomodoroRoundRepository, PomodoroRoundRepository>();
        
        return services;
    }
    
    private static IServiceCollection AddMigrations(this IServiceCollection services, IConfiguration config)
    {
        string connectionString = config["DataBases:PostgresConnectionString"];
        services.AddFluentMigratorCore().ConfigureRunner(builder => builder.AddPostgres()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(DependencyInjection).Assembly).For.Migrations());

        return services;
    }
    
    private static void MapCompositeTypes()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}