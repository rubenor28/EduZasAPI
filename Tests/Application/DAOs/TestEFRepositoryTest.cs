using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Tests;
using Domain.Entities;
using Domain.Entities.Questions;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class TestEFRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<TestDomain, NewTestDTO> _creator;
    private readonly IUpdaterAsync<TestDomain, TestUpdateDTO> _updater;
    private readonly IReaderAsync<Guid, TestDomain> _reader;
    private readonly IQuerierAsync<TestDomain, TestCriteriaDTO> _querier;
    private readonly IDeleterAsync<Guid, TestDomain> _deleter;

    public TestEFRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<TestDomain, NewTestDTO>>();
        _updater = _sp.GetRequiredService<IUpdaterAsync<TestDomain, TestUpdateDTO>>();
        _reader = _sp.GetRequiredService<IReaderAsync<Guid, TestDomain>>();
        _querier = _sp.GetRequiredService<IQuerierAsync<TestDomain, TestCriteriaDTO>>();
        _deleter = _sp.GetRequiredService<IDeleterAsync<Guid, TestDomain>>();
    }

    [Fact]
    public async Task AddTest_ReturnsTest()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var openQuestionId = Guid.NewGuid();
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Color = "#ffffff",
            Content = new Dictionary<Guid, IQuestion>
            {
                [openQuestionId] = new OpenQuestion
                {
                    Title = "Open Question Title",
                    ImageUrl = null
                }
            },
            ProfessorId = professor.Id,
        };

        // Act
        var created = await _creator.AddAsync(newTest);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(newTest.Title, created.Title);
        Assert.NotNull(created.Content);
        Assert.Single(created.Content);
        Assert.True(created.Content.ContainsKey(openQuestionId));
        var question = Assert.IsType<OpenQuestion>(created.Content[openQuestionId]);
        Assert.Equal("Open Question Title", question.Title);
    }

    [Fact]
    public async Task UpdateTest_ReturnsUpdatedTest()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var newTest = new NewTestDTO
        {
            Title = "Original Title",
            Color = "#000000",
            Content = new Dictionary<Guid, IQuestion>(),
            ProfessorId = professor.Id,
        };
        var created = await _creator.AddAsync(newTest);

        var newContentId = Guid.NewGuid();
        var update = new TestUpdateDTO
        {
            Id = created.Id,
            Color = "#ffffff",
            Title = "Updated Test Title",
            Content = new Dictionary<Guid, IQuestion>
            {
                [newContentId] = new OpenQuestion { Title = "Updated Question", ImageUrl = "image.png" }
            },
            ProfessorId = professor.Id,
            Active = false,
        };

        // Act
        var updatedTest = await _updater.UpdateAsync(update);

        // Assert
        Assert.NotNull(updatedTest);
        Assert.Equal(update.Title, updatedTest.Title);
        Assert.Equal(update.Active, updatedTest.Active);
        Assert.NotNull(updatedTest.Content);
        Assert.Single(updatedTest.Content);
        Assert.True(updatedTest.Content.ContainsKey(newContentId));
        var question = Assert.IsType<OpenQuestion>(updatedTest.Content[newContentId]);
        Assert.Equal("Updated Question", question.Title);
        Assert.Equal("image.png", question.ImageUrl);
    }

    [Fact]
    public async Task GetAsync_WhenTestExists_ReturnsTest()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var openQuestionId = Guid.NewGuid();
        var mcqId = Guid.NewGuid();
        var correctOptionId = Guid.NewGuid();
        var contentToSave = new Dictionary<Guid, IQuestion>
        {
            [openQuestionId] = new OpenQuestion
            {
                Title = "Open Q",
                ImageUrl = "url"
            },
            [mcqId] = new MultipleChoiseQuestion
            {
                Title = "MCQ",
                ImageUrl = null,
                Options = new Dictionary<Guid, string>
                {
                    { Guid.NewGuid(), "A" },
                    { correctOptionId, "B" }
                },
                CorrectOption = correctOptionId
            }
        };
        
        var created = await SeedTest(professor.Id, contentToSave);

        // Act
        var foundTest = await _reader.GetAsync(created.Id);

        // Assert
        Assert.NotNull(foundTest);
        Assert.Equal(created.Id, foundTest.Id);
        Assert.NotNull(foundTest.Content);
        Assert.Equal(2, foundTest.Content.Count);

        Assert.True(foundTest.Content.ContainsKey(openQuestionId));
        var retrievedOpenQ = Assert.IsType<OpenQuestion>(foundTest.Content[openQuestionId]);
        Assert.Equal("Open Q", retrievedOpenQ.Title);
        Assert.Equal("url", retrievedOpenQ.ImageUrl);

        Assert.True(foundTest.Content.ContainsKey(mcqId));
        var retrievedMcq = Assert.IsType<MultipleChoiseQuestion>(foundTest.Content[mcqId]);
        Assert.Equal("MCQ", retrievedMcq.Title);
        Assert.Equal(correctOptionId, retrievedMcq.CorrectOption);
        Assert.Equal("B", retrievedMcq.Options[correctOptionId]);
    }

    [Fact]
    public async Task GetAsync_WhenTestDoesNotExist_ReturnsEmptyOptional()
    {
        // Act
        var foundTest = await _reader.GetAsync(Guid.NewGuid());
        
        // Assert
        Assert.Null(foundTest);
    }

    [Fact]
    public async Task DeleteAsync_WhenTestExists_ReturnsDeletedTest()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var created = await SeedTest(professor.Id, new Dictionary<Guid, IQuestion>
        {
            { Guid.NewGuid(), new OpenQuestion { Title = "Q", ImageUrl = null } }
        });

        // Act
        var deletedTest = await _deleter.DeleteAsync(created.Id);
        var foundTest = await _reader.GetAsync(created.Id);

        // Assert
        Assert.NotNull(deletedTest);
        Assert.Equal(created.Id, deletedTest.Id);
        Assert.Null(foundTest);
    }

    [Fact]
    public async Task DeleteAsync_WhenTestDoesNotExist_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _deleter.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByAsync_WithTitleCriteria_ReturnsMatchingTest()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var test1 = await SeedTest(professor.Id);


        var criteria = new TestCriteriaDTO
        {
            Title = new StringQueryDTO { Text = test1.Title, SearchType = StringSearchType.EQ },
        };

        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Single(result.Results);
        Assert.Equal(test1.Title, result.Results.First().Title);
    }
}

