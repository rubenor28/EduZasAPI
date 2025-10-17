using Application.DTOs.Users;
using FluentValidationProj.Application.Services.Auth;

namespace FluentValidationTest.Auth;

public class UserCredentialsFluentValidatorTest
{
    private readonly UserCredentialsFluentValidator _validator = new();

    [Fact]
    public void IsValid_WithValidData_ReturnsOk()
    {
        var dto = new UserCredentialsDTO
        {
            Email = "john.doe@example.com",
            Password = "Password123!"
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsOk);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void IsValid_WithInvalidEmail_ReturnsError(string email)
    {
        var dto = new UserCredentialsDTO
        {
            Email = email,
            Password = "Password123!"
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "email");
    }

    [Fact]
    public void IsValid_WithEmptyPassword_ReturnsError()
    {
        var dto = new UserCredentialsDTO
        {
            Email = "john.doe@example.com",
            Password = ""
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "password");
    }
}
