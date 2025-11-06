using Application.Services;
using EntityFramework.Application.DTOs;
using Mariadb.Application.Services;
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
        IConfiguration cfg
    )
    {
        var connStr = cfg.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connStr))
            throw new InvalidOperationException("DefaultConnection no puede ser nulo o vacío");

        var dumpPath = cfg.GetValue<string>("DatabaseBinaries:DumpPath");
        if (string.IsNullOrEmpty(dumpPath))
            throw new InvalidOperationException("DumpPath no puede ser nulo o vacío");

        var mariadbPath = cfg.GetValue<string>("DatabaseBinaries:MariadbPath");
        if (string.IsNullOrEmpty(mariadbPath))
            throw new InvalidOperationException("MariadbPath no puede ser nulo o vacío");

        services.AddDbContext<EduZasDotnetContext>(opts =>
            opts.UseMySql(connStr, ServerVersion.Parse("12.0.2-mariadb"))
        );

        services.AddScoped<IDatabaseExporter>(s => new MariaDbDumpExporter(connStr, dumpPath));
        services.AddScoped<IDatabaseImporter>(s => new MariaDbImporter(connStr, dumpPath));

        return services;
    }
}
