using EduZasAPI.Application.Common;
using EduZasAPI.Infraestructure.Bcrypt.Application.Common;

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
            .AddSettings(configuration)
            .AddDatabaseServices(configuration)
            .AddRepositories()
            .AddValidators()
            .AddUseCases()
            .AddOtherInfrastructureServices();

        return services;
    }

    private static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration cfg)
    {
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
