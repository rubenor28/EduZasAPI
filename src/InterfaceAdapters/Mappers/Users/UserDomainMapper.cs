using Application.DTOs.Users;
using Domain.Entities;
using InterfaceAdapters.Mappers.Common;

namespace InterfaceAdapters.Mappers.Users;

public class UserDomainMapper : IMapper<UserDomain, PublicUserDTO>
{
    public PublicUserDTO Map(UserDomain source) =>
        new()
        {
            Id = source.Id,
            Email = source.Email,
            Role = source.Role,
            FirstName = source.FirstName,
            MidName = source.MidName,
            FatherLastName = source.FatherLastname,
            MotherLastname = source.MotherLastname,
        };
}
