
using EduZasAPI.Application.Users;
using EduZasAPI.Domain.Common;
using EduZasAPI.Infraestructure.FluentValidation.Application.Users;

namespace EduZasAPI.FluentValidation.Tests;

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
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.Some("FITZGERALD"),
            MotherLastname = Optional.Some("KENNEDY"),
            Active = true
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
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
            Active = true
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
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
            Active = true
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
        var dto = new UserUpdateDTO
        {
            Id = 1,
            FirstName = "JOHN",
            FatherLastName = fatherLastName,
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
            Active = true
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
        var dto = new UserUpdateDTO
        {
            Id = 1,
            FirstName = "JOHN",
            FatherLastName = "DOE",
            Email = email,
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
            Active = true
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
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = password,
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
            Active = true
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
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.Some(midName),
            MotherLastname = Optional.None<string>(),
            Active = true
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
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.Some(motherLastname),
            Active = true
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "motherLastname");
    }
}
