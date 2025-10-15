using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Users;
using EduZasAPI.Infraestructure.FluentValidation.Application.Users;

namespace EduZasAPI.FluentValidation.Tests;

public class RolChangeFluentValidatorTest
{
    private readonly RolChangeFluentValidator _validator = new();

    [Fact]
    public void IsValid_WithValidData_ReturnsOk()
    {
        var dto = new RolChangeDTO
        {
            Id = 1,
            Role = UserType.STUDENT
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsOk);
    }

    [Fact]
    public void IsValid_WithInvalidId_ReturnsError()
    {
        var dto = new RolChangeDTO
        {
            Id = 0,
            Role = UserType.STUDENT
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "id");
    }
}
