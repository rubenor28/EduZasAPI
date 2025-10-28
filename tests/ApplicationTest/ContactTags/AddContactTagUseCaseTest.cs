using Application.DTOs.Common;
using Application.DTOs.ContactTag;
using Application.UseCases.ContactTags;
using Domain.Enums;
using EntityFramework.Application.DAOs.Contacts;
using EntityFramework.Application.DAOs.ContactTag;
using EntityFramework.Application.DAOs.Tags;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tag = EntityFramework.Application.DTOs.Tag;
using User = EntityFramework.Application.DTOs.User;

namespace ApplicationTest.ContactTags;

public class AddContactTagUseCaseTest : IDisposable
{
    private readonly AddContactClassUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public AddContactTagUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var contactMapper = new ContactEFMapper();
        var tagMapper = new TagEFMapper();
        var contactTagMapper = new ContactTagEFMapper();

        var contactReader = new ContactEFReader(_ctx, contactMapper);
        var tagReader = new TagEFReader(_ctx, tagMapper);
        var contactTagReader = new ContactTagEFReader(_ctx, contactTagMapper);
        var contactTagCreator = new ContactTagEFCreator(_ctx, contactTagMapper, contactTagMapper);

        _useCase = new AddContactClassUseCase(
            contactTagCreator,
            contactReader,
            tagReader,
            contactTagReader,
            null // No validator
        );
    }

    private async Task<(User owner, AgendaContact contact, Tag tag)> SeedData()
    {
        var owner = new User
        {
            UserId = 1,
            Email = "owner@test.com",
            FirstName = "owner",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)UserType.PROFESSOR
        };
        var otherUser = new User
        {
            UserId = 2,
            Email = "contact@test.com",
            FirstName = "contact",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)UserType.PROFESSOR
        };
        _ctx.Users.AddRange(owner, otherUser);

        var contact = new AgendaContact
        {
            AgendaContactId = 10,
            AgendaOwnerId = owner.UserId,
            ContactId = otherUser.UserId,
            Alias = "My Contact"
        };
        _ctx.AgendaContacts.Add(contact);

        var tag = new Tag { TagId = 20, Text = "My Tag" };
        _ctx.Tags.Add(tag);

        await _ctx.SaveChangesAsync();
        return (owner, contact, tag);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndOwnerExecutor_ReturnsOk()
    {
        var (owner, contact, tag) = await SeedData();

        var dto = new NewContactTagDTO
        {
            AgendaContactId = contact.AgendaContactId,
            TagId = tag.TagId,
            Executor = new Executor { Id = owner.UserId, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
        var relation = await _ctx.TagsPerUsers.FindAsync(tag.TagId, contact.AgendaContactId);
        Assert.NotNull(relation);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonOwnerExecutor_ReturnsUnauthorizedError()
    {
        var (_, contact, tag) = await SeedData();
        var nonOwner = new User { UserId = 99, Role = (uint)UserType.PROFESSOR, Email = "non@owner.com", FirstName = "test", FatherLastname = "test", Password = "test" };
        _ctx.Users.Add(nonOwner);
        await _ctx.SaveChangesAsync();

        var dto = new NewContactTagDTO
        {
            AgendaContactId = contact.AgendaContactId,
            TagId = tag.TagId,
            Executor = new Executor { Id = nonOwner.UserId, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentContact_ReturnsInputError()
    {
        var (owner, _, tag) = await SeedData();
        var dto = new NewContactTagDTO
        {
            AgendaContactId = 999,
            TagId = tag.TagId,
            Executor = new Executor { Id = owner.UserId, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        var err = Assert.IsType<InputError>(result.UnwrapErr());
        Assert.Contains(err.Errors, e => e.Field == "agendaContactId");
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentTag_ReturnsInputError()
    {
        var (owner, contact, _) = await SeedData();
        var dto = new NewContactTagDTO
        {
            AgendaContactId = contact.AgendaContactId,
            TagId = 999,
            Executor = new Executor { Id = owner.UserId, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        var err = Assert.IsType<InputError>(result.UnwrapErr());
        Assert.Contains(err.Errors, e => e.Field == "tagId");
    }

    [Fact]
    public async Task ExecuteAsync_WhenRelationAlreadyExists_ReturnsAlreadyExistsError()
    {
        var (owner, contact, tag) = await SeedData();
        var dto = new NewContactTagDTO
        {
            AgendaContactId = contact.AgendaContactId,
            TagId = tag.TagId,
            Executor = new Executor { Id = owner.UserId, Role = UserType.PROFESSOR }
        };
        
        var firstResult = await _useCase.ExecuteAsync(dto);
        Assert.True(firstResult.IsOk);

        var secondResult = await _useCase.ExecuteAsync(dto);

        Assert.True(secondResult.IsErr);
        Assert.IsType<AlreadyExistsError>(secondResult.UnwrapErr());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
