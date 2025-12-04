using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
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
    IReaderAsync<string, UserDomain> userReader,
    IBusinessValidationService<UserCredentialsDTO> validator
) : IGuestUseCaseAsync<UserCredentialsDTO, UserDomain>
{
    private readonly IReaderAsync<string, UserDomain> _userReader = userReader;

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
        UserCredentialsDTO request
    )
    {
        var validation = validator.IsValid(request);
        if (validation.IsErr)
            return UseCaseErrors.Input(validation.UnwrapErr());

        var user = await _userReader.GetAsync(request.Email);
        if (user is null)
            return UseCaseErrors.NotFound();

        var pwdMatch = hasher.Matches(user.Password, request.Password);
        if (!pwdMatch)
            return UseCaseErrors.Input([
                new() { Field = "password", Message = "Contraseña incorrecta" },
            ]);

        return user;
    }
}
