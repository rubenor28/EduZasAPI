using Application.DTOs.Common;
using Application.DTOs.ContactTags;
using Application.UseCases.ContactTags;
using Domain.Enums;
using EntityFramework.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Tests.Application.UseCases.ContactTags;

public class AddContactTagUseCaseTest : BaseTest
{
    private readonly AddContactTagUseCase _useCase;
    private readonly EduZasDotnetContext _context;

    public AddContactTagUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<AddContactTagUseCase>();
        _context = _sp.GetRequiredService<EduZasDotnetContext>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagDoesNotExist_ShouldCreateTagAndAddAssociation()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, "owner@test.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, "contact@test.com");
        await SeedContact(owner.Id, contactUser.Id);

        var dto = new NewContactTagRequestDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser.Id,
            TagText = "new-tag",
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = dto,
                Executor = new() { Id = owner.Id, Role = owner.Role },
            }
        );

        // Assert
        Assert.True(result.IsOk);
        var createdTag = await _context.Tags.FirstOrDefaultAsync(t => t.Text == "new-tag".ToUpperInvariant());
        Assert.NotNull(createdTag);
        var association = await _context.ContactTags.FirstOrDefaultAsync();
        Assert.NotNull(association);
        Assert.Equal(createdTag.TagId, association.TagId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenContactDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, "owner@test.com");
        var contact = await SeedUser(UserType.PROFESSOR, "contact@test.com");
        var dto = new NewContactTagRequestDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contact.Id,
            TagText = "new-tag"
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = dto,
                Executor = new() { Id = owner.Id, Role = owner.Role },
            }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenOwnerDoesNotExist_ShouldReturnInputError()
    {
        // Arrange
        var contact = await SeedUser(UserType.PROFESSOR, "contact@test.com");
        var dto = new NewContactTagRequestDTO
        {
            AgendaOwnerId = 99,
            UserId = contact.Id,
            TagText = "new-tag",
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = dto,
                Executor = new() { Id = contact.Id, Role = contact.Role },
            }
        );

        // Assert
        Assert.True(result.IsErr);
        var error = result.UnwrapErr();
        Assert.IsType<InputError>(error);
        Assert.Contains(((InputError)error).Errors, e => e.Field == "agendaOwnerId");
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutorIsUnauthorized_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, "owner@test.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, "contact@test.com");
        var unauthorized = await SeedUser(UserType.PROFESSOR, "unauthorized@test.com");
        await SeedContact(owner.Id, contactUser.Id);

        var dto = new NewContactTagRequestDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser.Id,
            TagText = "new-tag",
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = dto,
                Executor = new() { Id = unauthorized.Id, Role = unauthorized.Role },
            }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutorIsStudent_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, "owner@test.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, "contact@test.com");
        var student = await SeedUser(UserType.STUDENT, "student@test.com");
        await SeedContact(owner.Id, contactUser.Id);

        var dto = new NewContactTagRequestDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser.Id,
            TagText = "new-tag",
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = dto,
                Executor = new() { Id = student.Id, Role = student.Role },
            }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutorIsAdmin_ShouldSucceed()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, "owner@test.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, "contact@test.com");
        var admin = await SeedUser(UserType.ADMIN, "admin@test.com");
        await SeedContact(owner.Id, contactUser.Id);

        var dto = new NewContactTagRequestDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser.Id,
            TagText = "new-tag",
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = dto,
                Executor = new() { Id = admin.Id, Role = admin.Role },
            }
        );

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(1, await _context.ContactTags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagIsAlreadyOnContact_ShouldReturnConflictError()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR, "owner@test.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, "contact@test.com");
        await SeedContact(owner.Id, contactUser.Id);

        var dto = new NewContactTagRequestDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contactUser.Id,
            TagText = "new-tag",
        };

        await _useCase.ExecuteAsync(
            new()
            {
                Data = dto,
                Executor = new() { Id = owner.Id, Role = owner.Role },
            }
        ); 
        
        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = dto,
                Executor = new() { Id = owner.Id, Role = owner.Role },
            }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<Conflict>(result.UnwrapErr());
    }
}

