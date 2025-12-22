using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class AgendaContactEFRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<ContactDomain, NewContactDTO> _creator;
    private readonly IReaderAsync<ContactIdDTO, ContactDomain> _reader;
    private readonly IUpdaterAsync<ContactDomain, ContactUpdateDTO> _updater;
    private readonly IQuerierAsync<ContactDomain, ContactCriteriaDTO> _querier;

    public AgendaContactEFRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<ContactDomain, NewContactDTO>>();
        _reader = _sp.GetRequiredService<IReaderAsync<ContactIdDTO, ContactDomain>>();
        _updater = _sp.GetRequiredService<IUpdaterAsync<ContactDomain, ContactUpdateDTO>>();
        _querier = _sp.GetRequiredService<IQuerierAsync<ContactDomain, ContactCriteriaDTO>>();
    }

    [Fact]
    public async Task AddContact_ReturnsContact()
    {
        var user1 = await SeedUser(UserType.PROFESSOR);
        var user2 = await SeedUser(UserType.PROFESSOR);
        var newContact = new NewContactDTO
        {
            Alias = "Test Alias",
            Notes = "Test notes",
            AgendaOwnerId = user1.Id,
            UserId = user2.Id,
            Tags = [],
        };

        var created = await _creator.AddAsync(newContact);

        Assert.NotNull(created);
        Assert.Equal(newContact.Alias, created.Alias);
        Assert.Equal(newContact.Notes, created.Notes);
        Assert.Equal(newContact.AgendaOwnerId, created.AgendaOwnerId);
        Assert.Equal(newContact.UserId, created.UserId);
    }

    [Fact]
    public async Task AddContact_WithDuplicateContact_ThrowsDbUpdateException()
    {
        var user1 = await SeedUser(UserType.PROFESSOR);
        var user2 = await SeedUser(UserType.PROFESSOR);
        var newContact = new NewContactDTO
        {
            Alias = "Test Alias",
            Notes = "Test notes",
            AgendaOwnerId = user1.Id,
            UserId = user2.Id,
            Tags = [],
        };
        await _creator.AddAsync(newContact);

        var duplicateContact = new NewContactDTO
        {
            Alias = "Duplicate Alias",
            Notes = "Duplicate notes",
            AgendaOwnerId = user1.Id,
            UserId = user2.Id,
            Tags = [],
        };

        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(duplicateContact));
    }

    [Fact]
    public async Task GetAsync_WhenContactExists_ReturnsContact()
    {
        var user1 = await SeedUser(UserType.PROFESSOR);
        var user2 = await SeedUser(UserType.PROFESSOR);
        var created = await SeedContact(user1.Id, user2.Id);

        var found = await _reader.GetAsync(
            new() { UserId = created.UserId, AgendaOwnerId = created.AgendaOwnerId }
        );

        Assert.NotNull(found);
        Assert.Equal(created.UserId, found.UserId);
        Assert.Equal(created.AgendaOwnerId, found.AgendaOwnerId);
    }

    [Fact]
    public async Task GetAsync_WhenContactDoesNotExist_ReturnsEmptyOptional()
    {
        var found = await _reader.GetAsync(new() { AgendaOwnerId = 98, UserId = 99 });
        Assert.Null(found);
    }

    [Fact]
    public async Task UpdateContact_ReturnsUpdatedContact()
    {
        var user1 = await SeedUser(UserType.PROFESSOR);
        var user2 = await SeedUser(UserType.PROFESSOR);
        var created = await SeedContact(user1.Id, user2.Id);

        var update = new ContactUpdateDTO
        {
            AgendaOwnerId = created.AgendaOwnerId,
            UserId = created.UserId,
            Alias = "Updated Alias",
            Notes = "Updated notes",
        };

        var updated = await _updater.UpdateAsync(update);

        Assert.NotNull(updated);
        Assert.Equal(update.UserId, updated.UserId);
        Assert.Equal(update.AgendaOwnerId, updated.AgendaOwnerId);
        Assert.Equal(update.Alias, updated.Alias);
        Assert.Equal(update.Notes, updated.Notes);
    }

    [Fact]
    public async Task GetByAsync_WithAliasCriteria_ReturnsMatchingContact()
    {
        var user1 = await SeedUser(UserType.PROFESSOR);
        var user2 = await SeedUser(UserType.PROFESSOR);
        await SeedContact(user1.Id, user2.Id);

        var criteria = new ContactCriteriaDTO
        {
            Alias = new StringQueryDTO { Text = "Test Contact", SearchType = StringSearchType.EQ },
        };

        var result = await _querier.GetByAsync(criteria);

        Assert.Single(result.Results);
        Assert.Equal("Test Contact", result.Results.First().Alias);
    }
}

