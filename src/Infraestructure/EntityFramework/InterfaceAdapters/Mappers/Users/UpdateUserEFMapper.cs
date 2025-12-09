using Application.DTOs.Users;
using Domain.Enums;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Users;

/// <summary>
/// Mapeador de actualización para usuarios.
/// </summary>
public class UpdateUserEFMapper(IMapper<UserType, uint> usrtMapper)
    : IUpdateMapper<UserUpdateDTO, User>
{
    private readonly IMapper<UserType, uint> _usrtMapper = usrtMapper;

    /// <inheritdoc/>
    public void Map(UserUpdateDTO source, User destination)
    {
        destination.UserId = source.Id;
        destination.FirstName = source.FirstName;
        destination.FatherLastname = source.FatherLastname;
        destination.Email = source.Email;
        destination.Password = source.Password ?? destination.Password;
        destination.MidName = source.MidName;
        destination.MotherLastname = source.MotherLastname;
        destination.Active = source.Active;
        destination.Role = _usrtMapper.Map(source.Role);
    }
}
