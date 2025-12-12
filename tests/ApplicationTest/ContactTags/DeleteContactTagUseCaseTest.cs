using Application.DTOs.Common;
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

    private readonly UserMapper _userMapper = new();
    private readonly ContactMapper _contactMapper = new();
    private readonly TagMapper _tagMapper = new();

    public DeleteContactTagUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var contactProjector = new ContactProjector();
        var contactQuerier = new ContactEFQuerier(_ctx, contactProjector, 10);

        var tagDeleter = new TagEFDeleter(_ctx, _tagMapper);
        var tagReader = new TagEFReader(_ctx, _tagMapper);

        var contactTagMapper = new ContactTagMapper();
        var contactTagDeleter = new ContactTagEFDeleter(_ctx, contactTagMapper);
        var contactTagReader = new ContactTagEFReader(_ctx, contactTagMapper);

        _useCase = new DeleteContactTagUseCase(
            contactTagDeleter,
            contactTagReader,
            contactQuerier,
            tagDeleter,
            tagReader
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

    private async Task<TagDomain> CreateTag(string text)
    {
        var tag = new Tag { Text = text };
        _ctx.Tags.Add(tag);
        await _ctx.SaveChangesAsync();

        return _tagMapper.Map(tag);
    }

    private async Task CreateContactTag(ulong ownerId, ulong userId, ulong tagId)
    {
        var contactTag = new ContactTag
        {
            TagId = tagId,
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
        var tag = await CreateTag("test-tag");
        await CreateContact(owner.Id, contactUser.Id);
        await CreateContactTag(owner.Id, contactUser.Id, tag.Id);

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
        var tag = await CreateTag("test-tag");

        await CreateContact(owner.Id, contactUser1.Id);
        await CreateContact(owner.Id, contactUser2.Id);
        await CreateContactTag(owner.Id, contactUser1.Id, tag.Id);
        await CreateContactTag(owner.Id, contactUser2.Id, tag.Id);

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
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var unauthorized = await CreateUser("unauthorized@test.com");
        var tag = await CreateTag("test-tag");
        
        await CreateContact(owner.Id, contactUser.Id);
        await CreateContactTag(owner.Id, contactUser.Id, tag.Id);

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
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var admin = await CreateUser("admin@test.com", UserType.ADMIN);
        var tag = await CreateTag("test-tag");
        await CreateContact(owner.Id, contactUser.Id);
        await CreateContactTag(owner.Id, contactUser.Id, tag.Id);

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
        Assert.Equal(0, await _ctx.ContactTags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutorIsStudent_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var student = await CreateUser("student@test.com", UserType.STUDENT);
        var tag = await CreateTag("test-tag");
        await CreateContact(owner.Id, contactUser.Id);
        await CreateContactTag(owner.Id, contactUser.Id, tag.Id);

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

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
