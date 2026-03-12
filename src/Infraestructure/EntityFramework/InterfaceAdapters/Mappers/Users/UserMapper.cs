using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Users;

/// <summary>
/// Mapeador de entidad EF a dominio para usuarios.
/// </summary>
public class UserMapper : IMapper<User, UserDomain>
{
    private static UserType MapRole(uint? role) =>
        role switch
        {
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => UserType.STUDENT,
        };

    /// <summary>
    /// Mapea una entidad de base de datos a un objeto de dominio de usuario.
    /// </summary>
    /// <param name="source">Entidad de usuario de la base de datos.</param>
    /// <returns>Objeto de dominio del usuario.</returns>
    public UserDomain Map(User source) =>
        new()
        {
            Id = source.UserId,
            Active = source.Active ?? false,
            Email = source.Email,
            FatherLastname = source.FatherLastname,
            FirstName = source.FirstName,
            MidName = source.MidName,
            MotherLastname = source.MotherLastname,
            CreatedAt = source.CreatedAt,
            ModifiedAt = source.ModifiedAt,
            Password = source.Password,
            Role = MapRole(source.Role),
        };
}
