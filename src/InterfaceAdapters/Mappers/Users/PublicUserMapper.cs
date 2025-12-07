using Application.DTOs.Users;
using Domain.Entities;
using InterfaceAdapters.Mappers.Common;

namespace InterfaceAdapters.Mappers.Users;

public sealed class PublicUserMapper : IMapper<UserDomain, PublicUserDTO>
{
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
