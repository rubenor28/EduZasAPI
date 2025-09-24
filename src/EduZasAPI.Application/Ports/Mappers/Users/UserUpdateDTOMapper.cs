using EduZasAPI.Domain.Users;

namespace EduZasAPI.Application.Users;

public static class UserUpdateDTOMapper
{
    public static UserUpdateDTO ToUserUpdateDTO(this UserDomain source) => new UserUpdateDTO
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
}
