
using EduZasAPI.Application.Classes;
using EduZasAPI.Domain.Common;
using EduZasAPI.Infraestructure.FluentValidation.Application.Classes;

namespace EduZasAPI.FluentValidation.Tests;

public class NewClassFluentValidatorTest
{
    private readonly NewClassFluentValidator _validator = new();

    [Fact]
    public void IsValid_WithValidData_ReturnsOk()
    {
        var dto = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("Section A"),
            Subject = Optional.Some("Subject B"),
            OwnerId = 1
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsOk);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void IsValid_WithInvalidClassName_ReturnsError(string className)
    {
        var dto = new NewClassDTO
        {
            Id = "test-class",
            ClassName = className,
            Color = "#ffffff",
            Section = Optional.Some("Section A"),
            Subject = Optional.Some("Subject B"),
            OwnerId = 1
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "className");
    }

    [Fact]
    public void IsValid_WithInvalidSubject_ReturnsError()
    {
        var dto = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("Section A"),
            Subject = Optional.Some("S"),
            OwnerId = 1
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "subject");
    }

    [Fact]
    public void IsValid_WithInvalidSection_ReturnsError()
    {
        var dto = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("S"),
            Subject = Optional.Some("Subject B"),
            OwnerId = 1
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "section");
    }
}
