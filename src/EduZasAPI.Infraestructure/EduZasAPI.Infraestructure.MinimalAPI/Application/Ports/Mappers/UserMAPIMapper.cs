using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Users;

/// <summary>
/// Proporciona métodos de extensión para mapear entre DTOs de la API mínima y DTOs de dominio.
/// </summary>
public static class UserMAPIMapper
{
    /// <summary>
    /// Convierte una instancia de <see cref="NewUserMAPI"/> en un objeto de dominio <see cref="NewUserDTO"/>.
    /// </summary>
    /// <param name="source">Instancia de <see cref="NewUserMAPI"/> a convertir.</param>
    /// <returns>
    /// Un <see cref="NewUserDTO"/> con los valores correspondientes mapeados desde <paramref name="source"/>.
    /// Los campos opcionales (<c>MotherLastname</c>, <c>MidName</c>) se convierten a <see cref="Optional{T}"/>.
    /// </returns>
    public static NewUserDTO ToDomain(this NewUserMAPI source) => new NewUserDTO
    {
        FirstName = source.FirstName,
        FatherLastName = source.FatherLastName,
        Email = source.Email,
        Password = source.Password,
        MotherLastname = source.MotherLastname.ToOptional(),
        MidName = source.MidName.ToOptional(),
    };

    /// <summary>
    /// Convierte una instancia de <see cref="PublicUserDTO"/> en un objeto de infraestructura <see cref="PublicUserMAPI"/>.
    /// </summary>
    /// <param name="source">Instancia de <see cref="PublicUserDTO"/> a convertir.</param>
    /// <returns>
    /// Un <see cref="PublicUserMAPI"/> con los valores correspondientes mapeados desde <paramref name="source"/>.
    /// Los valores opcionales (<c>MotherLastname</c>, <c>MidName</c>) se convierten a tipos anulables, 
    /// y el rol se transforma a un entero mediante el mapper de enums.
    /// </returns>
    public static PublicUserMAPI FromDomain(this PublicUserDTO source) => new PublicUserMAPI
    {
        Id = source.Id,
        FirstName = source.FirstName,
        FatherLastName = source.FatherLastName,
        Email = source.Email,
        MotherLastname = source.MotherLastname.ToNullable(),
        MidName = source.MidName.ToNullable(),
        Role = source.Role.ToInt().Unwrap()
    };

    /// <summary>
    /// Convierte una instancia de <see cref="UserDomain"/> en un objeto de infraestructura <see cref="PublicUserMAPI"/>.
    /// </summary>
    /// <param name="source">Instancia de <see cref="UserDomain"/> a convertir.</param>
    /// <returns>
    /// Un <see cref="PublicUserMAPI"/> con los valores correspondientes mapeados desde <paramref name="source"/>.
    /// Los valores opcionales (<c>MotherLastname</c>, <c>MidName</c>) se convierten a tipos anulables,
    /// y el rol se transforma a un entero mediante el mapper de enums.
    /// </returns>
    public static PublicUserMAPI FromDomain(this UserDomain source) => new PublicUserMAPI
    {
        Id = source.Id,
        FirstName = source.FirstName,
        FatherLastName = source.FatherLastName,
        Email = source.Email,
        MotherLastname = source.MotherLastname.ToNullable(),
        MidName = source.MidName.ToNullable(),
        Role = source.Role.ToInt().Unwrap()
    };

    /// <summary>
    /// Convierte una instancia de <see cref="RolChangeMAPI"/> en un <see cref="RolChangeDTO"/>.
    /// </summary>
    /// <param name="value">Instancia de <see cref="RolChangeMAPI"/> a convertir.</param>
    /// <returns>
    /// Un objeto <see cref="RolChangeDTO"/> con el identificador y el rol
    /// mapeados al modelo de dominio.  
    /// Si el rol no se puede convertir, se asigna el valor por defecto <c>0</c>.
    /// </returns>
    public static RolChangeDTO ToDomain(this RolChangeMAPI value) => new RolChangeDTO
    {
        Id = value.Id,
        Role = UserTypeMapper.FromInt(value.Role).UnwrapOr(0),
    };

    /// <summary>
    /// Convierte un objeto <see cref="UserCriteriaMAPI"/> en un <see cref="UserCriteriaDTO"/> de dominio.
    /// </summary>
    /// <param name="source">Instancia de <see cref="UserCriteriaMAPI"/> a mapear.</param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene el <see cref="UserCriteriaDTO"/> si la conversión
    /// fue exitosa, o una lista de <see cref="FieldErrorDTO"/> si se encontraron errores de formato.
    /// </returns>
    public static Result<UserCriteriaDTO, List<FieldErrorDTO>> ToDomain(this UserCriteriaMAPI source)
    {
        var role = Optional<UserType>.None();
        var firstName = Optional<StringQueryDTO>.None();
        var midName = Optional<StringQueryDTO>.None();
        var fatherLastName = Optional<StringQueryDTO>.None();
        var motherLastname = Optional<StringQueryDTO>.None();
        var email = Optional<StringQueryDTO>.None();
        var password = Optional<StringQueryDTO>.None();

        var errs = new List<FieldErrorDTO>();
        StringQueryMAPIMapper.ParseStringQuery(source.FirstName, "firstName", firstName, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.MidName, "midName", midName, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.FatherLastName, "fatherLastName", fatherLastName, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.MotherLastname, "motherLastname", motherLastname, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.Email, "email", email, errs);
        StringQueryMAPIMapper.ParseStringQuery(source.Password, "password", password, errs);

        if (errs.Count > 0)
            return Result.Err<UserCriteriaDTO, List<FieldErrorDTO>>(errs);

        return Result.Ok<UserCriteriaDTO, List<FieldErrorDTO>>(new UserCriteriaDTO
        {
            FirstName = firstName,
            MidName = midName,
            FatherLastName = fatherLastName,
            MotherLastname = motherLastname,
            Email = email,
            Password = password,
            Role = role,
            Page = source.Page,
            Active = source.Active.ToOptional(),
            CreatedAt = source.CreatedAt.ToOptional(),
            ModifiedAt = source.CreatedAt.ToOptional(),
        });
    }
}
