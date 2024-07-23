using AnTrello.Backend.DbInfrastructure;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var config = builder.Configuration;


        services.AddDbInfrastructure(config);
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();


        var app = builder.Build();

        if (Migrate(args, app))
            return;



        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseCors();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Run();
    }

    static bool Migrate(string[] args, WebApplication app)
    {
        if (args.Length > 0 && args[0] == "migrate")
        {
            using var scope = app.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            try
            {
                switch (args[1])
                {
                    case "up":
                        runner.MigrateUp();
                        break;
                    case "down":
                        runner.MigrateDown(Int32.Parse(args[2]));
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return true;
        }

        return false;
    }

}