using Application.DTOs.Common;
using Application.DTOs.Tags;
using Domain.Enums;
using EntityFramework.Application.DAOs.Tags;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest;

public class TagEFRepositoryTest : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly EduZasDotnetContext _ctx;
    private readonly TagEFCreator _creator;
    private readonly TagEFQuerier _querier;

    public TagEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new TagEFMapper();

        _creator = new(_ctx, mapper, mapper);
        _querier = new(_ctx, mapper, 10);
    }

    [Fact]
    public async Task AddTag_ReturnsTag()
    {
        var newTag = new NewTagDTO { Text = "Test Tag" };

        var created = await _creator.AddAsync(newTag);
        Assert.NotNull(created);
        Assert.Equal(newTag.Text, created.Text);
    }

    [Fact]
    public async Task AddTag_WithDuplicateText_ThrowsInvalidOperationException()
    {
        var newTag = new NewTagDTO { Text = "Test Tag" };
        await _creator.AddAsync(newTag);

        var duplicateTag = new NewTagDTO { Text = "Test Tag" };

        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(duplicateTag));
    }

    [Fact]
    public async Task GetByAsync_WithTextCriteria_ReturnsMatchingTag()
    {
        var newTag = new NewTagDTO { Text = "Test Tag" };
        await _creator.AddAsync(newTag);

        var criteria = new TagCriteriaDTO
        {
            Text = new StringQueryDTO { Text = "Test Tag", SearchType = StringSearchType.EQ },
        };

        var result = await _querier.GetByAsync(criteria);

        Assert.Single(result.Results);
        Assert.Equal(newTag.Text, result.Results.First().Text);
    }

    [Fact]
    public async Task GetByAsync_WithNonexistentTextCriteria_ReturnsEmpty()
    {
        var criteria = new TagCriteriaDTO
        {
            Text = new StringQueryDTO
            {
                Text = "Nonexistent Tag",
                SearchType = StringSearchType.EQ,
            },
        };

        var result = await _querier.GetByAsync(criteria);

        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task GetByAsync_WithOwnerAgendaIdCriteria_ReturnsMatchingTags()
    {
        // Arrange
        var user = new User
        {
            Email = "test@user.com",
            Password = "password",
            FirstName = "Test",
            FatherLastname = "User",
        };
        var contactUser = new User
        {
            Email = "contact@user.com",
            Password = "password",
            FirstName = "Contact",
            FatherLastname = "User",
        };
        _ctx.Users.AddRange(user, contactUser);
        await _ctx.SaveChangesAsync();

        var contact = new AgendaContact
        {
            AgendaOwnerId = user.UserId,
            UserId = contactUser.UserId,
            Alias = "Test Contact",
        };
        _ctx.AgendaContacts.Add(contact);
        await _ctx.SaveChangesAsync();

        var tag1 = new Tag { Text = "Tag1" };
        var tag2 = new Tag { Text = "Tag2" };
        _ctx.Tags.AddRange(tag1, tag2);
        await _ctx.SaveChangesAsync();

        _ctx.ContactTags.AddRange(
            new ContactTag
            {
                AgendaOwnerId = contact.AgendaOwnerId,
                UserId = contact.UserId,
                TagText = tag1.Text,
            },
            new ContactTag
            {
                AgendaOwnerId = contact.AgendaOwnerId,
                UserId = contact.UserId,
                TagText = tag2.Text,
            }
        );
        await _ctx.SaveChangesAsync();

        var criteria = new TagCriteriaDTO { AgendaOwnerId = user.UserId };

        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Equal(2, result.Results.Count());
        Assert.Contains(result.Results, t => t.Text == "Tag1");
        Assert.Contains(result.Results, t => t.Text == "Tag2");
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
