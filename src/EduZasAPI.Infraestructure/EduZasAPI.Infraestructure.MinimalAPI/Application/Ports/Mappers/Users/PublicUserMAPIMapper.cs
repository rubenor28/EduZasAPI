using EduZasAPI.Domain.Users;

using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Users;

/// <summary>
/// Proporciona métodos de extensión para convertir instancias de <see cref="PublicUserDTO"/> 
/// en objetos de infraestructura <see cref="PublicUserMAPI"/>.
/// </summary>
public static class PublicUserMAPIMapper
{
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
}
