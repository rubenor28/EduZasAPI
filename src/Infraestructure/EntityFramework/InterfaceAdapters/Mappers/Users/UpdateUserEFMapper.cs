using Application.DTOs.Users;
using Domain.Enums;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Users;

public class UpdateUserEFMapper(IMapper<UserType, uint> usrtMapper)
    : IUpdateMapper<UserUpdateDTO, User>
{
    private readonly IMapper<UserType, uint> _usrtMapper = usrtMapper;

    public void Map(UserUpdateDTO source, User destination)
    {
        destination.UserId = source.Id;
        destination.FirstName = source.FirstName;
        destination.FatherLastname = source.FatherLastname;
        destination.Email = source.Email;
        destination.Password = source.Password;
        destination.MidName = source.MidName.ToNullable();
        destination.MotherLastname = source.MotherLastname.ToNullable();
        destination.Active = source.Active;
        destination.Role = _usrtMapper.Map(source.Role);
    }
}
