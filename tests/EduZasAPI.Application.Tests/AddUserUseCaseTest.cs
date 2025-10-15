
using EduZasAPI.Application.Auth;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Domain.Common;
using EduZasAPI.Infraestructure.Bcrypt.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;
using EduZasAPI.Infraestructure.FluentValidation.Application.Auth;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Application.Tests;

public class AddUserUseCaseTest : IDisposable
{
    private readonly AddUserUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public AddUserUseCaseTest()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>()
          .UseSqlite(_conn)
          .Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var hasher = new BCryptHasher();
        var repository = new UserEntityFrameworkRepository(_ctx, 10);
        var validator = new NewUserFluentValidator();

        _useCase = new AddUserUseCase(hasher, repository, validator, repository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ReturnsOk()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>()
        };

        var result = await _useCase.ExecuteAsync(newUser);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateEmail_ReturnsError()
    {
        var existingUser = new NewUserDTO
        {
            FirstName = "JANE",
            FatherLastName = "DOE",
            Email = "jane.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>()
        };
        await _useCase.ExecuteAsync(existingUser);

        var newUser = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastName = "DOE",
            Email = "jane.doe@example.com", // Duplicate email
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>()
        };

        var result = await _useCase.ExecuteAsync(newUser);

        Assert.True(result.IsErr);
        var error = result.UnwrapErr();

        Assert.Equal(typeof(InputError), error.GetType());
        Assert.Contains(((InputError)error).Errors, e => e.Field == "email");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidData_ReturnsError()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "", // Invalid first name
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>()
        };

        var result = await _useCase.ExecuteAsync(newUser);

        Assert.True(result.IsErr);
        var error = result.UnwrapErr();
        Assert.Equal(typeof(InputError), error.GetType());
        Assert.Contains(((InputError)error).Errors, e => e.Field == "firstName");
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
