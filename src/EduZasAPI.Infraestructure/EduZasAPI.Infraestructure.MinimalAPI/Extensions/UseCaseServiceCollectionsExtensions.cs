using EduZasAPI.Domain.Users;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Auth;
using EduZasAPI.Application.Classes;

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

        // Class use cases
        services.AddTransient<AddClassUseCase>();
        services.AddTransient<QueryUseCase<ClassCriteriaDTO, ClassDomain>>();
        services.AddTransient<UpdateClassUseCase>();
        services.AddTransient<DeleteClassUseCase>();

        return services;
    }
}
