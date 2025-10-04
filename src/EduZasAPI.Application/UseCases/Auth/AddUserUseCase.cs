using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;

namespace EduZasAPI.Application.Auth;

/// <summary>
/// Caso de uso específico para la adición de nuevos usuarios al sistema.
/// </summary>
/// <remarks>
/// Esta clase extiende la funcionalidad base de <see cref="AddUseCase{NE, E}"/> para aplicar
/// validaciones y transformaciones específicas para usuarios, como la verificación de unicidad
/// de email y el hashing de contraseñas.
/// </remarks>
public class AddUserUseCase : AddUseCase<NewUserDTO, UserDomain>
{
    /// <summary>
    /// Servicio de hashing para contraseñas.
    /// </summary>
    protected IHashService _hasher;

    /// <summary>
    /// Consultor para verificar la existencia de usuarios.
    /// </summary>
    protected IQuerierAsync<UserDomain, UserCriteriaDTO> _querier;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="AddUserUseCase"/>.
    /// </summary>
    /// <param name="hasher">Servicio de hashing para contraseñas.</param>
    /// <param name="creator">Creador de entidades de usuario.</param>
    /// <param name="validator">Validador de reglas de negocio para usuarios.</param>
    /// <param name="querier">Consultor para verificar la existencia de usuarios.</param>
    public AddUserUseCase(
        IHashService hasher,
        ICreatorAsync<UserDomain, NewUserDTO> creator,
        IBusinessValidationService<NewUserDTO> validator,
        IQuerierAsync<UserDomain, UserCriteriaDTO> querier)
      : base(creator, validator)
    {
        _querier = querier;
        _hasher = hasher;
    }

    /// <summary>
    /// Realiza validación asíncrona adicional para verificar la unicidad del email.
    /// </summary>
    /// <param name="usr">DTO con los datos del nuevo usuario a validar.</param>
    /// <returns>
    /// Un resultado que indica si la validación fue exitosa o contiene errores de email duplicado.
    /// </returns>
    /// <exception cref="InvalidDataException">
    /// Se lanza cuando se detecta más de un usuario con el mismo email en la base de datos.
    /// </exception>
    /// <remarks>
    /// Este método verifica que no exista otro usuario con el mismo email en el sistema.
    /// Si encuentra exactamente un usuario con el mismo email, retorna un error de validación.
    /// Si encuentra más de uno, lanza una excepción indicando inconsistencia en los datos.
    /// </remarks>
    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(NewUserDTO usr)
    {
        var errs = new List<FieldErrorDTO>();

        var emailSearch = new StringQueryDTO { Text = usr.Email, SearchType = StringSearchType.EQ };

        var search = await _querier.GetByAsync(new UserCriteriaDTO
        {
            Email = Optional.Some(emailSearch)
        });

        var results = search.Results.Count;

        if (results > 1)
            throw new InvalidDataException($"Repeated email {usr.Email} stored");

        if (results == 1)
        {
            var error = new FieldErrorDTO { Field = "email", Message = "Email ya registrado" };
            errs.Add(error);
            return Result.Err(UseCaseError.InputError(errs));
        }

        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
    }

    protected override NewUserDTO PreValidationFormat(NewUserDTO value) => new NewUserDTO
    {
        FirstName = value.FirstName.ToUpperInvariant(),
        FatherLastName = value.FatherLastName.ToUpperInvariant(),
        MidName = value.MidName
          .Match<Optional<string>>(
            name => name.ToUpperInvariant().ToOptional(),
            () => Optional<string>.None()),
        MotherLastname = value.MotherLastname
          .Match<Optional<string>>(
            name => name.ToUpperInvariant().ToOptional(),
            () => Optional<string>.None()),
        Email = value.Email,
        Password = value.Password
    };

    /// <summary>
    /// Aplica formato final a los datos del usuario antes de la persistencia.
    /// </summary>
    /// <param name="u">DTO con los datos del usuario a formatear.</param>
    /// <returns>DTO con los datos formateados, incluyendo la contraseña hasheada.</returns>
    /// <remarks>
    /// Este método se encarga de aplicar el hashing a la contraseña del usuario
    /// antes de que sea persistida en la base de datos.
    /// </remarks>
    protected override NewUserDTO PostValidationFormat(NewUserDTO u)
    {
        u.Password = _hasher.Hash(u.Password);
        return u;
    }
}
