using Application.DTOs.Classes;
using Application.DTOs.Users;
using Application.Services;
using FluentValidationProj.Application.Services.Auth;
using FluentValidationProj.Application.Services.Classes;
using FluentValidationProj.Application.Services.Common;
using FluentValidationProj.Application.Services.Users;


namespace MinimalAPI.Extensions;

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
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddSingleton<IBusinessValidationService<ulong>, ULongFluentValidator>();

        // Auth validators
        services.AddSingleton<
            IBusinessValidationService<UserCredentialsDTO>,
            UserCredentialsFluentValidator
        >();

        services.AddSingleton<IBusinessValidationService<NewUserDTO>, NewUserFluentValidator>();

        // User validators
        services.AddSingleton<
            IBusinessValidationService<UserUpdateDTO>,
            UserUpdateFluentValidator
        >();

        services.AddSingleton<
            IBusinessValidationService<UserCredentialsDTO>,
            UserCredentialsFluentValidator
        >();

        services.AddSingleton<IBusinessValidationService<RolChangeDTO>, RolChangeFluentValidator>();

        // Class validators
        services.AddSingleton<IBusinessValidationService<NewClassDTO>, NewClassFluentValidator>();

        services.AddSingleton<
            IBusinessValidationService<ClassUpdateDTO>,
            ClassUpdateFluentValidator
        >();

        return services;
    }
}
