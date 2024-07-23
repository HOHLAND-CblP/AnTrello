using AnTrello.Backend.DbInfrastructure.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AnTrello.Backend.DbInfrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDbInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        string connectionString = config["DataBases:PostgresConnectionString"];
        services.AddFluentMigratorCore().ConfigureRunner(builder => builder.AddPostgres()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(DependencyInjection).Assembly).For.Migrations());
        
        return services;
    }
}