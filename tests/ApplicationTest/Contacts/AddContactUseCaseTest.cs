using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.Users;
using Application.UseCases.Contacts;
using Application.UseCases.ContactTags;
using Application.UseCases.Tags;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Contacts;
using EntityFramework.Application.DAOs.ContactTag;
using EntityFramework.Application.DAOs.Tags;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Contacts;

public class AddContactUseCaseTest : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly EduZasDotnetContext _ctx;
    private readonly AddContactUseCase _useCase;
    private readonly UserEFMapper _userMapper = new();
    private readonly UserEFCreator _userCreator;

    public AddContactUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var contactMapper = new ContactEFMapper();
        var contactCreator = new ContactEFCreator(_ctx, contactMapper, contactMapper);
        var userReader = new UserEFReader(_ctx, _userMapper);
        var contactQuerier = new ContactEFQuerier(_ctx, contactMapper, 10);
        var tagMapper = new TagEFMapper();
        var tagQuerier = new TagEFQuerier(_ctx, tagMapper, 10);
        var tagCreator = new TagEFCreator(_ctx, tagMapper, tagMapper);
        var addTagUseCase = new AddTagUseCase(tagCreator, tagQuerier);
        var contactTagMapper = new ContactTagEFMapper();
        var contactTagCreator = new ContactTagEFCreator(_ctx, contactTagMapper, contactTagMapper);
        var contactReaderForTag = new ContactEFReader(_ctx, contactMapper);
        var tagReaderForTag = new TagEFReader(_ctx, tagMapper);
        var contactTagReader = new ContactTagEFReader(_ctx, contactTagMapper);
        var addContactTagUseCase = new AddContactTagUseCase(contactTagCreator, contactReaderForTag, tagReaderForTag, contactTagReader);

        _userCreator = new UserEFCreator(_ctx, _userMapper, _userMapper);

        _useCase = new AddContactUseCase(
            contactCreator,
            userReader,
            contactQuerier,
            tagQuerier,
            addContactTagUseCase,
            addTagUseCase
        );
    }

    private async Task<UserDomain> CreateUser(string email)
    {
        var user = new NewUserDTO
        {
            FirstName = "Test",
            FatherLastName = "User",
            Email = email,
            Password = "Password123!",
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
            ContactId = contactUser.Id,
            Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            ContactTags = [],
        };

        var result = await _useCase.ExecuteAsync(newContact);

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
            ContactId = contactUser.Id,
            Executor = new Executor { Id = 999, Role = UserType.ADMIN },
            ContactTags = [],
        };

        var result = await _useCase.ExecuteAsync(newContact);

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
            ContactId = 999, // Non-existent user
            Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            ContactTags = [],
        };

        var result = await _useCase.ExecuteAsync(newContact);

        Assert.True(result.IsErr);
        var error = result.UnwrapErr();
        Assert.Equal(typeof(InputError), error.GetType());
        Assert.Contains(((InputError)error).Errors, e => e.Field == "contactId");
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateContact_ReturnsError()
    {
        var agendaOwner = await CreateUser("owner@example.com");
        var contactUser = await CreateUser("contact@example.com");

        var firstContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            ContactId = contactUser.Id,
            Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            ContactTags = [],
        };
        await _useCase.ExecuteAsync(firstContact);

        var secondContact = new NewContactDTO
        {
            Alias = "Test",
            Notes = "Test note",
            AgendaOwnerId = agendaOwner.Id,
            ContactId = contactUser.Id,
            Executor = new Executor { Id = agendaOwner.Id, Role = UserType.ADMIN },
            ContactTags = [],
        };

        var result = await _useCase.ExecuteAsync(secondContact);

        Assert.True(result.IsErr);
        Assert.Equal(typeof(AlreadyExistsError), result.UnwrapErr().GetType());
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
            ContactId = contactUser.Id,
            Executor = new Executor { Id = unauthorizedUser.Id, Role = UserType.PROFESSOR },
            ContactTags = [],
        };

        var result = await _useCase.ExecuteAsync(newContact);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
