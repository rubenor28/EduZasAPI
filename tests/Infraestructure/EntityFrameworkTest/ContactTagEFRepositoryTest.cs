using Application.DTOs.ContactTags;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.ContactTags;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.ContactTags;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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

        var mapper = new ContactTagMapper();

        _creator = new(_ctx, mapper, new NewContactTagEFMapper());
        _reader = new(_ctx, mapper);
        _deleter = new(_ctx, mapper);
    }

    private async Task<(User, User, Tag)> SeedData()
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

        return (ownerUser, contactUser, tag);
    }

    [Fact]
    public async Task Add_ValidRelation_ReturnsRelation()
    {
        var (owner, contact, tag) = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            TagId = tag.TagId,
        };

        var created = await _creator.AddAsync(newRelationDto);

        Assert.NotNull(created);
        Assert.Equal(newRelationDto.AgendaOwnerId, created.Id.AgendaOwnerId);
        Assert.Equal(newRelationDto.UserId, created.Id.UserId);
        Assert.Equal(newRelationDto.TagId, created.Id.TagId);
    }

    [Fact]
    public async Task Add_DuplicateRelation_ThrowsException()
    {
        var (owner, contact, tag) = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            TagId = tag.TagId,
        };

        await _creator.AddAsync(newRelationDto);

        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelationDto));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        var (owner, contact,tag) = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            TagId = tag.TagId,
        };

        await _creator.AddAsync(newRelationDto);

        var idToFind = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            TagId = tag.TagId,
        };

        var found = await _reader.GetAsync(idToFind);

        Assert.NotNull(found);
        Assert.Equal(newRelationDto.AgendaOwnerId, found.Id.AgendaOwnerId);
        Assert.Equal(newRelationDto.UserId, found.Id.UserId);
        Assert.Equal(newRelationDto.TagId, found.Id.TagId);
    }

    [Fact]
    public async Task Get_NonExistingRelation_ReturnsEmptyOptional()
    {
        var found = await _reader.GetAsync(
            new()
            {
                AgendaOwnerId = 99,
                UserId = 98,
                TagId = 97,
            }
        );

        Assert.Null(found);
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        var (owner, contact, tag) = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            TagId = tag.TagId,
        };

        await _creator.AddAsync(newRelationDto);

        var idToDelete = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.UserId,
            UserId = contact.UserId,
            TagId = tag.TagId,
        };

        var deleted = await _deleter.DeleteAsync(idToDelete);

        Assert.NotNull(deleted);
        Assert.Equal(newRelationDto.AgendaOwnerId, deleted.Id.AgendaOwnerId);
        Assert.Equal(newRelationDto.UserId, deleted.Id.UserId);
        Assert.Equal(newRelationDto.TagId, deleted.Id.TagId);

        var found = await _reader.GetAsync(idToDelete);
        Assert.Null(found);
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
                    TagId = 97,
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
