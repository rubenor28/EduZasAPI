using Application.DAOs;
using Application.DTOs.Answers;
using Domain.Entities;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

using AnswerCreator = ICreatorAsync<AnswerDomain, AnswerIdDTO>;
using AnswerProfessorUpdater = IUpdaterAsync<AnswerDomain, AnswerUpdateProfessorDTO>;
using AnswerQuerier = IQuerierAsync<AnswerDomain, AnswerCriteriaDTO>;
using AnswerReader = IReaderAsync<AnswerIdDTO, AnswerDomain>;
using AnswerStudentUpdater = IUpdaterAsync<AnswerDomain, AnswerUpdateStudentDTO>;

public class AnswerRepositoryTest : BaseTest
{
    private readonly AnswerCreator _creator;
    private readonly AnswerReader _reader;
    private readonly AnswerQuerier _querier;
    private readonly AnswerStudentUpdater _studentUpdater;
    private readonly AnswerProfessorUpdater _professorUpdater;

    public AnswerRepositoryTest()
    {
        _creator = _sp.GetRequiredService<AnswerCreator>();
        _reader = _sp.GetRequiredService<AnswerReader>();
        _studentUpdater = _sp.GetRequiredService<AnswerStudentUpdater>();
        _professorUpdater = _sp.GetRequiredService<AnswerProfessorUpdater>();
        _querier = _sp.GetRequiredService<AnswerQuerier>();
    }

    [Fact]
    public async Task AddAnswer_RetunsAnswer()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(ownerId: professor.Id);
        var test = await SeedTest(professorId: professor.Id);
        await SeedClassTest(classId: cls.Id, testId: test.Id);
        var student = await SeedUser(UserType.STUDENT);

        // Act
        var newAnswer = new AnswerIdDTO
        {
            ClassId = cls.Id,
            TestId = test.Id,
            UserId = student.Id,
        };

        var answer = await _creator.AddAsync(newAnswer);

