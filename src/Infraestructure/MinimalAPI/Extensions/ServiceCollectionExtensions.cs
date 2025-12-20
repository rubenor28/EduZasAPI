using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using InterfaceAdapters.Mappers.Tests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MinimalAPI.Application.DTOs;
using MinimalAPI.Application.DTOs.RateLimit;
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
    /// <param name="services">La colección de servicios a la que se agregarán los servicios.</param>
    /// <param name="configuration">La configuración de la aplicación para obtener valores necesarios.</param>
    /// <returns>La misma colección de servicios con los servicios agregados para permitir el encadenamiento.</returns>
    public static IServiceCollection AddApiSpecificServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddJsonSerialization()
            .AddCorsConfig(configuration)
            .AddAuthSettings(configuration)
            .AddRateLimit(configuration)
            .AddSwaggerServices()
            .AddApiServices();

        return services;
    }
    
    /// <summary>
    /// Configura las opciones de serialización JSON para la aplicación.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <returns>La misma colección de servicios para encadenamiento.</returns>
    private static IServiceCollection AddJsonSerialization(this IServiceCollection services)
    {
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new IQuestionJsonConverter());
            options.SerializerOptions.Converters.Add(new IPublicQuestionJsonConverter());
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        return services;
    }

    /// <summary>
    /// Registra configuraciones de autenticación y autorización para la API.
    /// </summary>
    /// <param name="services">La colección de servicios a la que se agregarán los servicios.</param>
    /// <param name="cfg">La configuración de la aplicación para obtener los ajustes de JWT.</param>
    /// <returns>La misma colección de servicios con los servicios de autenticación y autorización agregados.</returns>
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

    /// <summary>
    /// Registra los servicios de Swagger para la generación de documentación de la API.
    /// </summary>
    /// <param name="services">La colección de servicios a la que se agregarán los servicios de Swagger.</param>
    /// <returns>La misma colección de servicios con los servicios de Swagger agregados.</returns>
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

    /// <summary>
    /// Configura las políticas de CORS para permitir solicitudes desde el frontend.
    /// </summary>
    /// <param name="services">La colección de servicios a la que se agregará la configuración de CORS.</param>
    /// <param name="cfg">La configuración de la aplicación para obtener la URL del frontend.</param>
    /// <returns>La misma colección de servicios con la configuración de CORS agregada.</returns>
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

    /// <summary>
    /// Registra servicios de utilidad específicos de la API, como 'RoutesUtils'.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <returns>La misma colección de servicios con los servicios de utilidad agregados.</returns>
    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<RoutesUtils>();
        return services;
    }

    /// <summary>
    /// Configura el middleware de limitación de velocidad (Rate Limiting) para la API.
    /// </summary>
    /// <param name="services">La colección de servicios a la que se agregará la configuración.</param>
    /// <param name="config">La configuración de la aplicación para obtener los ajustes de Rate Limiting.</param>
    /// <returns>La misma colección de servicios con la configuración de Rate Limiting agregada.</returns>
    private static IServiceCollection AddRateLimit(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var cfg = config.GetRequiredSection("RateLimitSettings").Get<RateLimitSettings>();
        ArgumentNullException.ThrowIfNull(
            cfg,
            "RateLimitSettings must be defined on appsettings.json"
        );

        services.AddRateLimiter(options =>
        {
            // CONFIGURACIÓN GLOBAL
            // Usamos GlobalLimiter con PartitionedRateLimiter.
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                {
                    // Intentar obtener el ID del usuario autenticado.
                    // Esto funciona porque el middleware de RateLimiter se ejecuta DESPUÉS del de Autenticación.
                    var userId = (string?)httpContext.Items["UserId"];

                    // Si el usuario está autenticado, su ID es la clave de partición.
                    // Cada usuario tiene sus propio rate limit.
                    if (!string.IsNullOrEmpty(userId))
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: userId,
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = cfg.Authenticated.PermitLimit,
                                Window = TimeSpan.FromSeconds(cfg.Authenticated.WindowSeconds),
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = cfg.Authenticated.QueueLimit,
                            }
                        );
                    }

                    // Si el usuario es anónimo, usamos la IP como fallback.
                    // Todos los usuarios anónimos en la misma IP compartirán el mismo rate limit.
                    var remoteIpAddress =
                        httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? IPAddress.Loopback.ToString();

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: remoteIpAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = cfg.Guest.PermitLimit,
                            Window = TimeSpan.FromSeconds(cfg.Guest.WindowSeconds),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = cfg.Guest.QueueLimit,
                        }
                    );
                }
            );

            // Código de estado al rechazar (429)
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Personalizar el mensaje de error
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync(
                    "Has excedido el límite de solicitudes permitidas.",
                    token
                );
            };
        });

        return services;
    }
}

