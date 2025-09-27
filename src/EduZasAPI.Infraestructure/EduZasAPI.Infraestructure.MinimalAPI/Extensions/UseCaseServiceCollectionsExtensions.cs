using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Users;
using EduZasAPI.Application.Auth;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

/// <summary>
/// Métodos de extensión para registrar los casos de uso en el contenedor de dependencias.
/// </summary>
public static class UseCaseServiceCollectionExtensions
{
    /// <summary>
    /// Registra las implementaciones de los casos de uso relacionados con <see cref="UserDomain"/>.
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrarán los casos de uso.</param>
    /// <returns>La colección de servicios con los casos de uso registrados.</returns>
    public static IServiceCollection AddUseCases(
        this IServiceCollection services)
    {
        // Auth use cases
        services.AddTransient<AddUserUseCase>();
        services.AddTransient<LoginUseCase>();

        return services;
    }
}
