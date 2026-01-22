using Application.DAOs;
using Application.DTOs.Users;
using Application.Services;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.Auth;

/// <summary>
/// Caso de uso para autenticación de usuarios.
/// </summary>
/// <remarks>
/// Verifica credenciales contra la base de datos y retorna el usuario si son válidas.
/// </remarks>
public class LoginUseCase(
    IHashService hasher,
    IReaderAsync<string, UserDomain> userReader,
    IBusinessValidationService<UserCredentialsDTO> validator
) : IGuestUseCaseAsync<UserCredentialsDTO, UserDomain>
{
    private readonly IReaderAsync<string, UserDomain> _userReader = userReader;

    /// <summary>
    /// Ejecuta la autenticación.
    /// </summary>
    /// <param name="request">Credenciales (email y password).</param>
    /// <returns>Usuario autenticado o error de credenciales inválidas.</returns>
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

        var pwdMatch = hasher.Matches(request.Password, user.Password);
        if (!pwdMatch)
            return UseCaseErrors.Input([
                new() { Field = "password", Message = "Contraseña incorrecta" },
            ]);

        return user;
    }
}
