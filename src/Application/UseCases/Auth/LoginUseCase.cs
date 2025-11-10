using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Auth;

/// <summary>
/// Caso de uso para el proceso de autenticación de usuarios en el sistema.
/// </summary>
/// <remarks>
/// Esta clase implementa la lógica de login, verificando las credenciales del usuario
/// contra la base de datos y generando un token de autenticación en caso de éxito.
/// </remarks>
public class LoginUseCase(
    IHashService hasher,
    IQuerierAsync<UserDomain, UserCriteriaDTO> querier,
    IBusinessValidationService<UserCredentialsDTO> validator
) : IUseCaseAsync<UserCredentialsDTO, UserDomain>
{
    /// <summary>
    /// Ejecuta el proceso de autenticación con las credenciales proporcionadas.
    /// </summary>
    /// <param name="credentials">Credenciales del usuario (email y contraseña).</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado contiene un token de autenticación
    /// si las credenciales son válidas, o un error de campo específico si la autenticación falla.
    /// </returns>
    /// <exception cref="InvalidDataException">
    /// Se lanza cuando se detecta más de un usuario con el mismo email en la base de datos,
    /// indicando una inconsistencia en los datos.
    /// </exception>
    /// <remarks>
    /// El proceso de autenticación sigue estos pasos:
    /// 1. Busca el usuario por email exacto
    /// 2. Verifica que exista exactamente un usuario con ese email
    /// 3. Compara la contraseña proporcionada con el hash almacenado
    /// 4. Genera un token de autenticación si las credenciales son correctas
    /// </remarks>
    public async Task<Result<UserDomain, UseCaseError>> ExecuteAsync(
        UserCredentialsDTO credentials
    )
    {
        var validation = validator.IsValid(credentials);
        if (validation.IsErr)
            return UseCaseErrors.Input(validation.UnwrapErr());

        var emailSearch = new StringQueryDTO
        {
            Text = credentials.Email,
            SearchType = StringSearchType.EQ,
        };

        var userSearch = await querier.GetByAsync(
            new()
            {
                Active = Optional<bool>.Some(true),
                Email = Optional<StringQueryDTO>.Some(emailSearch),
            }
        );

        var results = userSearch.Results.Count();

        if (results > 1)
            throw new InvalidDataException($"Repeated email {credentials.Email} stored");

        if (results == 0)
            return UseCaseErrors.Input([new() { Field = "email", Message = "Email no encontrado" }]);

        var usr = userSearch.Results.ToList()[0];

        var pwdMatch = hasher.Matches(credentials.Password, usr.Password);
        if (!pwdMatch)
            return UseCaseErrors.Input(
                [new() { Field = "password", Message = "Contraseña incorrecta" }]
            );

        return usr;
    }
}
