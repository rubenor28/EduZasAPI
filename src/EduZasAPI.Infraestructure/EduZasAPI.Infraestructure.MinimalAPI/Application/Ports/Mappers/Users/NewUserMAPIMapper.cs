using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Users;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Users;

/// <summary>
/// Proporciona métodos de extensión para convertir instancias de <see cref="NewUserMAPI"/> 
/// en objetos de dominio <see cref="NewUserDTO"/>.
/// </summary>
public static class NewUserMAPIMapper
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
}
