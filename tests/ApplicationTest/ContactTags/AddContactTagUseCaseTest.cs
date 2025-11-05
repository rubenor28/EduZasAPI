using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.UseCases.ContactTags;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Contacts;
using EntityFramework.Application.DAOs.ContactTags;
using EntityFramework.Application.DAOs.Tags;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Tags;

public class AddTagToContactUseCaseTest : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly EduZasDotnetContext _ctx;
    private readonly AddContactTagUseCase _useCase;
    private readonly ContactEFCreator _contactCreator;
    private readonly UserEFMapper _userMapper;

    public AddTagToContactUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var userMapper = new UserEFMapper();
        var userReader = new UserEFReader(_ctx, userMapper);
        _userMapper = userMapper;

        var contactMapper = new ContactEFMapper();
        _contactCreator = new ContactEFCreator(_ctx, contactMapper, contactMapper);
        var contactReader = new ContactEFReader(_ctx, contactMapper);

        var tagMapper = new TagEFMapper();
        var tagReader = new TagEFReader(_ctx, tagMapper);
        var tagCreator = new TagEFCreator(_ctx, tagMapper, tagMapper);

        var contactTagMapper = new ContactTagEFMapper();
        var contactTagCreator = new ContactTagEFCreator(_ctx, contactTagMapper, contactTagMapper);
        var contactTagReader = new ContactTagEFReader(_ctx, contactTagMapper);

        _useCase = new AddContactTagUseCase(
            contactTagCreator,
            tagReader,
            tagCreator,
            userReader,
            contactReader,
            contactTagReader
        );
    }

    private async Task<UserDomain> CreateUser(string email, UserType role = UserType.PROFESSOR)
    {
        var user = new User
        {
            FirstName = "Test",
            FatherLastname = "User",
            Email = email,
            Password = "Password123!",
            Role = (uint)role,
        };

        await _ctx.Users.AddAsync(user);
        await _ctx.SaveChangesAsync();

        return _userMapper.Map(user);
    }

    private async Task<ContactDomain> CreateContact(ulong ownerId, ulong userId)
    {
        var contact = new NewContactDTO
        {
            Alias = "Test Contact",
            AgendaOwnerId = ownerId,
            UserId = userId, // Corrected property
            Executor = new Executor { Id = ownerId, Role = UserType.ADMIN },
        };
        return await _contactCreator.AddAsync(contact);
    }

    [Fact]
    public async Task ExecuteAsync_WhenContactExistsAndUserIsAuthorized_ShouldAddTag()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        await CreateContact(owner.Id, contactUser.Id);
        await _ctx.SaveChangesAsync();

        var dto = new ContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser.Id,
                Tag = "new-tag",
            },
            Executor = new() { Id = owner.Id, Role = owner.Role },
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        Assert.True(result.IsOk);
        var tagInDb = await _ctx.ContactTags.FirstOrDefaultAsync();
        Assert.NotNull(tagInDb);
        Assert.Equal("new-tag", tagInDb.TagText);
        Assert.Equal(owner.Id, tagInDb.AgendaOwnerId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenContactDoesNotExist_ShouldReturnInputError()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var dto = new ContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = 99,
                Tag = "new-tag",
            },
            Executor = new() { Id = owner.Id, Role = owner.Role },
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        Assert.True(result.IsErr);
        var error = result.UnwrapErr();
        Assert.IsType<InputError>(error);
        Assert.Contains(((InputError)error).Errors, e => e.Field == "userId");
    }

    [Fact]
    public async Task ExecuteAsync_WhenOwnerDoesNotExist_ShouldReturnInputError()
    {
        // Arrange
        var contact = await CreateUser("contact@test.com");
        var dto = new ContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = 99,
                UserId = contact.Id,
                Tag = "new-tag",
            },
            Executor = new() { Id = contact.Id, Role = contact.Role },
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

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
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var unauthorized = await CreateUser("unauthorized@test.com");
        await CreateContact(owner.Id, contactUser.Id);

        var dto = new ContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser.Id,
                Tag = "new-tag",
            },
            Executor = new() { Id = unauthorized.Id, Role = unauthorized.Role },
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutorIsStudent_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var student = await CreateUser("student@test.com", UserType.STUDENT);
        await CreateContact(owner.Id, contactUser.Id);

        var dto = new ContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser.Id,
                Tag = "new-tag",
            },
            Executor = new() { Id = student.Id, Role = student.Role },
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutorIsAdmin_ShouldSucceed()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var admin = await CreateUser("admin@test.com", UserType.ADMIN);
        await CreateContact(owner.Id, contactUser.Id);

        var dto = new ContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser.Id,
                Tag = "new-tag",
            },
            Executor = new() { Id = admin.Id, Role = admin.Role },
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(1, await _ctx.ContactTags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagIsAlreadyOnContact_ShouldThrowException()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        await CreateContact(owner.Id, contactUser.Id);

        var dto = new ContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser.Id,
                Tag = "new-tag",
            },
            Executor = new() { Id = owner.Id, Role = owner.Role },
        };

        await _useCase.ExecuteAsync(dto); // First time is OK
        var result = await _useCase.ExecuteAsync(dto); // Second time should fail
        Assert.True(result.IsErr);
        Assert.IsType<AlreadyExistsError>(result.UnwrapErr());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
