using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.UseCases.Users;
using Bcrypt.Application.Services;
using Domain.Enums;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using FluentValidationProj.Application.Services.Auth;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Auth;

public class AddUserUseCaseTest : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly AddUserUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;

    public AddUserUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var hasher = new BCryptHasher();
        var roleMapper = new UserTypeUintMapper();
        var mapper = new UserMapper();
        var creator = new UserEFCreator(_ctx, mapper, new NewUserEFMapper(roleMapper));
        var reader = new UserEmailEFReader(_ctx, mapper);
        var validator = new NewUserFluentValidator();

        _useCase = new AddUserUseCase(hasher, creator, validator, reader);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ReturnsOk()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            Role = UserType.STUDENT,
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newUser,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateEmail_ReturnsError()
    {
        var existingUser = new NewUserDTO
        {
            FirstName = "JANE",
            FatherLastname = "DOE",
            Email = "jane.doe@example.com",
            Password = "Password123!",
            Role = UserType.STUDENT,
        };
        await _useCase.ExecuteAsync(
            new()
            {
                Data = existingUser,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        var newUser = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = "jane.doe@example.com", // Duplicate email
            Password = "Password123!",
            Role = UserType.STUDENT,
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newUser,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsErr);
        Assert.IsType<Conflict>(result.UnwrapErr());
        var err = (Conflict)result.UnwrapErr();
        Assert.Equal("El recurso ya existe", err.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidData_ReturnsError()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "", // Invalid first name
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            Role = UserType.STUDENT,
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newUser,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

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
