namespace EduZasAPI.Application.Ports.DAOs;

using EduZasAPI.Domain.Entities;
using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Application.DTOs.Users;

public interface IUserRepositoryAsync : IRepositoryAsync<ulong, User, NewUserDTO, UserUpdateDTO, UserCriteriaDTO>
{
    public Task<Optional<User>> FindByEmail(string email);
}
