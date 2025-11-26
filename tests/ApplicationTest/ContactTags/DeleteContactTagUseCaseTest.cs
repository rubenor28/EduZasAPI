using Application.DTOs.Common;
using Application.DTOs.ContactTags;
using Application.UseCases.ContactTags;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Contacts;
using EntityFramework.Application.DAOs.ContactTags;
using EntityFramework.Application.DAOs.Tags;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Contacts;
using EntityFramework.InterfaceAdapters.Mappers.ContactTags;
using EntityFramework.InterfaceAdapters.Mappers.Tags;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ContactTags;

public class DeleteContactTagUseCaseTest : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly EduZasDotnetContext _ctx;
    private readonly DeleteContactTagUseCase _useCase;

    private readonly UserProjector _userMapper = new();
    private readonly ContactProjector _contactMapper = new();

    public DeleteContactTagUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var contactQuerier = new ContactEFQuerier(_ctx, _contactMapper, 10);

        var tagMapper = new TagProjector();
        var tagDeleter = new TagEFDeleter(_ctx, tagMapper);

        var contactTagMapper = new ContactTagProjector();
        var contactTagDeleter = new ContactTagEFDeleter(_ctx, contactTagMapper);
        var contactTagReader = new ContactTagEFReader(_ctx, contactTagMapper);

        _useCase = new DeleteContactTagUseCase(
            contactTagDeleter,
            contactTagReader,
            contactQuerier,
            tagDeleter
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
        var contact = new AgendaContact
        {
            Alias = "Test Contact",
            AgendaOwnerId = ownerId,
            UserId = userId,
        };

        _ctx.AgendaContacts.Add(contact);
        await _ctx.SaveChangesAsync();

        return _contactMapper.Map(contact);
    }

    private async Task CreateContactTag(ulong ownerId, ulong userId, string tag)
    {
        if (!await _ctx.Tags.AnyAsync(t => t.Text == tag))
            _ctx.Tags.Add(new Tag { Text = tag });

        var contactTag = new ContactTag
        {
            TagText = tag,
            AgendaOwnerId = ownerId,
            UserId = userId,
        };

        _ctx.ContactTags.Add(contactTag);

        await _ctx.SaveChangesAsync();
    }

    private static Executor AsExecutor(UserDomain u) => new() { Id = u.Id, Role = u.Role };

    [Fact]
    public async Task ExecuteAsync_WhenTagExistsAndIsTheOnlyOne_ShouldDeleteTagAndContactTag()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        await CreateContact(owner.Id, contactUser.Id);
        const string tag = "test-tag";
        await CreateContactTag(owner.Id, contactUser.Id, tag);

        var dto = new DeleteContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser.Id,
                Tag = tag,
            },
            Executor = AsExecutor(owner),
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(0, await _ctx.ContactTags.CountAsync());
        Assert.Equal(0, await _ctx.Tags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagExistsAndOtherContactsUseIt_ShouldDeleteOnlyContactTag()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser1 = await CreateUser("contact1@test.com");
        var contactUser2 = await CreateUser("contact2@test.com");
        await CreateContact(owner.Id, contactUser1.Id);
        await CreateContact(owner.Id, contactUser2.Id);
        const string tag = "test-tag";
        await CreateContactTag(owner.Id, contactUser1.Id, tag);
        await CreateContactTag(owner.Id, contactUser2.Id, tag);

        var dto = new DeleteContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser1.Id,
                Tag = tag,
            },
            Executor = AsExecutor(owner),
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(1, await _ctx.ContactTags.CountAsync());
        Assert.Equal(1, await _ctx.Tags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        await CreateContact(owner.Id, contactUser.Id);

        var dto = new DeleteContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser.Id,
                Tag = "non-existent-tag",
            },
            Executor = AsExecutor(owner),
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsUnauthorized_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var unauthorized = await CreateUser("unauthorized@test.com");
        await CreateContact(owner.Id, contactUser.Id);
        const string tag = "test-tag";
        await CreateContactTag(owner.Id, contactUser.Id, tag);

        var dto = new DeleteContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser.Id,
                Tag = tag,
            },
            Executor = AsExecutor(unauthorized),
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutorIsAdmin_ShouldDeleteTag()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var admin = await CreateUser("admin@test.com", UserType.ADMIN);
        await CreateContact(owner.Id, contactUser.Id);
        const string tag = "test-tag";
        await CreateContactTag(owner.Id, contactUser.Id, tag);

        var dto = new DeleteContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser.Id,
                Tag = tag,
            },
            Executor = AsExecutor(admin),
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        Assert.True(result.IsOk);
        Assert.Equal(0, await _ctx.ContactTags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutorIsStudent_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var student = await CreateUser("student@test.com", UserType.STUDENT);
        await CreateContact(owner.Id, contactUser.Id);
        const string tag = "test-tag";
        await CreateContactTag(owner.Id, contactUser.Id, tag);

        var dto = new DeleteContactTagDTO
        {
            Id = new()
            {
                AgendaOwnerId = owner.Id,
                UserId = contactUser.Id,
                Tag = tag,
            },
            Executor = AsExecutor(student),
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
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
