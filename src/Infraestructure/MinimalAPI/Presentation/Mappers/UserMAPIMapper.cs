using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Users;

namespace MinimalAPI.Presentation.Mappers;

/// <summary>
/// Mapeador centralizado para la entidad de Usuario en la capa de la API.
/// </summary>
/// <remarks>
/// Esta clase implementa múltiples interfaces <see cref="IMapper{TIn, TOut}"/> para manejar
/// las diversas transformaciones entre los DTOs de la API (Minimal API), los DTOs de la capa de aplicación
/// y las entidades de dominio relacionadas con el usuario.
/// Su rol es actuar como un "Adaptador" en el contexto de Clean Architecture.
/// </remarks>
public sealed class UserMAPIMapper(
    IMapper<UserType, ulong> roleFromDomainMapper,
    IMapper<uint, Result<UserType, Unit>> roleUintToDomainMapper,
    IMapper<Optional<UserType>, int?> optRoleFromDomainMapper,
    IMapper<int, Result<UserType, Unit>> roleToDomainMapper,
    IMapper<StringQueryMAPI?, Result<Optional<StringQueryDTO>, Unit>> strqToDomainMapper,
    IMapper<Optional<StringQueryDTO>, StringQueryMAPI?> strqFromDomainMapper
)
    : IMapper<UserDomain, UserMAPI>,
        IMapper<Executor, ReadUserDTO>,
        IMapper<UserDomain, PublicUserMAPI>,
        IMapper<PublicUserDTO, PublicUserMAPI>,
        IMapper<ulong, Executor, DeleteUserDTO>,
        IMapper<NewUserMAPI, Executor, NewUserDTO>,
        IMapper<UserUpdateMAPI, Executor, Result<UserUpdateDTO, IEnumerable<FieldErrorDTO>>>,
        IMapper<UserCriteriaMAPI, Result<UserCriteriaDTO, IEnumerable<FieldErrorDTO>>>,
        IMapper<UserCriteriaDTO, UserCriteriaMAPI>,
        IMapper<
            PaginatedQuery<UserDomain, UserCriteriaDTO>,
            PaginatedQuery<PublicUserMAPI, UserCriteriaMAPI>
        >
{
    private readonly IMapper<UserType, ulong> _roleFromDomainMapper = roleFromDomainMapper;
    private readonly IMapper<uint, Result<UserType, Unit>> _roleUintToDomainMapper =
        roleUintToDomainMapper;
    private readonly IMapper<int, Result<UserType, Unit>> _roleToDomainMapper = roleToDomainMapper;
    private readonly IMapper<Optional<UserType>, int?> _optRoleFromDomainMapper =
        optRoleFromDomainMapper;
    private readonly IMapper<Optional<StringQueryDTO>, StringQueryMAPI?> _strqFromDomainMapper =
        strqFromDomainMapper;
    private readonly IMapper<
        StringQueryMAPI?,
        Result<Optional<StringQueryDTO>, Unit>
    > _strqToDomainMapper = strqToDomainMapper;

    /// <summary>
    /// Mapea los datos de un nuevo usuario desde la API y el ejecutor de la acción a un DTO para el caso de uso de creación.
    /// </summary>
    /// <param name="input">El DTO de la API con los datos del nuevo usuario.</param>
    /// <param name="ex">El <see cref="Executor"/> que realiza la operación.</param>
    /// <returns>Un <see cref="NewUserDTO"/> para la capa de aplicación.</returns>
    public NewUserDTO Map(NewUserMAPI input, Executor ex) =>
        new()
        {
            FirstName = input.FirstName,
            FatherLastName = input.FatherLastName,
            Email = input.Email,
            Password = input.Password,
            MotherLastname = input.MotherLastname.ToOptional(),
            MidName = input.MidName.ToOptional(),
        };

    /// <summary>
    /// Mapea un DTO público de la capa de aplicación a un DTO público para la API.
    /// </summary>
    /// <param name="source">El DTO de la capa de aplicación.</param>
    /// <returns>Un <see cref="PublicUserMAPI"/> con datos públicos del usuario.</returns>
    public PublicUserMAPI Map(PublicUserDTO source) =>
        new()
        {
            Id = source.Id,
            FirstName = source.FirstName,
            FatherLastName = source.FatherLastName,
            Email = source.Email,
            MotherLastname = source.MotherLastname.ToNullable(),
            MidName = source.MidName.ToNullable(),
            Role = _roleFromDomainMapper.Map(source.Role),
        };

    /// <summary>
    /// Mapea una entidad de dominio de usuario a un DTO público para la API.
    /// </summary>
    /// <param name="source">La entidad <see cref="UserDomain"/>.</param>
    /// <returns>Un <see cref="PublicUserMAPI"/> con datos públicos del usuario.</returns>
    public PublicUserMAPI Map(UserDomain source) =>
        new()
        {
            Id = source.Id,
            FirstName = source.FirstName,
            FatherLastName = source.FatherLastname,
            Email = source.Email,
            MotherLastname = source.MotherLastname.ToNullable(),
            MidName = source.MidName.ToNullable(),
            Role = _roleFromDomainMapper.Map(source.Role),
        };

    /// <summary>
    /// Valida y mapea los criterios de búsqueda de usuarios desde la API a un DTO para la capa de aplicación.
    /// </summary>
    /// <remarks>
    /// Este método combina la validación y el mapeo. Si la validación falla, devuelve una lista de errores.
    /// En un diseño más estricto, la validación podría extraerse a un componente separado.
    /// </remarks>
    /// <param name="source">Los criterios de búsqueda provenientes de la API.</param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene el <see cref="UserCriteriaDTO"/> si la validación es exitosa,
    /// o una colección de <see cref="FieldErrorDTO"/> si falla.
    /// </returns>
    public Result<UserCriteriaDTO, IEnumerable<FieldErrorDTO>> Map(UserCriteriaMAPI source)
    {
        var errs = new List<FieldErrorDTO>();
        var firstNameValidation = _strqToDomainMapper.Map(source.FirstName);
        firstNameValidation.IfErr(_ => errs.Add(new() { Field = "fistName" }));

        var midNameValidation = _strqToDomainMapper.Map(source.MidName);
        midNameValidation.IfErr(_ => errs.Add(new() { Field = "midName" }));

        var fatherLastNameValidation = _strqToDomainMapper.Map(source.FatherLastname);
        fatherLastNameValidation.IfErr(_ => errs.Add(new() { Field = "fatherLastname" }));

        var motherLastnameValidation = _strqToDomainMapper.Map(source.MotherLastname);
        motherLastnameValidation.IfErr(_ => errs.Add(new() { Field = "motherLastname" }));

        var emailValidation = _strqToDomainMapper.Map(source.Email);
        emailValidation.IfErr(_ => errs.Add(new() { Field = "email" }));

        var passwordValidation = _strqToDomainMapper.Map(source.Password);
        passwordValidation.IfErr(_ => errs.Add(new() { Field = "password" }));

        var roleValidation = source.Role is null
            ? Unit.Value
            : _roleToDomainMapper.Map((int)source.Role);

        roleValidation.IfErr(_ => errs.Add(new() { Field = "role" }));

        if (errs.Count > 0)
            return errs;

        return new UserCriteriaDTO
        {
            FirstName = firstNameValidation.Unwrap(),
            MidName = midNameValidation.Unwrap(),
            FatherLastName = fatherLastNameValidation.Unwrap(),
            MotherLastname = motherLastnameValidation.Unwrap(),
            Email = emailValidation.Unwrap(),
            Password = passwordValidation.Unwrap(),
            Role = roleValidation.Unwrap(),
            Page = source.Page,
            Active = source.Active.ToOptional(),
            CreatedAt = source.CreatedAt.ToOptional(),
            ModifiedAt = source.CreatedAt.ToOptional(),
        };
    }

    /// <summary>
    /// Mapea un <see cref="Executor"/> a un <see cref="ReadUserDTO"/> para casos de uso de lectura.
    /// </summary>
    /// <param name="input">El <see cref="Executor"/> que realiza la acción.</param>
    /// <returns>Un <see cref="ReadUserDTO"/> que encapsula al ejecutor.</returns>
    public ReadUserDTO Map(Executor input) => new() { Id = input.Id, Executor = input };

    /// <summary>
    /// Mapea una entidad de dominio <see cref="UserDomain"/> a su representación completa para la API.
    /// </summary>
    /// <param name="input">La entidad de dominio del usuario.</param>
    /// <returns>Un DTO <see cref="UserMAPI"/> con todos los datos del usuario.</returns>
    UserMAPI IMapper<UserDomain, UserMAPI>.Map(UserDomain input) =>
        new()
        {
            Id = input.Id,
            Active = input.Active,
            Email = input.Email,
            Role = _roleFromDomainMapper.Map(input.Role),
            FirstName = input.FirstName,
            FatherLastName = input.FatherLastname,
            MidName = input.MidName.ToNullable(),
            MotherLastname = input.MotherLastname.ToNullable(),
            Password = input.Password,
        };

    public UserCriteriaMAPI Map(UserCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            Role = _optRoleFromDomainMapper.Map(input.Role),
            Active = input.Active.ToNullable(),
            Email = _strqFromDomainMapper.Map(input.Email),
            Password = _strqFromDomainMapper.Map(input.Password),
            FirstName = _strqFromDomainMapper.Map(input.FirstName),
            FatherLastname = _strqFromDomainMapper.Map(input.FatherLastName),
            MidName = _strqFromDomainMapper.Map(input.MidName),
            MotherLastname = _strqFromDomainMapper.Map(input.MotherLastname),
            CreatedAt = input.CreatedAt.ToNullable(),
            ModifiedAt = input.ModifiedAt.ToNullable(),
        };

    public PaginatedQuery<PublicUserMAPI, UserCriteriaMAPI> Map(
        PaginatedQuery<UserDomain, UserCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            TotalPages = input.TotalPages,
            Criteria = Map(input.Criteria),
            Results = input.Results.Select(Map),
        };

    public Result<UserUpdateDTO, IEnumerable<FieldErrorDTO>> Map(UserUpdateMAPI input, Executor ex)
    {
        var errs = new List<FieldErrorDTO>();
        var roleValidation = _roleUintToDomainMapper.Map(input.Role);
        roleValidation.IfErr(_ => errs.Add(new() { Field = "role" }));

        if (errs.Count > 0)
            return errs;

        return new UserUpdateDTO
        {
            Id = input.Id,
            Active = input.Active,
            Role = roleValidation.Unwrap(),
            Email = input.Email,
            Password = input.Password,
            FirstName = input.FirstName,
            FatherLastName = input.FatherLastName,
            MidName = input.MidName.ToOptional(),
            MotherLastname = input.MotherLastname.ToOptional(),
            Executor = ex,
        };
    }

    public DeleteUserDTO Map(ulong userId, Executor ex) => new() { Id = userId, Executor = ex };
}
