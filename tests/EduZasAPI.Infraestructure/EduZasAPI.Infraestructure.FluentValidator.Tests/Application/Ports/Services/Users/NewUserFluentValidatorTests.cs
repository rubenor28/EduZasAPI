using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Infraestructure.FluentValidation.Application.Users;

namespace EduZasAPI.Tests.Infraestructure.FluentValidation.Application.Users;

public class NewUserFluentValidatorTests
{
    private readonly IBusinessValidationService<NewUserDTO> _validator;

    public NewUserFluentValidatorTests()
    {
        _validator = new NewUserFluentValidator();
    }

    [Fact]
    public void Validate_ValidUser_ShouldReturnOk()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "JUAN",
            FatherLastName = "PEREZ",
            Email = "juan.perez@example.com",
            Password = "Aa123456!",
            MidName = Optional<string>.Some("CARLOS"),
            MotherLastname = Optional<string>.Some("GOMEZ")
        };

        var result = _validator.IsValid(newUser);

        Console.WriteLine(result.IsOk);
        Console.WriteLine(result.IsOk);
        Assert.True(result.IsOk, "Expected validation to succeed for a valid user.");
    }

    [Fact]
    public void Validate_EmptyFirstName_ShouldReturnError()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "",
            FatherLastName = "PEREZ",
            Email = "juan.perez@example.com",
            Password = "Aa123456!",
        };

        var result = _validator.IsValid(newUser);

        Assert.True(result.IsErr, "Expected validation to fail when FirstName is empty.");
        Assert.Contains(result.UnwrapErr(), e => e.Field == "FirstName");
    }

    [Fact]
    public void Validate_InvalidEmail_ShouldReturnError()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "JUAN",
            FatherLastName = "PEREZ",
            Email = "not-an-email",
            Password = "Aa123456!"
        };

        var result = _validator.IsValid(newUser);

        Assert.True(result.IsErr, "Expected validation to fail with invalid email.");
        Assert.Contains(result.UnwrapErr(), e => e.Field == "Email");
    }

    [Fact]
    public void Validate_OptionalFieldsEmpty_ShouldReturnOk()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "JUAN",
            FatherLastName = "PEREZ",
            Email = "juan.perez@example.com",
            Password = "Aa123456!",
            MidName = Optional<string>.None(),
            MotherLastname = Optional<string>.None()
        };

        var result = _validator.IsValid(newUser);

        Assert.True(result.IsOk, "Validation should succeed when optional fields are not provided.");
    }
}

