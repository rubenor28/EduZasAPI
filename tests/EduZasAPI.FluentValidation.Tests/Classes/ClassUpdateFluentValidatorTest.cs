
using EduZasAPI.Application.Classes;
using EduZasAPI.Application.Common;
using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Infraestructure.FluentValidation.Application.Classes;

namespace EduZasAPI.FluentValidation.Tests;

public class ClassUpdateFluentValidatorTest
{
    private readonly ClassUpdateFluentValidator _validator = new();

    [Fact]
    public void IsValid_WithValidData_ReturnsOk()
    {
        var dto = new ClassUpdateDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Active = true,
            Section = Optional.Some("Section A"),
            Subject = Optional.Some("Subject B"),
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR }
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsOk);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void IsValid_WithInvalidClassName_ReturnsError(string className)
    {
        var dto = new ClassUpdateDTO
        {
            Id = "test-class",
            ClassName = className,
            Color = "#ffffff",
            Active = true,
            Section = Optional.Some("Section A"),
            Subject = Optional.Some("Subject B"),
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR }
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "className");
    }

    [Fact]
    public void IsValid_WithInvalidSubject_ReturnsError()
    {
        var dto = new ClassUpdateDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Active = true,
            Section = Optional.Some("Section A"),
            Subject = Optional.Some("S"),
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR }
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "subject");
    }

    [Fact]
    public void IsValid_WithInvalidSection_ReturnsError()
    {
        var dto = new ClassUpdateDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Active = true,
            Section = Optional.Some("S"),
            Subject = Optional.Some("Subject B"),
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR }
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "section");
    }
}
