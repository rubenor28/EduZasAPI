using Application.DTOs.ContactTags;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.ContactTags;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tag = EntityFramework.Application.DTOs.Tag;
using User = EntityFramework.Application.DTOs.User;

namespace EntityFrameworkTest;

public class ContactTagEFRepositoryTest : IDisposable
{
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly ContactTagEFCreator _creator;
    private readonly ContactTagEFReader _reader;
    private readonly ContactTagEFDeleter _deleter;

    public ContactTagEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new ContactTagEFMapper();

        _creator = new(_ctx, mapper, mapper);
        _reader = new(_ctx, mapper);
        _deleter = new(_ctx, mapper);
    }

    private async Task<(User, User)> SeedData()
    {
        var ownerUser = new User
        {
            UserId = 1,
            Email = "owner@test.com",
            FirstName = "owner",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)UserType.PROFESSOR,
        };
        var contactUser = new User
        {
            UserId = 2,
            Email = "contact@test.com",
            FirstName = "contact",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)UserType.PROFESSOR,
        };

        var agendaContact = new AgendaContact
        {
            AgendaOwnerId = 1,
            UserId = 2,
            Alias = "test-contact",
        };

        var tag = new Tag { Text = "tag-test" };

        _ctx.Users.AddRange(ownerUser, contactUser);
        _ctx.AgendaContacts.Add(agendaContact);
        _ctx.Tags.Add(tag);
        await _ctx.SaveChangesAsync();

        return (ownerUser, contactUser);
    }

    [Fact]
    public async Task Add_ValidRelation_ReturnsRelation()
    {
        var (owner, contact) = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            Tag = "tag-test",
            Executor = new() { Id = owner.UserId, Role = UserType.ADMIN },
        };

        var created = await _creator.AddAsync(newRelationDto);

        Assert.NotNull(created);
        Assert.Equal(newRelationDto.AgendaOwnerId, created.Id.AgendaOwnerId);
        Assert.Equal(newRelationDto.UserId, created.Id.UserId);
        Assert.Equal(newRelationDto.Tag, created.Id.Tag);
    }

    [Fact]
    public async Task Add_DuplicateRelation_ThrowsException()
    {
        var (owner, contact) = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            Tag = "tag-test",
            Executor = new() { Id = owner.UserId, Role = UserType.ADMIN },
        };

        await _creator.AddAsync(newRelationDto);

        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelationDto));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        var (owner, contact) = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            Tag = "tag-test",
            Executor = new() { Id = owner.UserId, Role = UserType.ADMIN },
        };

        await _creator.AddAsync(newRelationDto);

        var idToFind = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            Tag = "tag-test",
        };

        var found = await _reader.GetAsync(idToFind);

        Assert.True(found.IsSome);
        Assert.Equal(newRelationDto.AgendaOwnerId, found.Unwrap().Id.AgendaOwnerId);
        Assert.Equal(newRelationDto.UserId, found.Unwrap().Id.UserId);
        Assert.Equal(newRelationDto.Tag, found.Unwrap().Id.Tag);
    }

    [Fact]
    public async Task Get_NonExistingRelation_ReturnsEmptyOptional()
    {
        var found = await _reader.GetAsync(
            new()
            {
                AgendaOwnerId = 99,
                UserId = 98,
                Tag = "non-exists",
            }
        );

        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        var (owner, contact) = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            Tag = "tag-test",
            Executor = new() { Id = owner.UserId, Role = UserType.ADMIN },
        };

        await _creator.AddAsync(newRelationDto);

        var idToDelete = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            Tag = "tag-test",
        };

        var deleted = await _deleter.DeleteAsync(idToDelete);

        Assert.NotNull(deleted);
        Assert.Equal(newRelationDto.AgendaOwnerId, deleted.Id.AgendaOwnerId);
        Assert.Equal(newRelationDto.UserId, deleted.Id.UserId);
        Assert.Equal(newRelationDto.Tag, deleted.Id.Tag);

        var found = await _reader.GetAsync(idToDelete);
        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task Delete_NonExistingRelation_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _deleter.DeleteAsync(
                new()
                {
                    AgendaOwnerId = 99,
                    UserId = 98,
                    Tag = "tag-test",
                }
            )
        );
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
