using Application.DTOs.Users;
using Domain.Enums;
using FluentValidationProj.Application.Services.Users;

namespace FluentValidationTest.Users;

public class UserUpdateFluentValidatorTest
{
    private readonly UserUpdateFluentValidator _validator = new();

    [Fact]
    public void IsValid_WithValidData_ReturnsOk()
    {
        var dto = new UserUpdateDTO
        {
            Id = 1,
            FirstName = "JOHN",
            FatherLastname = "LUKE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = "FITZGERALD",
            MotherLastname = "KENNEDY",
            Active = true,
            Role = UserType.ADMIN,
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsOk);
    }

    [Fact]
    public void IsValid_WithInvalidId_ReturnsError()
    {
        var dto = new UserUpdateDTO
        {
            Id = 0,
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            Active = true,
            Role = UserType.ADMIN,
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "id");
    }

    [Theory]
    [InlineData("")]
    [InlineData("JO")]
    [InlineData("john")]
    public void IsValid_WithInvalidFirstName_ReturnsError(string firstName)
    {
        var dto = new UserUpdateDTO
        {
            Id = 1,
            FirstName = firstName,
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            Active = true,
            Role = UserType.ADMIN,
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "firstName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("DO")]
    [InlineData("doe")]
    public void IsValid_WithInvalidFatherLastname_ReturnsError(string fatherLastname)
    {
        var dto = new UserUpdateDTO
        {
            Id = 1,
            FirstName = "JOHN",
            FatherLastname = fatherLastname,
            Email = "john.doe@example.com",
            Password = "Password123!",
            Active = true,
            Role = UserType.ADMIN,
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "fatherLastname");
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void IsValid_WithInvalidEmail_ReturnsError(string email)
    {
        var dto = new UserUpdateDTO
        {
            Id = 1,
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = email,
            Password = "Password123!",
            Active = true,
            Role = UserType.ADMIN,
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("nouppercase1!")]
    [InlineData("NOLOWERCASE1!")]
    [InlineData("NoSpecialChar1")]
    public void IsValid_WithInvalidPassword_ReturnsError(string password)
    {
        var dto = new UserUpdateDTO
        {
            Id = 1,
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = password,
            Active = true,
            Role = UserType.ADMIN,
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "password");
    }

    [Theory]
    [InlineData("jo")]
    [InlineData("123")]
    public void IsValid_WithInvalidMidName_ReturnsError(string midName)
    {
        var dto = new UserUpdateDTO
        {
            Id = 1,
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = midName,
            Active = true,
            Role = UserType.ADMIN,
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "midName");
    }

    [Theory]
    [InlineData("ke")]
    [InlineData("456")]
    public void IsValid_WithInvalidMotherLastname_ReturnsError(string motherLastname)
    {
        var dto = new UserUpdateDTO
        {
            Id = 1,
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MotherLastname = motherLastname,
            Active = true,
            Role = UserType.ADMIN,
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "motherLastname");
    }
}
