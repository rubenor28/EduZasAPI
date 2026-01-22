using Application.DAOs;
using Application.DTOs.Tags;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class TagRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<TagDomain, NewTagDTO> _creator;
    private readonly IQuerierAsync<TagDomain, TagCriteriaDTO> _querier;

    public TagRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<TagDomain, NewTagDTO>>();
        _querier = _sp.GetRequiredService<IQuerierAsync<TagDomain, TagCriteriaDTO>>();
    }

    [Fact]
    public async Task AddTag_ReturnsTag()
    {
        // Arrange
        var newTag = new NewTagDTO { Text = "Test Tag" };

        // Act
        var created = await _creator.AddAsync(newTag);
        
        // Assert
        Assert.NotNull(created);
        Assert.Equal(newTag.Text, created.Text);
    }

    [Fact]
    public async Task AddTag_WithDuplicateText_ThrowsInvalidOperationException()
    {
        // Arrange
        var newTag = new NewTagDTO { Text = "Test Tag" };
        await _creator.AddAsync(newTag);

        var duplicateTag = new NewTagDTO { Text = "Test Tag" };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(duplicateTag));
    }

    [Fact]
    public async Task GetByAsync_WithTextCriteria_ReturnsMatchingTag()
    {
        // Arrange
        var newTag = new NewTagDTO { Text = "Test Tag" };
        await _creator.AddAsync(newTag);

        var criteria = new TagCriteriaDTO
        {
            Text = new StringQueryDTO { Text = "Test Tag", SearchType = StringSearchType.EQ },
        };

        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Single(result.Results);
        Assert.Equal(newTag.Text, result.Results.First().Text);
    }

    [Fact]
    public async Task GetByAsync_WithNonexistentTextCriteria_ReturnsEmpty()
    {
        // Arrange
        var criteria = new TagCriteriaDTO
        {
            Text = new StringQueryDTO
            {
                Text = "Nonexistent Tag",
                SearchType = StringSearchType.EQ,
            },
        };

        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task GetByAsync_WithOwnerAgendaIdCriteria_ReturnsMatchingTags()
    {
        // Arrange
        var user = await SeedUser(UserType.PROFESSOR);
        var contactUser = await SeedUser(UserType.PROFESSOR);
        await SeedContact(user.Id, contactUser.Id);

        var tag1 = await SeedTag("Tag1");
        var tag2 = await SeedTag("Tag2");

        await SeedContactTag(user.Id, contactUser.Id, tag1.Id);
        await SeedContactTag(user.Id, contactUser.Id, tag2.Id);
        
        var criteria = new TagCriteriaDTO { AgendaOwnerId = user.Id };

        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Equal(2, result.Results.Count());
        Assert.Contains(result.Results, t => t.Text == "Tag1");
        Assert.Contains(result.Results, t => t.Text == "Tag2");
    }
}

