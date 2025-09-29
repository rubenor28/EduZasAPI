using EduZasAPI.Application.Common;
using EduZasAPI.Infraestructure.Bcrypt.Application.Common;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

/// <summary>
/// Métodos de extensión para registrar servicios de infraestructura en el contenedor de dependencias.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios de infraestructura necesarios para la aplicación,
    /// incluyendo base de datos, repositorios, validadores, casos de uso y otros servicios.
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrarán las dependencias.</param>
    /// <param name="configuration">Configuración de la aplicación usada para inicializar servicios.</param>
    /// <returns>La colección de servicios con las dependencias de infraestructura registradas.</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddCorsConfig(configuration)
            .AddAuthSettings(configuration)
            .AddDatabaseServices(configuration)
            .AddRepositories()
            .AddValidators()
            .AddUseCases()
            .AddOtherInfrastructureServices()
            .AddSwaggerServices();

        return services;
    }

    /// <summary>
    ///  Registra configuraciones ya sea definidas en codigo o provenientes del 
    ///  appsettings.json de uso general
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrarán las dependencias.</param>
    /// <param name="cfg">Configuración de la aplicación usada para inicializar servicios.</param>
    /// <returns></returns>
    private static IServiceCollection AddAuthSettings(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        var jwtSettings = cfg.GetSection("JwtSettings").Get<JwtSettings>();
        ArgumentNullException.ThrowIfNull(jwtSettings, "JwtSettings must be defined on appsettings.json");
        services.AddSingleton<JwtSettings>(jwtSettings);
        services.AddScoped<JwtService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
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
                  }
              };
          });

        services.AddAuthorizationBuilder()
          .AddPolicy("RequireAuthenticated", policy => policy.RequireAuthenticatedUser())
          .AddPolicy("ProfessorOrAdmin", policy => policy.RequireRole("PROFESSOR", "ADMIN"))
          .AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"));

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "EduZasAPI",
                Version = "v1",
                Description = "Documentación de la API generada desde el código"
            });
        });

        return services;
    }

    public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration cfg)
    {
        var frontend = cfg.GetValue<string>("ServerOptions:FrontEndUrl");
        ArgumentNullException.ThrowIfNull(frontend, "FrontEndUrl must be defined on .env file");
        services.AddCors(opt =>
        {
            opt.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(frontend)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });
        return services;
    }

    /// <summary>
    /// Registra servicios adicionales de infraestructura que no pertenecen a categorías específicas.
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrarán las dependencias.</param>
    /// <returns>La colección de servicios con los servicios adicionales registrados.</returns>
    private static IServiceCollection AddOtherInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IHashService, BCryptHasher>();
        services.AddSingleton<RoutesUtils>();
        return services;
    }
}
