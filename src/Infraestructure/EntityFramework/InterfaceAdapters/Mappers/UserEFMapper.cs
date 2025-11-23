using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class UserEFMapper(IMapper<UserType, uint> usrtMapper)
    : IMapper<User, UserDomain>,
        IMapper<NewUserDTO, User>,
        IUpdateMapper<UserUpdateDTO, User>
{
    private readonly IMapper<UserType, uint> _usrtMapper = usrtMapper;

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
            Role = source.Role switch
            {
                1 => UserType.PROFESSOR,
                2 => UserType.ADMIN,
                _ => UserType.STUDENT,
            },
        };

    public User Map(NewUserDTO source) =>
        new()
        {
            Active = true,
            Email = source.Email,
            Password = source.Password,
            Role = _usrtMapper.Map(UserType.STUDENT),
            FirstName = source.FirstName,
            FatherLastname = source.FatherLastname,
            MidName = source.MidName.ToNullable(),
            MotherLastname = source.MotherLastname.ToNullable(),
        };

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
