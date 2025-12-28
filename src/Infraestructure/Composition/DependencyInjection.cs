using Composition.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Composition;

/// <summary>
/// Punto de entrada principal para la composición de la infraestructura.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra los servicios comunes de la aplicación (repositorios, casos de uso, validadores, etc.).
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddRepositories(configuration)
            .AddValidators()
            .AddMapperServices()
            .AddUseCases()
            .AddOtherInfrastructureServices();
            // Nota: Los servicios específicos de la API (Auth, Swagger, CORS) y de producción (DB, Email) no se registran aquí.

        return services;
    }

    /// <summary>
    /// Registra todos los servicios de infraestructura y aplicación en el contenedor de dependencias para el entorno de producción.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <param name="configuration">La configuración de la aplicación.</param>
    /// <returns>La colección de servicios actualizada.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDatabaseServices(configuration)
            .AddEmailSender(configuration)
            .AddApplicationServices(configuration);

        return services;
    }
}
