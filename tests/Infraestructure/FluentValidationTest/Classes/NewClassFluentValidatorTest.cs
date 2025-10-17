using Application.DTOs.Classes;
using FluentValidationProj.Application.Services.Classes;

namespace FluentValidationTest.Classes;

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
            Section = "Section A",
            Subject = "Subject B",
            OwnerId = 1,
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
            Section = "Section A",
            Subject = "Subject B",
            OwnerId = 1,
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
            Section = "Section A",
            Subject = "S",
            OwnerId = 1,
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
            Section = "S",
            Subject = "Subject B",
            OwnerId = 1,
        };

        var result = _validator.IsValid(dto);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "section");
    }
}
