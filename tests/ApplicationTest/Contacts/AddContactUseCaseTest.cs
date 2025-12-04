using Application.DTOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.Users;
using Application.UseCases.Contacts;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Contacts;
using EntityFramework.Application.DAOs.ContactTags;
using EntityFramework.Application.DAOs.Tags;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Contacts;
using EntityFramework.InterfaceAdapters.Mappers.ContactTags;
using EntityFramework.InterfaceAdapters.Mappers.Tags;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Contacts;

public class AddContactUseCaseTest : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly EduZasDotnetContext _ctx;
    private readonly AddContactUseCase _useCase;
    private readonly UserMapper _userMapper = new();
    private readonly UserEFCreator _userCreator;

    public AddContactUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var contactMapper = new ContactMapper();
        var contactReader = new ContactEFReader(_ctx, contactMapper);

        var contactCreator = new ContactEFCreator(_ctx, contactMapper, new NewContactEFMapper());
        var userReader = new UserEFReader(_ctx, _userMapper);
        var tagMapper = new TagMapper();
        var tagReader = new TagEFReader(_ctx, tagMapper);
        var tagCreator = new TagEFCreator(_ctx, tagMapper, new NewTagEFMapper());
        var contactTagMapper = new ContactTagMapper();
        var contactTagCreator = new ContactTagEFCreator(
            _ctx,
            contactTagMapper,
            new NewContactTagEFMapper()
        );

        var newUsrMapper = new NewUserEFMapper(new UserTypeUintMapper());
        _userCreator = new UserEFCreator(_ctx, _userMapper, newUsrMapper);

        _useCase = new AddContactUseCase(
            contactCreator,
            contactReader,
            userReader,
            tagReader,
            tagCreator,
            contactTagCreator
        );
    }

    private async Task<UserDomain> CreateUser(string email, UserType role = UserType.STUDENT)
    {
        var user = new NewUserDTO
        {
            FirstName = "Test",
            FatherLastname = "User",
            Email = email,
            Password = "Password123!",
            Role = role,
        };
        return await _userCreator.AddAsync(user);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ReturnsOk()
    {
        var agendaOwner = await CreateUser("owner@example.com");
        var contactUser = await CreateUser("contact@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = [],
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentAgendaOwner_ReturnsError()
    {
        var contactUser = await CreateUser("contact@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = 999,
            UserId = contactUser.Id,
            Tags = [],
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = 999, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsErr);
        var error = result.UnwrapErr();
        Assert.Equal(typeof(InputError), error.GetType());
        Assert.Contains(((InputError)error).Errors, e => e.Field == "agendaOwnerId");
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentContactUser_ReturnsError()
    {
        var agendaOwner = await CreateUser("owner@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = 999, // Non-existent user
            Tags = [],
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsErr);
        var error = result.UnwrapErr();
        Assert.Equal(typeof(InputError), error.GetType());
        Assert.Contains(((InputError)error).Errors, e => e.Field == "userId");
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateContact_ReturnsError()
    {
        var agendaOwner = await CreateUser("owner@example.com");
        var contactUser = await CreateUser("contact@example.com");
        _ctx.ChangeTracker.Clear();

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
        var result = await _useCase.ExecuteAsync(execution);

        Assert.True(result.IsErr);
        Assert.IsType<AlreadyExistsError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_UnauthorizedExecutor_ReturnsError()
    {
        var agendaOwner = await CreateUser("owner@example.com");
        var contactUser = await CreateUser("contact@example.com");
        var unauthorizedUser = await CreateUser("unauthorized@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = [],
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = unauthorizedUser.Id, Role = UserType.PROFESSOR },
            }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AuthorizedNonAdminExecutor_ReturnsOk()
    {
        var agendaOwner = await CreateUser("owner@example.com");
        var contactUser = await CreateUser("contact@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = [],
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.PROFESSOR }, // Authorized non-admin
            }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithNewTags_CreatesTagsAndAssociations()
    {
        var agendaOwner = await CreateUser("owner@example.com");
        var contactUser = await CreateUser("contact@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = ["tag1", "tag2"],
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsOk);
        Assert.Equal(2, await _ctx.Tags.CountAsync());
        Assert.Equal(2, await _ctx.ContactTags.CountAsync());
        Assert.True(await _ctx.Tags.AnyAsync(t => t.Text == "tag1"));
        Assert.True(await _ctx.Tags.AnyAsync(t => t.Text == "tag2"));
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingTags_CreatesOnlyAssociations()
    {
        var agendaOwner = await CreateUser("owner@example.com");
        var contactUser = await CreateUser("contact@example.com");

        // Pre-existing tag
        _ctx.Tags.Add(new Tag { Text = "existing-tag", CreatedAt = DateTime.UtcNow });
        await _ctx.SaveChangesAsync();

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = ["existing-tag"],
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsOk);
        Assert.Equal(1, await _ctx.Tags.CountAsync()); // No new tag created
        Assert.Equal(1, await _ctx.ContactTags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WithMixedTags_CreatesAndAssociatesCorrectly()
    {
        var agendaOwner = await CreateUser("owner@example.com");
        var contactUser = await CreateUser("contact@example.com");

        // Pre-existing tag
        _ctx.Tags.Add(new Tag { Text = "existing-tag", CreatedAt = DateTime.UtcNow });
        await _ctx.SaveChangesAsync();

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
            Tags = ["existing-tag", "new-tag"],
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsOk);
        Assert.Equal(2, await _ctx.Tags.CountAsync()); // One new tag created
        Assert.Equal(2, await _ctx.ContactTags.CountAsync());
        Assert.True(await _ctx.ContactTags.AnyAsync(ct => ct.TagText == "existing-tag"));
        Assert.True(await _ctx.ContactTags.AnyAsync(ct => ct.TagText == "new-tag"));
    }

    [Fact]
    public async Task ExecuteAsync_WithOptionalNoneTags_ReturnsOk()
    {
        var agendaOwner = await CreateUser("owner@example.com");
        var contactUser = await CreateUser("contact@example.com");

        var newContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            UserId = contactUser.Id,
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newContact,
                Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsOk);
        Assert.Equal(0, await _ctx.Tags.CountAsync());
        Assert.Equal(0, await _ctx.ContactTags.CountAsync());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
