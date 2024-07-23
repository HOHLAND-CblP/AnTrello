using System.Text;
using AnTrello.Backend.DbInfrastructure;
using AnTrello.Backend.Domain;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var config = builder.Configuration;

        services.AddDomain(config);
        services.AddDbInfrastructure(config);
        services.AddControllers();

        
// Auth =========================================================================
        var issuer = config["JWT:Issuer"];
        var audience = config["JWT:Audience"];
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JWT:SecretKey"]));
        
        services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                };
            });
        
// Swagger =================================================
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
// Create App ==============================================
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

        app.MapControllers();
        
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