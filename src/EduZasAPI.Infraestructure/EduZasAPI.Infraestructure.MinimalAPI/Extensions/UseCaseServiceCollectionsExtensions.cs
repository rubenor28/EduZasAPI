using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;

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
        // User use cases
        services.AddTransient<AddUserUseCase>(sp => new AddUserUseCase(
              sp.GetRequiredService<IHashService>(),
              sp.GetRequiredService<ICreatorAsync<UserDomain, NewUserDTO>>(),
              sp.GetRequiredService<IBusinessValidationService<NewUserDTO>>(),
              sp.GetRequiredService<IQuerierAsync<UserDomain, UserCriteriaDTO>>()
        ));

        return services;
    }
}
