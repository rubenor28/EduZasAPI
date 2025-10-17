using Application.DTOs.Users;
using Domain.Entities;

namespace InterfaceAdapters.Mappers.Users;

public static class UserDomainMapper
{
    public static UserUpdateDTO ToUserUpdateDTO(this UserDomain source) =>
        new()
        {
            Id = source.Id,
            Active = source.Active,
            Email = source.Email,
            FirstName = source.FirstName,
            MidName = source.MidName,
            FatherLastName = source.FatherLastName,
            Password = source.Password,
            MotherLastname = source.MotherLastname,
        };

    public static PublicUserDTO ToPublicUserDTO(this UserDomain source) =>
        new ()
        {
            Id = source.Id,
            Email = source.Email,
            Role = source.Role,
            FirstName = source.FirstName,
            MidName = source.MidName,
            FatherLastName = source.FatherLastName,
            MotherLastname = source.MotherLastname,
        };
}
