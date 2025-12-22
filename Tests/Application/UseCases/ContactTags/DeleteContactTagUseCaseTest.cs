using Application.DTOs.Common;
using Application.UseCases.ContactTags;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.UseCases.ContactTags;

public class DeleteContactTagUseCaseTest : BaseTest
{
    private readonly DeleteContactTagUseCase _useCase;
    private readonly EduZasDotnetContext _context;

    public DeleteContactTagUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<DeleteContactTagUseCase>();
        _context = _sp.GetRequiredService<EduZasDotnetContext>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagExistsAndIsTheOnlyOne_ShouldDeleteTagAndContactTag()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, email: "owner@test.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@test.com");
        var tag = await SeedTag("test-tag");
        await SeedContact(owner.Id, contactUser.Id);
        await SeedContactTag(owner.Id, contactUser.Id, tag.Id);

        var dto = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser.Id,
            TagId = tag.Id,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(owner) }
        );

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(0, await _context.ContactTags.CountAsync());
        Assert.Equal(0, await _context.Tags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagExistsAndOtherContactsUseIt_ShouldDeleteOnlyContactTag()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, email: "owner@test.com");
        var contactUser1 = await SeedUser(UserType.PROFESSOR, email: "contact1@test.com");
        var contactUser2 = await SeedUser(UserType.PROFESSOR, email: "contact2@test.com");
        var tag = await SeedTag("test-tag");

        await SeedContact(owner.Id, contactUser1.Id);
        await SeedContact(owner.Id, contactUser2.Id);
        await SeedContactTag(owner.Id, contactUser1.Id, tag.Id);
        await SeedContactTag(owner.Id, contactUser2.Id, tag.Id);

        var dto = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser1.Id,
            TagId = tag.Id,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(owner) }
        );

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(1, await _context.ContactTags.CountAsync());
        Assert.Equal(1, await _context.Tags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, email: "owner@test.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@test.com");
        await SeedContact(owner.Id, contactUser.Id);

        var dto = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser.Id,
            TagId = 999,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(owner) }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsUnauthorized_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, email: "owner@test.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@test.com");
        var unauthorized = await SeedUser(UserType.PROFESSOR, email: "unauthorized@test.com");
        var tag = await SeedTag("test-tag");
        
        await SeedContact(owner.Id, contactUser.Id);
        await SeedContactTag(owner.Id, contactUser.Id, tag.Id);

        var dto = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser.Id,
            TagId = tag.Id,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(unauthorized) }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutorIsAdmin_ShouldDeleteTag()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, email: "owner@test.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@test.com");
        var admin = await SeedUser(UserType.ADMIN, email: "admin@test.com");
        var tag = await SeedTag("test-tag");
        await SeedContact(owner.Id, contactUser.Id);
        await SeedContactTag(owner.Id, contactUser.Id, tag.Id);

        var dto = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser.Id,
            TagId = tag.Id,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(0, await _context.ContactTags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutorIsStudent_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, email: "owner@test.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@test.com");
        var student = await SeedUser(UserType.STUDENT, email: "student@test.com");
        var tag = await SeedTag("test-tag");
        await SeedContact(owner.Id, contactUser.Id);
        await SeedContactTag(owner.Id, contactUser.Id, tag.Id);

        var dto = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser.Id,
            TagId = tag.Id,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(student) }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }
}

