using Application.DTOs.Common;
using Application.DTOs.ContactTag;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.ContactTag;
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

    private async Task<User> SeedData()
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
            AgendaContactId = 1,
            AgendaOwnerId = 1,
            ContactId = 2,
            Alias = "test-contact",
        };

        var tag = new Tag { TagId = 1, Text = "tag-test" };

        _ctx.Users.AddRange(ownerUser, contactUser);
        _ctx.AgendaContacts.Add(agendaContact);
        _ctx.Tags.Add(tag);
        await _ctx.SaveChangesAsync();
        return ownerUser;
    }

    [Fact]
    public async Task Add_ValidRelation_ReturnsRelation()
    {
        var owner = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaContactId = 1,
            TagId = 1,
            Executor = new Executor { Id = owner.UserId, Role = (UserType)owner.Role! },
        };

        var created = await _creator.AddAsync(newRelationDto);

        Assert.NotNull(created);
        Assert.Equal(newRelationDto.AgendaContactId, created.Id.AgendaContactId);
        Assert.Equal(newRelationDto.TagId, created.Id.TagId);
    }

    [Fact]
    public async Task Add_DuplicateRelation_ThrowsException()
    {
        var owner = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaContactId = 1,
            TagId = 1,
            Executor = new Executor { Id = owner.UserId, Role = (UserType)owner.Role! },
        };
        await _creator.AddAsync(newRelationDto);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _creator.AddAsync(newRelationDto)
        );
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        var owner = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaContactId = 1,
            TagId = 1,
            Executor = new Executor { Id = owner.UserId, Role = (UserType)owner.Role! },
        };
        await _creator.AddAsync(newRelationDto);

        var idToFind = new ContactTagIdDTO { AgendaContactId = 1, TagId = 1 };
        var found = await _reader.GetAsync(idToFind);

        Assert.True(found.IsSome);
        Assert.Equal(newRelationDto.AgendaContactId, found.Unwrap().Id.AgendaContactId);
        Assert.Equal(newRelationDto.TagId, found.Unwrap().Id.TagId);
    }

    [Fact]
    public async Task Get_NonExistingRelation_ReturnsEmptyOptional()
    {
        var found = await _reader.GetAsync(new() { AgendaContactId = 99, TagId = 98 });

        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        var owner = await SeedData();
        var newRelationDto = new NewContactTagDTO
        {
            AgendaContactId = 1,
            TagId = 1,
            Executor = new Executor { Id = owner.UserId, Role = (UserType)owner.Role! },
        };
        await _creator.AddAsync(newRelationDto);

        var idToDelete = new ContactTagIdDTO { AgendaContactId = 1, TagId = 1 };
        var deleted = await _deleter.DeleteAsync(idToDelete);

        Assert.NotNull(deleted);
        Assert.Equal(newRelationDto.AgendaContactId, deleted.Id.AgendaContactId);
        Assert.Equal(newRelationDto.TagId, deleted.Id.TagId);

        var found = await _reader.GetAsync(idToDelete);
        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task Delete_NonExistingRelation_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _deleter.DeleteAsync(new() { AgendaContactId = 99, TagId = 98 })
        );
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
