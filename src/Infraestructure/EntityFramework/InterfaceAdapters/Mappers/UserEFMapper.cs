using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class UserEFMapper(
    IMapper<uint, Result<UserType, Unit>> userTypeToDomainMapper,
    IMapper<UserType, uint> userTypeFromDomainMapper
) : IMapper<User, UserDomain>, IMapper<NewUserDTO, User>, IUpdateMapper<UserUpdateDTO, User>
{
    private readonly IMapper<uint, Result<UserType, Unit>> _userTypeToDomainMapper =
        userTypeToDomainMapper;
    private readonly IMapper<UserType, uint> _userTypeFromDomainMapper = userTypeFromDomainMapper;

    public UserDomain Map(User source) =>
        new()
        {
            Id = source.UserId,
            Active = source.Active ?? false,
            Email = source.Email,
            FatherLastname = source.FatherLastname,
            FirstName = source.FirstName,
            MidName = source.MidName.ToOptional(),
            MotherLastname = source.MotherLastname.ToOptional(),
            CreatedAt = source.CreatedAt,
            ModifiedAt = source.ModifiedAt,
            Password = source.Password,
            Role = source.Role is null
                ? UserType.STUDENT
                : _userTypeToDomainMapper.Map((uint)source.Role).Unwrap(),
        };

    public User Map(NewUserDTO source) =>
        new()
        {
            Active = true,
            Email = source.Email,
            Password = source.Password,
            Role = _userTypeFromDomainMapper.Map(UserType.STUDENT),
            FirstName = source.FirstName,
            FatherLastname = source.FatherLastName,
            MidName = source.MidName.ToNullable(),
            MotherLastname = source.MotherLastname.ToNullable(),
        };

    public void Map(UserUpdateDTO source, User destination)
    {
        destination.UserId = source.Id;
        destination.FirstName = source.FirstName;
        destination.FatherLastname = source.FatherLastName;
        destination.Email = source.Email;
        destination.Password = source.Password;
        destination.MidName = source.MidName.ToNullable();
        destination.MotherLastname = source.MotherLastname.ToNullable();
        destination.Active = source.Active;
        destination.Role = _userTypeFromDomainMapper.Map(source.Role);
    }
}
