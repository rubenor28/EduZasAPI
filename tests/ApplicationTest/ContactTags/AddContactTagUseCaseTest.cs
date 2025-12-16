using Application.DTOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.DTOs.Tags;
using Application.UseCases.ContactTags;
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
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ContactTags;

public class AddContactTagUseCaseTest : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly EduZasDotnetContext _ctx;
    private readonly AddContactTagUseCase _useCase;
    private readonly ContactEFCreator _contactCreator;

    private readonly UserMapper _userMapper = new();
    private readonly TagMapper _tagMapper = new();

    public AddContactTagUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var userReader = new UserEFReader(_ctx, _userMapper);

        var contactMapper = new ContactMapper();
        _contactCreator = new ContactEFCreator(_ctx, contactMapper, new NewContactEFMapper());
        var contactReader = new ContactEFReader(_ctx, contactMapper);

        var contactTagMapper = new ContactTagMapper();
        var contactTagCreator = new ContactTagEFCreator(
            _ctx,
            contactTagMapper,
            new NewContactTagEFMapper()
        );
        var contactTagReader = new ContactTagEFReader(_ctx, contactTagMapper);
        
        var tagCreator = new TagEFCreator(_ctx, _tagMapper, new NewTagEFMapper());
        var tagProjector = new TagProjector();
        var tagQuerier = new TagEFQuerier(_ctx, tagProjector, 50);

        _useCase = new AddContactTagUseCase(
            contactTagCreator,
            tagQuerier,
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
            UserId = userId,
        };
        return await _contactCreator.AddAsync(contact);
    }

    private async Task<TagDomain> CreateTag(string text)
    {
        var tag = new Tag { Text = text };

        _ctx.Tags.Add(tag);
        await _ctx.SaveChangesAsync();

        return _tagMapper.Map(tag);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagDoesNotExist_ShouldCreateTagAndAddAssociation()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        await CreateContact(owner.Id, contactUser.Id);

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
        var createdTag = await _ctx.Tags.FirstOrDefaultAsync(t => t.Text == "new-tag".ToUpperInvariant());
        Assert.NotNull(createdTag);
        var association = await _ctx.ContactTags.FirstOrDefaultAsync();
        Assert.NotNull(association);
        Assert.Equal(createdTag.TagId, association.TagId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenContactDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contact = await CreateUser("contact@test.com");
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
        var contact = await CreateUser("contact@test.com");
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
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var unauthorized = await CreateUser("unauthorized@test.com");
        await CreateContact(owner.Id, contactUser.Id);

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
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var student = await CreateUser("student@test.com", UserType.STUDENT);
        await CreateContact(owner.Id, contactUser.Id);

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
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        var admin = await CreateUser("admin@test.com", UserType.ADMIN);
        await CreateContact(owner.Id, contactUser.Id);

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
        Assert.Equal(1, await _ctx.ContactTags.CountAsync());
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagIsAlreadyOnContact_ShouldReturnConflictError()
    {
        // Arrange
        var owner = await CreateUser("owner@test.com");
        var contactUser = await CreateUser("contact@test.com");
        await CreateContact(owner.Id, contactUser.Id);

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

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
