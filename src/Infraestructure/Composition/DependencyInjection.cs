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
    /// Registra todos los servicios de infraestructura y aplicación en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <param name="configuration">La configuración de la aplicación.</param>
    /// <returns>La colección de servicios actualizada.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDatabaseServices(configuration)
            .AddRepositories(configuration)
            .AddValidators()
            .AddMapperServices()
            .AddUseCases()
            .AddEmailSender(configuration)
            .AddOtherInfrastructureServices();
            // Nota: Los servicios específicos de la API (Auth, Swagger, CORS) no se registran aquí.

        return services;
    }
}