        // Assert
        Assert.NotNull(answer);
        Assert.Equal(cls.Id, answer.ClassId);
        Assert.Equal(student.Id, answer.UserId);
        Assert.Equal(test.Id, answer.TestId);
    }

    [Fact]
    public async Task GetExistentAnswer_ReturnsAnswer()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(ownerId: professor.Id);
        var test = await SeedTest(professorId: professor.Id);
        await SeedClassTest(classId: cls.Id, testId: test.Id);
        var student = await SeedUser(UserType.STUDENT);

        var id = new AnswerIdDTO
        {
            ClassId = cls.Id,
            TestId = test.Id,
            UserId = student.Id,
        };

        await _creator.AddAsync(id);

        // Act
        var answer = await _reader.GetAsync(id);

        // Assert
        Assert.NotNull(answer);
        Assert.Equal(cls.Id, answer.ClassId);
        Assert.Equal(student.Id, answer.UserId);
        Assert.Equal(test.Id, answer.TestId);
    }

    [Fact]
    public async Task GetNonExistentAnswer_ReturnsNull()
    {
        // Arrange
        var id = new AnswerIdDTO
        {
            TestId = Guid.NewGuid(),
            ClassId = "Non-existend",
            UserId = 9999,
        };

        // Act
        var answer = await _reader.GetAsync(id);

        // Arrange
        Assert.Null(answer);
    }

    public static TheoryData<
        Guid,
        IQuestion,
        IQuestionAnswer,
        Action<IQuestionAnswer>
    > GetQuestionTypes()
    {
        var data = new TheoryData<Guid, IQuestion, IQuestionAnswer, Action<IQuestionAnswer>>();
        var questionId = Guid.NewGuid();

        data.Add(
            questionId,
            new OpenQuestion { Title = "2x2" },
            new OpenQuestionAnswer { Text = "4" },
            res =>
            {
                var openQuestion = Assert.IsType<OpenQuestionAnswer>(res);
                Assert.Equal("4", openQuestion.Text);
            }
        );

        data.Add(
            Guid.NewGuid(),
            new ConceptRelationQuestion
            {
                Title = "Conceptos",
                Concepts = new HashSet<ConceptPair> { new("A", "B"), new("C", "D") },
            },
            new ConceptRelationQuestionAnswer
            {
                AnsweredPairs = new HashSet<ConceptPair> { new("A", "B"), new("C", "D") },
            },
            res =>
            {
                var val = Assert.IsType<ConceptRelationQuestionAnswer>(res);
                Assert.True(val.AnsweredPairs.Contains(new("A", "B")));
                Assert.True(val.AnsweredPairs.Contains(new("C", "D")));
            }
        );

        var correctOption = Guid.NewGuid();
        var options = new Dictionary<Guid, string>
        {
            { correctOption, "Paris" },
            { Guid.NewGuid(), "Brazilia" },
            { Guid.NewGuid(), "Washington" },
        };

        data.Add(
            Guid.NewGuid(),
            new MultipleChoiseQuestion
            {
                Title = "Capital de Francia",
                Options = options,
                CorrectOption = correctOption,
            },
            new MultipleChoiseQuestionAnswer { SelectedOption = correctOption },
            res =>
            {
                var val = Assert.IsType<MultipleChoiseQuestionAnswer>(res);
                Assert.Equal(correctOption, val.SelectedOption);
            }
        );

        var correctOptions = new HashSet<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var incorrectOption = Guid.NewGuid();
        var selectionOptions = new Dictionary<Guid, string>
        {
            { correctOptions.ElementAt(0), "A" },
            { correctOptions.ElementAt(1), "B" },
            { incorrectOption, "C" },
        };

        data.Add(
            Guid.NewGuid(),
            new MultipleSelectionQuestion
            {
                Title = "Select A and B",
                Options = selectionOptions,
                CorrectOptions = correctOptions,
            },
            new MultipleSelectionQuestionAnswer { SelectedOptions = correctOptions },
            res =>
            {
                var val = Assert.IsType<MultipleSelectionQuestionAnswer>(res);
                Assert.Equal(correctOptions.Count, val.SelectedOptions.Count);
                Assert.Contains(correctOptions.ElementAt(0), val.SelectedOptions);
                Assert.Contains(correctOptions.ElementAt(1), val.SelectedOptions);
            }
        );

        var sequence = new List<string> { "First", "Second", "Third" };

        data.Add(
            Guid.NewGuid(),
            new OrderingQuestion { Title = "Order the sequence", Sequence = sequence },
            new OrderingQuestionAnswer { Sequence = [.. sequence.AsEnumerable().Reverse()] },
            res =>
            {
                var val = Assert.IsType<OrderingQuestionAnswer>(res);
                Assert.Equal(sequence.Count, val.Sequence.Count);
                Assert.Equal([.. sequence.AsEnumerable().Reverse()], val.Sequence);
            }
        );

        return data;
    }

    [Theory]
    [MemberData(nameof(GetQuestionTypes))]
    public async Task StudentUpdateAnswer_ReturnsUpdated_ForMultipleTypes(
        Guid questionId,
        IQuestion questionObj,
        IQuestionAnswer answerObj,
        Action<IQuestionAnswer> assertSpecifics
    ) // Acción para validar campos únicos
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var student = await SeedUser(UserType.STUDENT);
        var cls = await SeedClass(ownerId: professor.Id);

        var questions = new Dictionary<Guid, IQuestion> { { questionId, questionObj } };

        var test = await SeedTest(professorId: professor.Id, questions);
        await SeedClassTest(classId: cls.Id, testId: test.Id);

        var answer = new AnswerIdDTO
        {
            ClassId = cls.Id,
            TestId = test.Id,
            UserId = student.Id,
        };
        await _creator.AddAsync(answer);

        // Act
        var update = new AnswerUpdateStudentDTO
        {
            ClassId = cls.Id,
            UserId = student.Id,
            TestId = test.Id,
            Content = new Dictionary<Guid, IQuestionAnswer> { { questionId, answerObj } },
        };

        var updated = await _studentUpdater.UpdateAsync(update);

        // Assert
        Assert.NotNull(updated);
        Assert.True(updated.Content.ContainsKey(questionId));
        assertSpecifics(updated.Content[questionId]); // Validación específica
    }

    [Fact]
    public async Task ProfessorUpdateAnswer_ReturnsUpdated()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var student = await SeedUser(UserType.STUDENT);
        var cls = await SeedClass(ownerId: professor.Id);

        var questionId = Guid.NewGuid();
        var questions = new Dictionary<Guid, IQuestion>
        {
            {
                questionId,
                new OpenQuestion { Title = "Explain this" }
            },
        };

        var test = await SeedTest(professorId: professor.Id, questions);
        await SeedClassTest(classId: cls.Id, testId: test.Id);

        var answerId = new AnswerIdDTO
        {
            ClassId = cls.Id,
            TestId = test.Id,
            UserId = student.Id,
        };
        await _creator.AddAsync(answerId);

        var studentUpdate = new AnswerUpdateStudentDTO
        {
            ClassId = cls.Id,
            UserId = student.Id,
            TestId = test.Id,
            Content = new Dictionary<Guid, IQuestionAnswer>
            {
                {
                    questionId,
                    new OpenQuestionAnswer { Text = "My explanation" }
                },
            },
        };
        await _studentUpdater.UpdateAsync(studentUpdate);

        // Act
        var professorUpdate = new AnswerUpdateProfessorDTO
        {
            ClassId = cls.Id,
            UserId = student.Id,
            TestId = test.Id,
            Metadata = new AnswerMetadata
            {
                ManualGrade = new Dictionary<Guid, bool> { { questionId, true } },
            },
        };

        var updated = await _professorUpdater.UpdateAsync(professorUpdate);

        // Assert
        Assert.NotNull(updated);
        Assert.NotNull(updated.Metadata);
        Assert.Single(updated.Metadata.ManualGrade);
        Assert.Contains(questionId, updated.Metadata.ManualGrade);
    }

    [Fact]
    public async Task GetByAsync_WithCriteria_ReturnsMatchingAnswer()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(ownerId: professor.Id);
        var test = await SeedTest(professorId: professor.Id);
        await SeedClassTest(classId: cls.Id, testId: test.Id);
        var student1 = await SeedUser(UserType.STUDENT);
        var student2 = await SeedUser(UserType.STUDENT);

        await _creator.AddAsync(
            new AnswerIdDTO
            {
                ClassId = cls.Id,
                TestId = test.Id,
                UserId = student1.Id,
            }
        );
        await _creator.AddAsync(
            new AnswerIdDTO
            {
                ClassId = cls.Id,
                TestId = test.Id,
                UserId = student2.Id,
            }
        );

        var criteria = new AnswerCriteriaDTO
        {
            UserId = student1.Id,
            ClassId = cls.Id,
            TestId = test.Id,
        };

        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Single(result.Results);
        var answer = result.Results.First();
        Assert.Equal(student1.Id, answer.UserId);
        Assert.Equal(cls.Id, answer.ClassId);
        Assert.Equal(test.Id, answer.TestId);
    }

    [Fact]
    public async Task GetByAsync_WithTestOwnerIdCriteria_ReturnsMatchingAnswers()
    {
        // Arrange
        var professor1 = await SeedUser(UserType.PROFESSOR);
        var professor2 = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(ownerId: professor1.Id);
        var test1 = await SeedTest(professorId: professor1.Id);
        var test2 = await SeedTest(professorId: professor2.Id);
        await SeedClassTest(classId: cls.Id, testId: test1.Id);
        await SeedClassTest(classId: cls.Id, testId: test2.Id);
        var student = await SeedUser(UserType.STUDENT);

        await _creator.AddAsync(
            new AnswerIdDTO
            {
                ClassId = cls.Id,
                TestId = test1.Id,
                UserId = student.Id,
            }
        );
        await _creator.AddAsync(
            new AnswerIdDTO
            {
                ClassId = cls.Id,
                TestId = test2.Id,
                UserId = student.Id,
            }
        );

        var criteria = new AnswerCriteriaDTO { TestOwnerId = professor1.Id };

        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Single(result.Results);
        var answer = result.Results.First();
        Assert.Equal(test1.Id, answer.TestId);
    }
}
