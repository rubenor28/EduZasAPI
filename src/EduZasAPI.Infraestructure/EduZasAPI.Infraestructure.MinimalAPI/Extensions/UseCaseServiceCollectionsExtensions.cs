using EduZasAPI.Domain.Entities;

using EduZasAPI.Application.UseCases.Common;
using EduZasAPI.Application.DTOs.Users;

namespace EduZasAPI.Infraestructure.Extensions;

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
        services.AddTransient<AddUseCase<NewUserDTO, UserDomain>>();
        services.AddTransient<DeleteUseCase<ulong, UserDomain>>();
        services.AddTransient<ReadUseCase<ulong, UserDomain>>();
        services.AddTransient<UpdateUseCase<UserUpdateDTO, UserDomain>>();
        services.AddTransient<QueryUseCase<UserCriteriaDTO, UserDomain>>();

        return services;
    }
}
