using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;

using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Tests.EntityFramework;

public class UserEFRepository
{
    private readonly IRepositoryAsync<ulong, UserDomain, NewUserDTO, UserUpdateDTO, DeleteUserDTO, UserCriteriaDTO> _repository;

    public UserEFRepository()
    {
        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>()
          .UseInMemoryDatabase("TestDB")
          .Options;

        var ctx = new EduZasDotnetContext(opts);
        _repository = new UserEntityFrameworkRepository(ctx, 10);
    }

    [Fact]
    public async Task AddUser_RetunsUser()
    {
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastName = "test",
        };

        var created = await _repository.AddAsync(newUser);
        Assert.NotNull(created);
    }
}
