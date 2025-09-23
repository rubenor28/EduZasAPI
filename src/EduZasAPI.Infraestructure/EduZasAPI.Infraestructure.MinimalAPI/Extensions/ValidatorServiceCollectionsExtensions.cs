using EduZasAPI.Application.Ports.Services.Common;
using EduZasAPI.Domain.Entities;
using EduZasAPI.Application.DTOs.Users;
using EduZasAPI.Infraestructure.Application.Ports.Services.Users;

namespace EduZasAPI.Infraestructure.Extensions;

/// <summary>
/// Métodos de extensión para registrar validadores en el contenedor de dependencias.
/// </summary>
public static class ValidatorServiceCollectionExtensions
{
    /// <summary>
    /// Registra las implementaciones de validadores basados en <see cref="IBusinessValidationService{T}"/>.
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrarán los validadores.</param>
    /// <returns>La colección de servicios con los validadores registrados.</returns>
    public static IServiceCollection AddValidators(
        this IServiceCollection services)
    {
        // User validators
        services.AddTransient<IBusinessValidationService<UserDomain>, UserFluentValidator>();
        services.AddTransient<IBusinessValidationService<NewUserDTO>, NewUserFluentValidator>();
        services.AddTransient<IBusinessValidationService<UserUpdateDTO>, UserUpdateFluentValidator>();
        services.AddTransient<IBusinessValidationService<UserCredentialsDTO>, UserCredentialsFluentValidator>();
        services.AddTransient<IBusinessValidationService<RolChangeDTO>, RolChangeFluentValidator>();

        return services;
    }
}
