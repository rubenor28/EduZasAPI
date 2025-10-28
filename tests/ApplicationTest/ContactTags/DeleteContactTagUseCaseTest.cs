using Application.DTOs.Common;
using Application.DTOs.ContactTags;
using Application.UseCases.ContactTags;
using Domain.Enums;
using EntityFramework.Application.DAOs.Contacts;
using EntityFramework.Application.DAOs.ContactTag;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tag = EntityFramework.Application.DTOs.Tag;
using User = EntityFramework.Application.DTOs.User;

namespace ApplicationTest.ContactTags;

public class DeleteContactTagUseCaseTest : IDisposable
{
    private readonly DeleteContactTagUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public DeleteContactTagUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var contactTagMapper = new ContactTagEFMapper();
        var contactMapper = new ContactEFMapper();

        var deleter = new ContactTagEFDeleter(_ctx, contactTagMapper);
        var reader = new ContactTagEFReader(_ctx, contactTagMapper);
        var contactReader = new ContactEFReader(_ctx, contactMapper);

        _useCase = new DeleteContactTagUseCase(deleter, reader, contactReader, null);
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

        _ctx.TagsPerUsers.Add(new TagsPerUser { AgendaContactId = contact.AgendaContactId, TagId = tag.TagId });

        await _ctx.SaveChangesAsync();
        return (owner, contact, tag);
    }

    [Fact]
    public async Task ExecuteAsync_AsOwner_DeletesSuccessfully()
    {
        var (owner, contact, tag) = await SeedData();
        var dto = new DeleteContactTagDTO
        {
            Id = new() { AgendaContactId = contact.AgendaContactId, TagId = tag.TagId },
            Executor = new() { Id = owner.UserId, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
        var relation = await _ctx.TagsPerUsers.FindAsync(tag.TagId, contact.AgendaContactId);
        Assert.Null(relation);
    }

    [Fact]
    public async Task ExecuteAsync_AsAdmin_DeletesSuccessfully()
    {
        var (_, contact, tag) = await SeedData();
        var admin = new User { UserId = 98, Role = (uint)UserType.ADMIN, Email="admin@test.com", FirstName="admin", FatherLastname="admin", Password="admin" };
        _ctx.Users.Add(admin);
        await _ctx.SaveChangesAsync();

        var dto = new DeleteContactTagDTO
        {
            Id = new() { AgendaContactId = contact.AgendaContactId, TagId = tag.TagId },
            Executor = new() { Id = admin.UserId, Role = UserType.ADMIN }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
        var relation = await _ctx.TagsPerUsers.FindAsync(tag.TagId, contact.AgendaContactId);
        Assert.Null(relation);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentRelation_ReturnsNotFoundError()
    {
        var (owner, _, _) = await SeedData();
        var dto = new DeleteContactTagDTO
        {
            Id = new() { AgendaContactId = 999, TagId = 998 },
            Executor = new() { Id = owner.UserId, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsNonOwner_ReturnsUnauthorizedError()
    {
        var (_, contact, tag) = await SeedData();
        var nonOwner = new User { UserId = 99, Role = (uint)UserType.PROFESSOR, Email = "non@owner.com", FirstName = "test", FatherLastname = "test", Password = "test" };
        _ctx.Users.Add(nonOwner);
        await _ctx.SaveChangesAsync();

        var dto = new DeleteContactTagDTO
        {
            Id = new() { AgendaContactId = contact.AgendaContactId, TagId = tag.TagId },
            Executor = new() { Id = nonOwner.UserId, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
        var relation = await _ctx.TagsPerUsers.FindAsync(tag.TagId, contact.AgendaContactId);
        Assert.NotNull(relation);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}