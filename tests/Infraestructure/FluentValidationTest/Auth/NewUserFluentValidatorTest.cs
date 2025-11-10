using Application.DTOs.Users;
using Domain.Enums;
using Domain.ValueObjects;
using FluentValidationProj.Application.Services.Auth;

namespace FluentValidationTest.Auth;

public class NewUserFluentValidatorTest
{
    private readonly NewUserFluentValidator _validator = new();

    [Fact]
    public void IsValid_WithValidData_ReturnsOk()
    {
        var dto = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = "FITZGERALD",
            MotherLastname = "KENNEDY",
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsOk);
    }

    [Theory]
    [InlineData("")]
    [InlineData("JO")]
    [InlineData("john")]
    public void IsValid_WithInvalidFirstName_ReturnsError(string firstName)
    {
        var dto = new NewUserDTO
        {
            FirstName = firstName,
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "firstName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("DO")]
    [InlineData("doe")]
    public void IsValid_WithInvalidFatherLastName_ReturnsError(string fatherLastName)
    {
        var dto = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastName = fatherLastName,
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "fatherLastName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void IsValid_WithInvalidEmail_ReturnsError(string email)
    {
        var dto = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastName = "DOE",
            Email = email,
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
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
        var dto = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = password,
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
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
        var dto = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.Some(midName),
            MotherLastname = Optional.None<string>(),
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
        var dto = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.Some(motherLastname),
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "motherLastname");
    }
}
