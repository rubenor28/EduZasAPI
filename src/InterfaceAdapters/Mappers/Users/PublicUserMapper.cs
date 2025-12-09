using Application.DTOs.Users;
using Domain.Entities;
using InterfaceAdapters.Mappers.Common;

namespace InterfaceAdapters.Mappers.Users;

/// <summary>
/// Mapeador para transformar una entidad de usuario de dominio a un DTO público.
/// </summary>
public sealed class PublicUserMapper : IMapper<UserDomain, PublicUserDTO>
{
    /// <summary>
    /// Mapea un usuario de dominio a su representación pública.
    /// </summary>
    /// <param name="input">Entidad de usuario.</param>
    /// <returns>DTO con información pública del usuario.</returns>
    public PublicUserDTO Map(UserDomain input) =>
        new()
        {
            Id = input.Id,
            Active = input.Active,
            Email = input.Email,
            FatherLastname = input.FatherLastname,
            FirstName = input.FirstName,
            MidName = input.MidName,
            MotherLastname = input.MotherLastname,
            Role = (ulong)input.Role,
        };
}
