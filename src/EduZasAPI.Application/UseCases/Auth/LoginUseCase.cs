using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;

namespace EduZasAPI.Application.Auth;

/// <summary>
/// Caso de uso para el proceso de autenticación de usuarios en el sistema.
/// </summary>
/// <remarks>
/// Esta clase implementa la lógica de login, verificando las credenciales del usuario
/// contra la base de datos y generando un token de autenticación en caso de éxito.
/// </remarks>
public class LoginUseCase : IUseCaseAsync<UserCredentialsDTO, Result<UserDomain, FieldErrorDTO>>
{
    private IHashService _hasher;
    private IQuerierAsync<UserDomain, UserCriteriaDTO> _querier;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="LoginUseCase"/>.
    /// </summary>
    /// <param name="hasher">Servicio de hashing para verificar contraseñas.</param>
    /// <param name="tokenService">Servicio de generación de tokens de autenticación.</param>
    /// <param name="querier">Consultor para buscar usuarios en la base de datos.</param>
    public LoginUseCase(
        IHashService hasher,
        IQuerierAsync<UserDomain, UserCriteriaDTO> querier)
    {
        _hasher = hasher;
        _querier = querier;
    }

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
    public async Task<Result<UserDomain, FieldErrorDTO>> ExecuteAsync(UserCredentialsDTO credentials)
    {
        var emailSearch = new StringQueryDTO
        {
            Text = credentials.Email,
            SearchType = StringSearchType.EQ
        };

        var userSearch = await _querier.GetByAsync(new UserCriteriaDTO
        {
            Active = Optional<bool>.Some(true),
            Email = Optional<StringQueryDTO>.Some(emailSearch)
        });

        var results = userSearch.Results.Count;

        if (results > 1)
            throw new InvalidDataException($"Repeated email {credentials.Email} stored");

        if (results == 0)
            return Result<UserDomain, FieldErrorDTO>
              .Err(new FieldErrorDTO
              {
                  Field = "email",
                  Message = "Email no encontrado",
              });

        var usr = userSearch.Results[0];
        var pwdMatch = _hasher.Matches(credentials.Password, usr.Password);

        if (!pwdMatch)
            return Result<UserDomain, FieldErrorDTO>
              .Err(new FieldErrorDTO
              {
                  Field = "password",
                  Message = "Contraseña incorrecta"
              });

        return Result<UserDomain, FieldErrorDTO>
          .Ok(usr);
    }
}
