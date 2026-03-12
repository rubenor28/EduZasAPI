using Application.DTOs.Users;
using Domain.Enums;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Users;

/// <summary>
/// Mapeador de creación para usuarios.
/// </summary>
public class NewUserEFMapper(IMapper<UserType, uint> usrtMapper) : IMapper<NewUserDTO, User>
{
    private readonly IMapper<UserType, uint> _usrtMapper = usrtMapper;

    /// <summary>
    /// Mapea un DTO de nuevo usuario a una entidad de base de datos.
    /// </summary>
    /// <param name="source">DTO con los datos del nuevo usuario.</param>
    /// <returns>Entidad de usuario para persistencia.</returns>
    public User Map(NewUserDTO source) =>
        new()
        {
            Active = true,
            Email = source.Email,
            Password = source.Password,
            Role = _usrtMapper.Map(source.Role),
            FirstName = source.FirstName,
            FatherLastname = source.FatherLastname,
            MidName = source.MidName,
            MotherLastname = source.MotherLastname,
        };
}
