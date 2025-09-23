using EduZasAPI.Application.Ports.DAOs;
using EduZasAPI.Infraestructure.Application.Ports.DAOs;
using EduZasAPI.Infraestructure.Application.DTOs;

namespace EduZasAPI.Infraestructure.Extensions;

/// <summary>
/// Métodos de extensión para registrar los repositorios en el contenedor de dependencias.
/// </summary>
public static class RepositoryServiceCollectionExtensions
{
    /// <summary>
    /// Registra las implementaciones de repositorios en el contenedor de servicios.
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrarán los repositorios.</param>
    /// <returns>La colección de servicios con los repositorios registrados.</returns>
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddTransient<IUserRepositoryAsync>(provider =>
        {
            var context = provider.GetRequiredService<EduZasDotnetContext>();
            var pageSize = provider.GetRequiredService<IConfiguration>()
                .GetValue<long>("ServerOptions:PageSize");

            return new UserEntityFrameworkRepository(context, (ulong)pageSize);
        });

        return services;
    }
}
