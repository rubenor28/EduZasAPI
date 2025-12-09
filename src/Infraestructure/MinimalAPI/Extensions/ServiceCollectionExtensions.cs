using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.Services;
using MinimalAPI.Presentation.Routes;

namespace MinimalAPI.Extensions;

/// <summary>
/// Métodos de extensión para registrar servicios de infraestructura en el contenedor de dependencias.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios específicos de la API, como autenticación, CORS y Swagger.
    /// </summary>
    public static IServiceCollection AddApiSpecificServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddCorsConfig(configuration)
            .AddAuthSettings(configuration)
            .AddSwaggerServices()
            .AddApiServices();

        return services;
    }

    /// <summary>
    /// Registra configuraciones de autenticación y autorización para la API.
    /// </summary>
    private static IServiceCollection AddAuthSettings(
        this IServiceCollection services,
        IConfiguration cfg
    )
    {
        var jwtSettings = cfg.GetSection("JwtSettings").Get<JwtSettings>();
        ArgumentNullException.ThrowIfNull(
            jwtSettings,
            "JwtSettings must be defined on appsettings.json"
        );
        services.AddSingleton(jwtSettings);
        services.AddScoped<JwtService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = null,
                    ValidIssuer = null,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)
                    ),
                };

                opts.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("AuthToken", out var token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    },
                };
            });

        services
            .AddAuthorizationBuilder()
            .AddPolicy("RequireAuthenticated", policy => policy.RequireAuthenticatedUser())
            .AddPolicy("ProfessorOrAdmin", policy => policy.RequireRole("PROFESSOR", "ADMIN"))
            .AddPolicy("Admin", policy => policy.RequireRole("ADMIN"));

        services.AddAuthorization();
        services.AddAntiforgery();

        return services;
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "EduZasAPI",
                    Version = "v1",
                    Description = "Documentación de la API generada desde el código",
                }
            );
        });

        return services;
    }

    public static IServiceCollection AddCorsConfig(
        this IServiceCollection services,
        IConfiguration cfg
    )
    {
        var frontend = cfg.GetValue<string>("ServerOptions:FrontEndUrl");
        ArgumentNullException.ThrowIfNull(frontend, "FrontEndUrl must be defined on .env file");
        services.AddCors(opt =>
        {
            opt.AddPolicy(
                "AllowFrontend",
                policy =>
                {
                    policy
                        .WithOrigins(frontend)
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            );
        });
        return services;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<RoutesUtils>();
        return services;
    }
}

