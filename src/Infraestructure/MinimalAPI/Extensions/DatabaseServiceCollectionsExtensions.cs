using EntityFramework.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MinimalAPI.Extensions;

/// <summary>
/// Métodos de extensión para registrar los servicios de base de datos en el contenedor de dependencias.
/// </summary>
public static class DatabaseServiceCollectionExtensions
{
    /// <summary>
    /// Registra el contexto de base de datos <see cref="EduZasDotnetContext"/> usando MySQL/MariaDB
    /// con la cadena de conexión especificada en la configuración.
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrará el contexto.</param>
    /// <param name="configuration">La configuración de la aplicación que contiene la cadena de conexión.</param>
    /// <returns>La colección de servicios con el contexto de base de datos registrado.</returns>
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var conn = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<EduZasDotnetContext>(opts =>
            opts.UseMySql(conn, ServerVersion.Parse("12.0.2-mariadb"))
        );

        return services;
    }
}
