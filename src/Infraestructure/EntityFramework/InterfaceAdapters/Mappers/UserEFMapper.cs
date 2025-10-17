using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Users;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class UserEFMapper
    : IMapper<User, UserDomain>,
        IMapper<NewUserDTO, User>,
        IUpdateMapper<UserUpdateDTO, User>
{
    public UserDomain Map(User source) =>
        new()
        {
            Id = source.UserId,
            Active = source.Active ?? false,
            Email = source.Email,
            FatherLastName = source.FatherLastname,
            FirstName = source.FirstName,
            MidName = source.MidName.ToOptional(),
            MotherLastname = source.MotherLastname.ToOptional(),
            CreatedAt = source.CreatedAt,
            ModifiedAt = source.ModifiedAt,
            Password = source.Password,
            Role = source.Role is null
                ? UserType.STUDENT
                : UserTypeMapper.FromInt((int)source.Role).Unwrap(),
        };

    public User Map(NewUserDTO source) =>
        new()
        {
            Active = true,
            Email = source.Email,
            Password = source.Password,
            Role = (uint)UserType.STUDENT.ToInt().Unwrap(),
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
    }
}
