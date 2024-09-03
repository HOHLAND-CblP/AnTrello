using System.Text;
using AnTrello.Backend.DbInfrastructure;
using AnTrello.Backend.Domain;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities.Jwt;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

// CORS =========================================================================        

        var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        
        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy  =>
                {
                    policy.WithOrigins("http://localhost:3000");
                    policy.AllowCredentials();
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });
        });
        
// Auth =========================================================================

        var issuer = config["JWT:Issuer"];
        var audience = config["JWT:Audience"];
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JWT:SecretKey"]));

        services.AddAuthorization(); 
        services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.IncludeErrorDetails = true;
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
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context => 
                    {
                        if (context.Principal.Claims.FirstOrDefault(claim => claim.Type == "type") == null)
                            context.Fail("Not valid token");

                        var tokenType = context.Principal.Claims.First(claim => claim.Type == "type").Value;
                        if (tokenType != TokenType.Access.ToString())
                            context.Fail("Not access token");

                        var userId = long.Parse(context.Principal.Claims.First(claim => claim.Type == "sub").Value);
                        var userService = services.BuildServiceProvider().GetService<IUserService>();
                        
                        var user = await userService!.GetById(userId, CancellationToken.None);
                        
                        
                        if (user==null)
                            context.Fail("No such user");
                    } };
            });
            Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        
// Swagger =================================================
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter JWT token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
        
// Create App ==============================================
        var app = builder.Build();

        if (Migrate(args, app))
            return;

        app.UseCors(MyAllowSpecificOrigins);

        app.UseAuthentication();
        app.UseAuthorization();
        
        
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