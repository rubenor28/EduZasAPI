using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Users;

namespace MinimalAPI.Presentation.Mappers;

public sealed class NewUserMAPIMapper(IMapper<uint, Result<UserType, Unit>> roleMapper)
    : IMapper<NewUserMAPI, Result<NewUserDTO, IEnumerable<FieldErrorDTO>>>
{
    public Result<NewUserDTO, IEnumerable<FieldErrorDTO>> Map(NewUserMAPI input)
    {
        var roleParse = roleMapper.Map(input.Role);
        if (roleParse.IsErr)
            return new FieldErrorDTO[] { new() { Field = "role" } };

        return new NewUserDTO
        {
            Role = roleParse.Unwrap(),
            Email = input.Email,
            FatherLastname = input.FatherLastname,
            FirstName = input.FirstName,
            MidName = input.MidName,
            MotherLastname = input.MotherLastname,
            Password = input.Password
        };
    }
}

public sealed class UserMAPIMapper(IMapper<UserType, uint> roleMapper)
    : IMapper<UserDomain, PublicUserDTO>
{
    /// <summary>
    /// Mapea una entidad de dominio de usuario a un DTO público para la API.
    /// </summary>
    /// <param name="source">La entidad <see cref="UserDomain"/>.</param>
    /// <returns>Un <see cref="PublicUserDTO"/> con datos públicos del usuario.</returns>
    public PublicUserDTO Map(UserDomain source) =>
        new()
        {
            Id = source.Id,
            FirstName = source.FirstName,
            FatherLastname = source.FatherLastname,
            Email = source.Email,
            MotherLastname = source.MotherLastname,
            MidName = source.MidName,
            Role = roleMapper.Map(source.Role),
        };
}

public sealed class UserUpdateMAPIMapper(IMapper<uint, Result<UserType, Unit>> roleMapper)
    : IMapper<UserUpdateMAPI, Result<UserUpdateDTO, IEnumerable<FieldErrorDTO>>>
{
    public Result<UserUpdateDTO, IEnumerable<FieldErrorDTO>> Map(UserUpdateMAPI input)
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
            MidName = input.MidName,
            MotherLastname = input.MotherLastname,
        };
    }
}

public sealed class UserCriteriaMAPIMapper(
    IBidirectionalResultMapper<StringQueryMAPI?, StringQueryDTO?, Unit> strqMapper,
    IBidirectionalResultMapper<uint?, UserType?, Unit> roleMapper
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
            Active = source.Active,
            EnrolledInClass = source.EnrolledInClass,
            TeachingInClass = source.TeachingInClass,
            CreatedAt = source.CreatedAt,
            ModifiedAt = source.CreatedAt,
        };
    }

    public UserCriteriaMAPI Map(UserCriteriaDTO input) => MapFrom(input);

    public UserCriteriaMAPI MapFrom(UserCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            Role = roleMapper.MapFrom(input.Role),
            Active = input.Active,
            Email = strqMapper.MapFrom(input.Email),
            Password = strqMapper.MapFrom(input.Password),
            FirstName = strqMapper.MapFrom(input.FirstName),
            FatherLastname = strqMapper.MapFrom(input.FatherLastname),
            MidName = strqMapper.MapFrom(input.MidName),
            MotherLastname = strqMapper.MapFrom(input.MotherLastname),
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
        };
}

public sealed class UserSearchMAPIMapper(
    IMapper<UserDomain, PublicUserDTO> usrMapper,
    IMapper<UserCriteriaDTO, UserCriteriaMAPI> cMapper
)
    : IMapper<
        PaginatedQuery<UserDomain, UserCriteriaDTO>,
        PaginatedQuery<PublicUserDTO, UserCriteriaMAPI>
    >
{
    public PaginatedQuery<PublicUserDTO, UserCriteriaMAPI> Map(
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
