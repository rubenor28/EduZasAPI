using Domain.ValueObjects;
using Application.DTOs.Contacts;
using Application.UseCases.Contacts;
using Domain.Enums;
using EntityFramework.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.UseCases.Contacts;

public class AddContactUseCaseTest : BaseTest
{
    private readonly AddContactUseCase _useCase;
    private readonly EduZasDotnetContext _context;

    public AddContactUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<AddContactUseCase>();
        _context = _sp.GetRequiredService<EduZasDotnetContext>();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ReturnsOk()
    {
        // Arrange
        var agendaOwner = await SeedUser(UserType.PROFESSOR, email: "owner@example.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = [],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        // Assert
        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithStudentContact_ReturnsInputError()
    {
        // Arrange
        var agendaOwner = await SeedUser(UserType.PROFESSOR, email: "owner@example.com");
        var contactUser = await SeedUser(UserType.STUDENT, email: "student@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = [],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<InputError>(result.UnwrapErr());
        var err = (InputError)result.UnwrapErr();
        Assert.Contains("userId", err.Errors.Select(e => e.Field));
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentAgendaOwner_ReturnsError()
    {
        // Arrange
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = 999,
            UserId = contactUser.Id,
            Tags = [],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = 999, Role = UserType.ADMIN },
            }
        );

        // Assert
        Assert.True(result.IsErr);
        var err = result.UnwrapErr();
        Assert.IsType<InputError>(err);
        Assert.Contains(((InputError)err).Errors, e => e.Field == "agendaOwnerId");
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentContactUser_ReturnsError()
    {
        // Arrange
        var agendaOwner = await SeedUser(UserType.PROFESSOR, email: "owner@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = 999, // Non-existent user
            Tags = [],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        // Assert
        Assert.True(result.IsErr);
        var error = result.UnwrapErr();
        Assert.Equal(typeof(InputError), error.GetType());
        Assert.Contains(((InputError)error).Errors, e => e.Field == "userId");
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateContact_ReturnsError()
    {
        // Arrange
        var agendaOwner = await SeedUser(UserType.PROFESSOR, email: "owner@example.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@example.com");

        var contact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = [],
        };

        var execution = new UserActionDTO<NewContactDTO>
        {
            Data = contact,
            Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
        };

        await _useCase.ExecuteAsync(execution);
        
        // Act
        var result = await _useCase.ExecuteAsync(execution);

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<Conflict>(result.UnwrapErr());
        var err = (Conflict)result.UnwrapErr();
        Assert.Equal("El recurso ya existe", err.Message);
    }

    [Fact]
    public async Task ExecuteAsync_UnauthorizedExecutor_ReturnsError()
    {
        // Arrange
        var agendaOwner = await SeedUser(UserType.PROFESSOR, email: "owner@example.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@example.com");
        var unauthorizedUser = await SeedUser(UserType.PROFESSOR, email: "unauthorized@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = [],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = unauthorizedUser.Id, Role = UserType.PROFESSOR },
            }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AuthorizedNonAdminExecutor_ReturnsOk()
    {
        // Arrange
        var agendaOwner = await SeedUser(UserType.PROFESSOR, email: "owner@example.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = [],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.PROFESSOR }, // Authorized non-admin
            }
        );

        // Assert
        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithNewTags_CreatesTagsAndAssociations()
    {
        // Arrange
        var agendaOwner = await SeedUser(UserType.PROFESSOR, email: "owner@example.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = ["tag1", "tag2"],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(2, await _context.Tags.CountAsync());
        Assert.Equal(2, await _context.ContactTags.CountAsync());
        Assert.True(await _context.Tags.AnyAsync(t => t.Text == "TAG1"));
        Assert.True(await _context.Tags.AnyAsync(t => t.Text == "TAG2"));
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingTags_CreatesOnlyAssociations()
    {
        // Arrange
        var agendaOwner = await SeedUser(UserType.PROFESSOR, email: "owner@example.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@example.com");
        await SeedTag("EXISTING-TAG");
        
        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = ["existing-tag"],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(1, await _context.Tags.CountAsync());
        Assert.Equal(1, await _context.ContactTags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WithMixedTags_CreatesAndAssociatesCorrectly()
    {
        // Arrange
        var agendaOwner = await SeedUser(UserType.PROFESSOR, email: "owner@example.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@example.com");
        await SeedTag("EXISTING-TAG");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = ["existing-tag", "new-tag"],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );
        
        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(2, await _context.Tags.CountAsync());
        Assert.Equal(2, await _context.ContactTags.CountAsync());
        Assert.True(await _context.ContactTags.AnyAsync(ct => ct.Tag.Text == "EXISTING-TAG"));
        Assert.True(await _context.ContactTags.AnyAsync(ct => ct.Tag.Text == "NEW-TAG"));
    }

    [Fact]
    public async Task ExecuteAsync_WithOptionalNoneTags_ReturnsOk()
    {
        // Arrange
        var agendaOwner = await SeedUser(UserType.PROFESSOR, email: "owner@example.com");
        var contactUser = await SeedUser(UserType.PROFESSOR, email: "contact@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(0, await _context.Tags.CountAsync());
        Assert.Equal(0, await _context.ContactTags.CountAsync());
    }
}

