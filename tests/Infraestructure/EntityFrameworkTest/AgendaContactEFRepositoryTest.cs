using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Domain.Enums;
using EntityFramework.Application.DAOs.Contacts;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Domain.ValueObjects;

namespace EntityFrameworkTest;

public class AgendaContactEFRepositoryTest : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly EduZasDotnetContext _ctx;
    private readonly ContactEFCreator _creator;
    private readonly ContactEFReader _reader;
    private readonly ContactEFUpdater _updater;
    private readonly ContactEFQuerier _querier;
    private readonly User _user1;
    private readonly User _user2;

    public AgendaContactEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new ContactEFMapper();

        _creator = new(_ctx, mapper, mapper);
        _reader = new(_ctx, mapper);
        _updater = new(_ctx, mapper, mapper);
        _querier = new(_ctx, mapper, 10);

        _user1 = new User { Email = "test1@user.com", Password = "password", FirstName = "Test1", FatherLastname = "User" };
        _user2 = new User { Email = "test2@user.com", Password = "password", FirstName = "Test2", FatherLastname = "User" };
        _ctx.Users.AddRange(_user1, _user2);
        _ctx.SaveChanges();
    }

    [Fact]
    public async Task AddContact_ReturnsContact()
    {
        var newContact = new NewContactDTO
        {
            Alias = "Test Alias",
            Notes = "Test notes".ToOptional(),
            AgendaOwnerId = _user1.UserId,
            ContactId = _user2.UserId
        };

        var created = await _creator.AddAsync(newContact);

        Assert.NotNull(created);
        Assert.Equal(newContact.Alias, created.Alias);
        Assert.Equal(newContact.Notes.Unwrap(), created.Notes.Unwrap());
        Assert.Equal(newContact.AgendaOwnerId, created.AgendaOwnerId);
        Assert.Equal(newContact.ContactId, created.ContactId);
    }

    [Fact]
    public async Task AddContact_WithDuplicateContact_ThrowsDbUpdateException()
    {
        var newContact = new NewContactDTO
        {
            Alias = "Test Alias",
            Notes = "Test notes".ToOptional(),
            AgendaOwnerId = _user1.UserId,
            ContactId = _user2.UserId
        };
        await _creator.AddAsync(newContact);

        var duplicateContact = new NewContactDTO
        {
            Alias = "Duplicate Alias",
            Notes = "Duplicate notes".ToOptional(),
            AgendaOwnerId = _user1.UserId,
            ContactId = _user2.UserId
        };

        await Assert.ThrowsAsync<DbUpdateException>(() => _creator.AddAsync(duplicateContact));
    }

    [Fact]
    public async Task GetAsync_WhenContactExists_ReturnsContact()
    {
        var newContact = new NewContactDTO
        {
            Alias = "Test Alias",
            Notes = "Test notes".ToOptional(),
            AgendaOwnerId = _user1.UserId,
            ContactId = _user2.UserId
        };
        var created = await _creator.AddAsync(newContact);

        var found = await _reader.GetAsync(created.Id);

        Assert.True(found.IsSome);
        var foundContact = found.Unwrap();
        Assert.Equal(created.Id, foundContact.Id);
    }

    [Fact]
    public async Task GetAsync_WhenContactDoesNotExist_ReturnsEmptyOptional()
    {
        var found = await _reader.GetAsync(123);
        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task UpdateContact_ReturnsUpdatedContact()
    {
        var newContact = new NewContactDTO
        {
            Alias = "Test Alias",
            Notes = "Test notes".ToOptional(),
            AgendaOwnerId = _user1.UserId,
            ContactId = _user2.UserId
        };
        var created = await _creator.AddAsync(newContact);

        var update = new ContactUpdateDTO
        {
            Id = created.Id,
            Alias = "Updated Alias",
            Notes = "Updated notes".ToOptional(),
            AgendaOwnerId = _user1.UserId,
            ContactId = _user2.UserId
        };

        var updated = await _updater.UpdateAsync(update);

        Assert.NotNull(updated);
        Assert.Equal(update.Id, updated.Id);
        Assert.Equal(update.Alias, updated.Alias);
        Assert.Equal(update.Notes.Unwrap(), updated.Notes.Unwrap());
    }

    [Fact]
    public async Task GetByAsync_WithAliasCriteria_ReturnsMatchingContact()
    {
        var newContact1 = new NewContactDTO
        {
            Alias = "Alias 1",
            AgendaOwnerId = _user1.UserId,
            ContactId = _user2.UserId
        };
        await _creator.AddAsync(newContact1);

        var criteria = new ContactCriteriaDTO
        {
            Alias = new StringQueryDTO
            {
                Text = "Alias 1",
                SearchType = StringSearchType.EQ
            }
        };

        var result = await _querier.GetByAsync(criteria);

        Assert.Single(result.Results);
        Assert.Equal(newContact1.Alias, result.Results.First().Alias);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
