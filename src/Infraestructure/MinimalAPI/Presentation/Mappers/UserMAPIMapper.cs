using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Users;

namespace MinimalAPI.Presentation.Mappers;

public sealed class UserReadMAPIMapper : IMapper<Executor, ReadUserDTO>
{
    public ReadUserDTO Map(Executor input) => new() { Id = input.Id, Executor = input };
}

public sealed class UserMAPIMapper(IMapper<UserType, uint> roleMapper)
    : IMapper<UserDomain, PublicUserMAPI>
{
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
            FatherLastname = source.FatherLastname,
            Email = source.Email,
            MotherLastname = source.MotherLastname.ToNullable(),
            MidName = source.MidName.ToNullable(),
            Role = roleMapper.Map(source.Role),
        };
}

public sealed class NewUserMAPIMapper : IMapper<NewUserMAPI, NewUserDTO>
{
    /// <summary>
    /// Mapea los datos de un nuevo usuario desde la API y el ejecutor de la acción a un DTO para el caso de uso de creación.
    /// </summary>
    /// <param name="input">El DTO de la API con los datos del nuevo usuario.</param>
    /// <param name="ex">El <see cref="Executor"/> que realiza la operación.</param>
    /// <returns>Un <see cref="NewUserDTO"/> para la capa de aplicación.</returns>
    public NewUserDTO Map(NewUserMAPI input) =>
        new()
        {
            FirstName = input.FirstName,
            FatherLastname = input.FatherLastname,
            Email = input.Email,
            Password = input.Password,
            MotherLastname = input.MotherLastname.ToOptional(),
            MidName = input.MidName.ToOptional(),
        };
}

public sealed class UserUpdateMAPIMapper(IMapper<uint, Result<UserType, Unit>> roleMapper)
    : IMapper<UserUpdateMAPI, Executor, Result<UserUpdateDTO, IEnumerable<FieldErrorDTO>>>
{
    public Result<UserUpdateDTO, IEnumerable<FieldErrorDTO>> Map(UserUpdateMAPI input, Executor ex)
    {
        var errs = new List<FieldErrorDTO>();
        var roleValidation = roleMapper.Map(input.Role);
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
            FatherLastname = input.FatherLastname,
            MidName = input.MidName.ToOptional(),
            MotherLastname = input.MotherLastname.ToOptional(),
            Executor = ex,
        };
    }
}

public sealed class DeleteUserMAPIMapper : IMapper<ulong, Executor, DeleteUserDTO>
{
    public DeleteUserDTO Map(ulong userId, Executor ex) => new() { Id = userId, Executor = ex };
}

public sealed class UserCriteriaMAPIMapper(
    IBidirectionalResultMapper<StringQueryMAPI?, Optional<StringQueryDTO>, Unit> strqMapper,
    IBidirectionalResultMapper<uint?, Optional<UserType>, Unit> roleMapper
)
    : IBidirectionalResultMapper<UserCriteriaMAPI, UserCriteriaDTO, IEnumerable<FieldErrorDTO>>,
        IMapper<UserCriteriaDTO, UserCriteriaMAPI>
{
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
        var firstNameValidation = strqMapper.Map(source.FirstName);
        var midNameValidation = strqMapper.Map(source.MidName);
        var fatherLastnameValidation = strqMapper.Map(source.FatherLastname);
        var motherLastnameValidation = strqMapper.Map(source.MotherLastname);
        var emailValidation = strqMapper.Map(source.Email);
        var passwordValidation = strqMapper.Map(source.Password);
        var roleValidation = roleMapper.Map(source.Role);

        firstNameValidation.IfErr(_ => errs.Add(new() { Field = "fistName" }));
        midNameValidation.IfErr(_ => errs.Add(new() { Field = "midName" }));
        fatherLastnameValidation.IfErr(_ => errs.Add(new() { Field = "fatherLastname" }));
        motherLastnameValidation.IfErr(_ => errs.Add(new() { Field = "motherLastname" }));
        emailValidation.IfErr(_ => errs.Add(new() { Field = "email" }));
        passwordValidation.IfErr(_ => errs.Add(new() { Field = "password" }));
        roleValidation.IfErr(_ => errs.Add(new() { Field = "role" }));

        if (errs.Count > 0)
            return errs;

        return new UserCriteriaDTO
        {
            FirstName = firstNameValidation.Unwrap(),
            MidName = midNameValidation.Unwrap(),
            FatherLastname = fatherLastnameValidation.Unwrap(),
            MotherLastname = motherLastnameValidation.Unwrap(),
            Email = emailValidation.Unwrap(),
            Password = passwordValidation.Unwrap(),
            Role = roleValidation.Unwrap(),
            Page = source.Page,
            Active = source.Active.ToOptional(),
            EnrolledInClass = source.EnrolledInClass.ToOptional(),
            TeachingInClass = source.TeachingInClass.ToOptional(),
            CreatedAt = source.CreatedAt.ToOptional(),
            ModifiedAt = source.CreatedAt.ToOptional(),
        };
    }

    public UserCriteriaMAPI Map(UserCriteriaDTO input) => MapFrom(input);

    public UserCriteriaMAPI MapFrom(UserCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            Role = roleMapper.MapFrom(input.Role),
            Active = input.Active.ToNullable(),
            Email = strqMapper.MapFrom(input.Email),
            Password = strqMapper.MapFrom(input.Password),
            FirstName = strqMapper.MapFrom(input.FirstName),
            FatherLastname = strqMapper.MapFrom(input.FatherLastname),
            MidName = strqMapper.MapFrom(input.MidName),
            MotherLastname = strqMapper.MapFrom(input.MotherLastname),
            CreatedAt = input.CreatedAt.ToNullable(),
            ModifiedAt = input.ModifiedAt.ToNullable(),
        };
}

public sealed class UserSearchMAPIMapper(
    IMapper<UserDomain, PublicUserMAPI> usrMapper,
    IMapper<UserCriteriaDTO, UserCriteriaMAPI> cMapper
)
    : IMapper<
        PaginatedQuery<UserDomain, UserCriteriaDTO>,
        PaginatedQuery<PublicUserMAPI, UserCriteriaMAPI>
    >
{
    public PaginatedQuery<PublicUserMAPI, UserCriteriaMAPI> Map(
        PaginatedQuery<UserDomain, UserCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            TotalPages = input.TotalPages,
            Criteria = cMapper.Map(input.Criteria),
            Results = input.Results.Select(usrMapper.Map),
        };
}
